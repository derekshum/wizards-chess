//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using WizardsChess.Movement.Drv;
//using WizardsChess.Movement.Drv.Events;
//using WizardsChess.Movement.Events;
//using WizardsChess.Movement.Exceptions;

//namespace WizardsChess.Movement
//{
//	enum MoveState
//	{
//		Stopped,
//		Moving,
//		HomingForwards,
//		HomingBackwards,
//		PreparingToCalibrate,
//		Calibrating
//	}
	
//	public class CalibratedMotorMoverOld
//	{
//		public CalibratedMotorMoverOld(Axis axis, int gridMax, int gridMin, int msPerStep, IMotorDrv mtrDrv, IPositionSignaler stepCntr,
//			IPhotoInterrupter topPhotoInterrupt, IPhotoInterrupter bottomPhotoInterrupt)
//		{
//			stepPosition = 0;
//			gridPosition = 0;
//			estimatedExtraSteps = 100;
//			StepsPerGridUnit = 100;
//			millisecondsPerStep = msPerStep;

//			this.axis = axis;
//			state = MoveState.Stopped;
//			isCalibrating = false;

//			this.gridMax = gridMax;
//			this.gridMin = gridMin;

//			motorDrv = mtrDrv;
//			stepCounter = stepCntr;
//			topInterrupt = topPhotoInterrupt;
//			bottomInterrupt = bottomPhotoInterrupt;

//			stepCounter.FinishedCounting += finishedCounting;
//			stepCounter.AdditionalStepsCounted += additionalStepsCounted;
//			stepCounter.MoveTimedOut += moveTimedOut;
//			topInterrupt.ValueChanged += topEdgeDetected;
//			bottomInterrupt.ValueChanged += bottomEdgeDetected;
//		}

//		/// <summary>
//		/// The current GridPosition. This is invalid if CalibratedMotorMove is in the middle of a move.
//		/// </summary>
//		public int GridPosition { get { return gridPosition; } }

//		/// <summary>
//		/// The current StepPosition. This is invalid if CalibratedMotorMove is in the middle of a move.
//		/// </summary>
//		public int StepPosition { get { return stepPosition; } }

//		/// <summary>
//		/// The number of steps between grid units, as a double.
//		/// </summary>
//		public double StepsPerGridUnit { get; private set; }

//		/// <summary>
//		/// Calibrates this motor. This moves the motor across the photointerrupters to determine the grid size and absolute position.
//		/// </summary>
//		/// <returns>An awaitable Task.</returns>
//		public async Task CalibrateAsync()
//		{
//			if (state == MoveState.Moving)
//			{
//				System.Diagnostics.Debug.WriteLine("Started calibration during a move.");
//			}
//			System.Diagnostics.Debug.WriteLine($"Started {axis} calibration.");

//			lowerBottomPosition = null;
//			upperBottomPosition = null;
//			lowerTopPosition = null;
//			upperTopPosition = null;

//			// TODO: Optimize move direction based on current position
//			// Look for the interrupter going forwards first
//			isCalibrating = true;
//			System.Diagnostics.Debug.WriteLine($"Homing forwards on {axis} axis.");
//			await moveAsync((gridMax - gridMin) / 2);
//			state = MoveState.HomingForwards;

//			// Don't return until calibration is complete
//			while (isCalibrating)
//			{
//				await Task.Delay(300);
//			}

//			updateCalibrationSettings();
//		}

//		/// <summary>
//		/// Move this axis "gridUnits" away from its approximate current grid position.
//		/// Limits the move to be within the expected grid size. Returns when the move is complete.
//		/// </summary>
//		/// <param name="gridUnits">The number of gridUnits to move. Can be positive or negative.</param>
//		/// <returns>An awaitable Task.</returns>
//		public async Task MoveAsync(int gridUnits)
//		{
//			if (gridUnits == 0)
//			{
//				return;
//			}

//			if (GridPosition + gridUnits > gridMax)
//			{
//				gridUnits = gridMax - GridPosition;
//			}
//			else if (GridPosition + gridUnits < gridMin)
//			{
//				gridUnits = gridMin - GridPosition;
//			}

//			await moveAsync(gridUnits);

//			state = MoveState.Moving;

//			while (state != MoveState.Stopped)
//			{
//				await Task.Delay(100);
//			}

//			var stillToMove = convertGridUnitsToSteps(GridPosition) - StepPosition;
//			if (stillToMove > 5)
//			{
//				moveInSteps((int)((float)stillToMove / 1.5));
//				state = MoveState.Moving;
//			}

//			while (state != MoveState.Stopped)
//			{
//				await Task.Delay(100);
//			}
//		}

//		private async Task moveAsync(int gridUnits)
//		{
//			if (gridUnits == 0)
//			{
//				System.Diagnostics.Debug.WriteLine($"Stopping the {axis} axis.");
//				motorDrv.Direction = MoveDirection.Stopped;
//				stepCounter.CountToPosition(0, TimeSpan.FromMilliseconds(100));
//			}
//			else
//			{
//				while (state != MoveState.Stopped)
//				{
//					await Task.Delay(50);
//				}

//				var offset = StepPosition - convertGridUnitsToSteps(GridPosition);
//				var distanceToMove = convertGridUnitsToSteps(gridUnits) - offset;

//				var steps = Math.Abs(distanceToMove) - estimatedExtraSteps;
//				if (steps < 0)
//				{
//					steps = Math.Abs(distanceToMove) / 2;
//				}
//				// Adjust the number of steps if it is very close to the estimated number of Extra steps
//				//if (steps < estimatedExtraSteps / 2)
//				//	steps = Math.Abs(distanceToMove) / 2;
//				var newMotorState = distanceToMove > 0 ? MoveDirection.Forward : MoveDirection.Backward;
//				System.Diagnostics.Debug.WriteLine($"Moving the {axis} axis {steps} steps {newMotorState} from position {StepPosition}.");
//				stepCounter.CountToPosition(steps, getMoveTimeout(steps));
//				currentMovePolarity = distanceToMove > 0 ? 1 : -1;
//				motorDrv.Direction = newMotorState;
//			}
//		}

//		private void moveInSteps(int steps)
//		{
//			if (steps == 0)
//			{
//				System.Diagnostics.Debug.WriteLine($"Stopping the {axis} axis.");
//				motorDrv.Direction = MoveDirection.Stopped;
//				stepCounter.CountToPosition(0, TimeSpan.FromMilliseconds(100));
//			}
//			else
//			{
//				var newMotorState = steps > 0 ? MoveDirection.Forward : MoveDirection.Backward;
//				currentMovePolarity = steps > 0 ? 1 : -1;
//				System.Diagnostics.Debug.WriteLine($"Moving the {axis} axis {steps} steps {newMotorState} from position {StepPosition}.");
//				stepCounter.CountToPosition(Math.Abs(steps), getMoveTimeout(steps));
//				motorDrv.Direction = newMotorState;
//			}
//		}

//		private TimeSpan getMoveTimeout(int steps)
//		{
//			return TimeSpan.FromMilliseconds(Math.Abs(steps) * millisecondsPerStep * 2 + 100);
//		}

//		private void updateCalibrationSettings()
//		{
//			System.Diagnostics.Debug.WriteLine($"Measured interrupt positions:\n\ttop: {lowerTopPosition}\t{upperTopPosition}\n\tbottom: {lowerBottomPosition}\t{upperBottomPosition}");
//			if (!upperTopPosition.HasValue || !lowerTopPosition.HasValue || !upperBottomPosition.HasValue || !lowerBottomPosition.HasValue)
//			{
//				throw new CalibrationException("UpdateCalibrationSettings called with at least one null interrupter position.");
//			}
//			float topPos = (float)(upperTopPosition.Value + lowerTopPosition.Value) / 2;
//			float bottomPos = (float)(upperBottomPosition.Value + lowerBottomPosition.Value) / 2;

//			var differenceInGridUnits = topInterrupt.GridPosition - bottomInterrupt.GridPosition;
//			StepsPerGridUnit = (topPos - bottomPos) / differenceInGridUnits;

//			var expectedTopPos = topInterrupt.GridPosition * StepsPerGridUnit;
//			int offset = (int)Math.Round(topPos - expectedTopPos);

//			System.Diagnostics.Debug.WriteLine($"Subtracting {offset} from StepPosition {StepPosition}.");
//			stepPosition -= offset;
//			upperTopPosition -= offset;
//			lowerTopPosition -= offset;
//			upperBottomPosition -= offset;
//			lowerBottomPosition -= offset;
//			gridPosition = convertStepsToGridUnits(StepPosition);
//			System.Diagnostics.Debug.WriteLine($"{axis} axis updated stepPosition to {StepPosition} with GridPosition {GridPosition}, and stepsPerGridUnit {StepsPerGridUnit}.");
//		}

//		private int convertGridUnitsToSteps(int gridUnits)
//		{
//			return (int)Math.Round(gridUnits * StepsPerGridUnit);
//		}

//		private int convertStepsToGridUnits(int steps)
//		{
//			return (int)Math.Round((float)steps / StepsPerGridUnit);
//		}

//		private void finishedCounting(object sender, PositionChangedEventArgs args)
//		{
//			// Stop the motor
//			motorDrv.Direction = MoveDirection.Stopped;
//			System.Diagnostics.Debug.WriteLine($"{axis} axis reached target position {args.Position} in state {state}");
//		}

//		private void additionalStepsCounted(object sender, PositionChangedEventArgs args)
//		{
//			System.Diagnostics.Debug.WriteLine($"{axis} axis reached {args.Position} after stopping in state {state}");
//			handleMoveEnd();
//		}

//		private void moveTimedOut(object sender, PositionChangedEventArgs args)
//		{
//			// Stop the motor
//			motorDrv.Direction = MoveDirection.Stopped;
//			System.Diagnostics.Debug.WriteLine($"Move timed out with motor in position {args.Position} in the {axis} axis in state {state}.");
//			handleMoveEnd();
//		}

//		private void handleMoveEnd()
//		{
//			if (state == MoveState.HomingForwards)
//			{
//				System.Diagnostics.Debug.WriteLine($"{axis} axis didn't get interrupted while HomingForwards, so is turning around.");
//				// Didn't find the home location going one way, try the other way.
//				state = MoveState.Stopped;
//				moveAsync(gridMin - gridMax).Wait();
//				state = MoveState.HomingBackwards;
//				return;
//			}
//			else if (state == MoveState.HomingBackwards)
//			{
//				System.Diagnostics.Debug.WriteLine($"ERROR: {axis} axis never encountered interrupts while homing.");
//				isCalibrating = false;
//			}
//			else if (state == MoveState.PreparingToCalibrate)
//			{
//				System.Diagnostics.Debug.WriteLine($"{axis} axis finished PreparingToCalibrate.");
//				// Ready to turn around to calibrate
//				// Turn around and go "calibrationSteps" number of steps
//				moveInSteps(CALIBRATION_STEPS * currentMovePolarity * -1);
//				state = MoveState.Calibrating;
//				return;
//			}
//			else if (state == MoveState.Calibrating)
//			{
//				System.Diagnostics.Debug.WriteLine($"{axis} axis finished Calibrating moves.");
//				// Done calibration
//				isCalibrating = false;
//			}

//			state = MoveState.Stopped;
//		}

//		private void topEdgeDetected(object sender, GpioValueChangedEventArgs edgeDetectArgs)
//		{
//			var pos = stepCounter.Position*currentMovePolarity + StepPosition;
//			System.Diagnostics.Debug.WriteLine($"{axis} axis detected the top interrupter's {edgeDetectArgs.Edge} during {state}.");
//			switch (state)
//			{
//				case MoveState.HomingForwards:
//					if (edgeDetectArgs.Edge == GpioEdge.RisingEdge)
//					{
//						// Passed the upper top edge, time to turn around and calibrate
//						// Stop the motor
//						System.Diagnostics.Debug.WriteLine($"Preparing to perform the Calibrate move for the {axis} axis.");
//						moveAsync(0).Wait();
//						state = MoveState.PreparingToCalibrate;
//					}
//					break;
//				case MoveState.HomingBackwards:
//					// Reached the upper part of the top interrupt, enter calibration without stopping
//					if (edgeDetectArgs.Edge == GpioEdge.FallingEdge)
//					{
//						upperTopPosition = pos;
//						System.Diagnostics.Debug.WriteLine($"Performing the Calibrate move for the {axis} axis.");
//						state = MoveState.Calibrating;
//						moveInSteps(CALIBRATION_STEPS * currentMovePolarity);
//					}
//					break;
//				case MoveState.Calibrating:
//					// Update position
//					if (edgeDetectArgs.Edge == GpioEdge.FallingEdge)
//					{
//						if (currentMovePolarity > 0)
//						{
//							lowerTopPosition = pos;
//						}
//						else
//						{
//							upperTopPosition = pos;
//						}
//					}
//					// Update position and finish calibrating if done
//					else
//					{
//						if (currentMovePolarity < 0)
//						{
//							lowerTopPosition = pos;
//						}
//						else
//						{
//							upperTopPosition = pos;
//							// Done calibrating, time to stop
//							System.Diagnostics.Debug.WriteLine($"{axis} axis done calibration move, stopping motor.");
//							moveAsync(0).Wait();
//						}
//					}
//					break;
//				case MoveState.Moving:
//					if (edgeDetectArgs.Edge == GpioEdge.FallingEdge)
//					{
//						if (currentMovePolarity > 0)
//						{
//							checkInterruptLocation(lowerTopPosition.Value, pos);
//						}
//						else
//						{
//							checkInterruptLocation(upperTopPosition.Value, pos);
//						}
//					}
//					else
//					{
//						if (currentMovePolarity > 0)
//						{
//							checkInterruptLocation(upperTopPosition.Value, pos);
//						}
//						else
//						{
//							checkInterruptLocation(lowerTopPosition.Value, pos);
//						}
//					}
//					break;
//			}
//		}

//		private void bottomEdgeDetected(object sender, GpioValueChangedEventArgs edgeDetectArgs)
//		{
//			var pos = stepCounter.Position * currentMovePolarity + StepPosition;
//			System.Diagnostics.Debug.WriteLine($"{axis} axis detected the bottom interrupter's {edgeDetectArgs.Edge} during {state}.");
//			switch (state)
//			{
				
//				case MoveState.HomingForwards:
//					// Reached the upper part of the top interrupt, enter calibration without stopping
//					if (edgeDetectArgs.Edge == GpioEdge.FallingEdge)
//					{
//						lowerBottomPosition = pos;
//						state = MoveState.Calibrating;
//						System.Diagnostics.Debug.WriteLine($"Performing the Calibrate move for the {axis} axis.");
//						moveInSteps(CALIBRATION_STEPS * currentMovePolarity);
//					}
//					break;
//				case MoveState.HomingBackwards:
//					if (edgeDetectArgs.Edge == GpioEdge.RisingEdge)
//					{
//						// Passed the upper top edge, time to turn around and calibrate
//						// Stop the motor
//						System.Diagnostics.Debug.WriteLine($"Preparing to perform the Calibrate move for the {axis} axis.");
//						moveAsync(0).Wait();
//						state = MoveState.PreparingToCalibrate;
//					}
//					break;
//				case MoveState.Calibrating:
//					// Update position
//					if (edgeDetectArgs.Edge == GpioEdge.FallingEdge)
//					{
//						if (currentMovePolarity > 0)
//						{
//							lowerBottomPosition = pos;
//						}
//						else
//						{
//							upperBottomPosition = pos;
//						}
//					}
//					// Update position and finish calibrating if done
//					else
//					{
//						if (currentMovePolarity < 0)
//						{
//							lowerBottomPosition = pos;
//							// Done calibrating, time to stop
//							System.Diagnostics.Debug.WriteLine($"{axis} axis done calibration move, stopping motor.");
//							moveAsync(0).Wait();
//						}
//						else
//						{
//							upperBottomPosition = pos;
//						}
//					}
//					break;
//				case MoveState.Moving:
//					if (edgeDetectArgs.Edge == GpioEdge.FallingEdge)
//					{
//						if (currentMovePolarity > 0)
//						{
//							checkInterruptLocation(lowerBottomPosition.Value, pos);
//						}
//						else
//						{
//							checkInterruptLocation(upperBottomPosition.Value, pos);
//						}
//					}
//					else
//					{
//						if (currentMovePolarity > 0)
//						{
//							checkInterruptLocation(upperBottomPosition.Value, pos);
//						}
//						else
//						{
//							checkInterruptLocation(lowerBottomPosition.Value, pos);
//						}
//					}
//					break;
//			}
//		}

//		private void checkInterruptLocation(int expectedPos, int measuredPos)
//		{
//			System.Diagnostics.Debug.WriteLine($"{axis} axis passed interrupter with measured position {measuredPos} and expected position {expectedPos}.");
//			if (Math.Abs(measuredPos - expectedPos) > INTERRUPT_TOLERANCE)
//			{
//				System.Diagnostics.Debug.WriteLine("Interrupt position is outside the normal bounds!!");
//			}
//		}
		
//		private int gridMax;
//		private int gridMin;
//		private int estimatedExtraSteps;
//		private int currentMovePolarity;
//		private readonly int millisecondsPerStep;
//		private volatile int stepPosition;
//		private volatile int gridPosition;

//		private const int INTERRUPT_TOLERANCE = 75;
//		private const int CALIBRATION_STEPS = 650;

//		private bool isCalibrating;
//		private int? upperTopPosition;
//		private int? lowerTopPosition;
//		private int? upperBottomPosition;
//		private int? lowerBottomPosition;

//		private readonly Axis axis;
//		private volatile MoveState state;

//		private IMotorDrv motorDrv;
//		private IPositionSignaler stepCounter;

//		private IPhotoInterrupter topInterrupt;
//		private IPhotoInterrupter bottomInterrupt;
//	}
//}
