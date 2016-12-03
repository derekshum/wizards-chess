using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WizardsChess.Movement.Drv;
using WizardsChess.Movement.Drv.Events;

namespace WizardsChess.Movement
{
	enum MoveState
	{
		Stopped,
		Moving,
		HomingForwards,
		HomingBackwards,
		PreparingToCalibrate,
		Calibrating
	}
	
	public class CalibratedMotorMover
	{
		public CalibratedMotorMover(Axis axis, int gridMax, int gridMin, IMotorDrv mtrDrv, IStepCounter stepCntr,
			IPhotoInterrupter topPhotoInterrupt, IPhotoInterrupter bottomPhotoInterrupt)
		{
			stepPosition = 0;
			GridPosition = 0;
			estimatedExtraSteps = 100;
			stepsPerGridUnit = 225;

			this.axis = axis;
			state = MoveState.Stopped;
			isCalibrating = false;

			this.gridMax = gridMax;
			this.gridMin = gridMin;

			motorDrv = mtrDrv;
			stepCounter = stepCntr;
			topInterrupt = topPhotoInterrupt;
			bottomInterrupt = bottomPhotoInterrupt;

			stepCounter.FinishedCounting += finishedCounting;
			stepCounter.AdditionalStepsCounted += additionalStepsCounted;
			stepCounter.MoveTimedOut += moveTimedOut;
			topInterrupt.EdgeDetected += topEdgeDetected;
			bottomInterrupt.EdgeDetected += bottomEdgeDetected;
		}

		/// <summary>
		/// The current GridPosition. This is invalid if CalibratedMotorMove is in the middle of a move.
		/// </summary>
		public int GridPosition { get; private set; }

		/// <summary>
		/// Calibrates this motor. This moves the motor across the photointerrupters to determine the grid size and absolute position.
		/// </summary>
		/// <returns>An awaitable Task.</returns>
		public async Task CalibrateAsync()
		{
			if (state == MoveState.Moving)
			{
				System.Diagnostics.Debug.WriteLine("Started calibration during a move.");
			}
			System.Diagnostics.Debug.WriteLine($"Started {axis} calibration.");

			lowerBottomPosition = null;
			upperBottomPosition = null;
			lowerTopPosition = null;
			upperTopPosition = null;

			state = MoveState.HomingForwards;
			isCalibrating = true;

			// TODO: Optimize move direction based on current position
			// Look for the interrupter going forwards first
			await moveAsync((gridMax - gridMin) / 2);

			// Don't return until calibration is complete
			while(isCalibrating)
			{
				await Task.Delay(300);
			}

			updateCalibrationSettings();
		}

		/// <summary>
		/// Move this axis "gridUnits" away from its approximate current grid position.
		/// Limits the move to be within the expected grid size. Returns when the move is complete.
		/// </summary>
		/// <param name="gridUnits">The number of gridUnits to move. Can be positive or negative.</param>
		/// <returns>An awaitable Task.</returns>
		public async Task MoveAsync(int gridUnits)
		{
			if (GridPosition + gridUnits > gridMax)
			{
				gridUnits = gridMax - GridPosition;
			}
			else if (GridPosition + gridUnits < gridMin)
			{
				gridUnits = gridMin - GridPosition;
			}

			await moveAsync(gridUnits);

			state = MoveState.Moving;

			while (state != MoveState.Stopped)
			{
				await Task.Delay(100);
			}
		}

		private async Task moveAsync(int gridUnits)
		{
			if (gridUnits == 0)
			{
				motorDrv.SetState(MotorState.Stopped);
				stepCounter.CountSteps(0, TimeSpan.FromMilliseconds(100));
			}
			else
			{
				var newMotorState = gridUnits > 0 ? MotorState.Forward : MotorState.Backward;
				currentMovePolarity = gridUnits > 0 ? 1 : -1;
				while (state != MoveState.Stopped)
				{
					await Task.Delay(50);
				}

				var offset = stepPosition - GridPosition;
				var distanceToMove = convertGridUnitsToSteps(gridUnits) - offset;

				var steps = Math.Abs(distanceToMove) - estimatedExtraSteps;
				// Adjust the number of steps if it is very close to the estimated number of Extra steps
				if (steps < estimatedExtraSteps / 2)
					steps = Math.Abs(distanceToMove) / 2;
				stepCounter.CountSteps(steps, getMoveTimeout(steps));
				motorDrv.SetState(newMotorState);
			}
		}

		private void moveInSteps(int steps)
		{
			if (steps == 0)
			{
				motorDrv.SetState(MotorState.Stopped);
				stepCounter.CountSteps(0, TimeSpan.FromMilliseconds(100));
			}
			else
			{
				var newMotorState = steps > 0 ? MotorState.Forward : MotorState.Backward;
				currentMovePolarity = steps > 0 ? 1 : -1;
				stepCounter.CountSteps(steps, getMoveTimeout(steps));
				motorDrv.SetState(newMotorState);
			}
		}

		private TimeSpan getMoveTimeout(int steps)
		{
			return TimeSpan.FromMilliseconds(steps * 3);
		}

		private void updateCalibrationSettings()
		{
			float topPos = (float)(upperTopPosition.Value + lowerTopPosition.Value) / 2;
			float bottomPos = (float)(upperBottomPosition.Value + lowerBottomPosition.Value) / 2;

			var differenceInGridUnits = topInterrupt.GridPosition - bottomInterrupt.GridPosition;
			stepsPerGridUnit = (topPos - bottomPos) / differenceInGridUnits;

			var expectedTopPos = topInterrupt.GridPosition * stepsPerGridUnit;
			int offset = (int)Math.Round(topPos - expectedTopPos);

			stepPosition -= offset;
			GridPosition = convertStepsToGridUnits(stepPosition);
		}

		private int convertGridUnitsToSteps(int gridUnits)
		{
			return (int)Math.Round(gridUnits * stepsPerGridUnit);
		}

		private int convertStepsToGridUnits(int steps)
		{
			return (int)Math.Round((float)steps / stepsPerGridUnit);
		}

		private void finishedCounting(object sender, StepEventArgs stepEventArgs)
		{
			motorDrv.SetState(MotorState.Stopped);
			updatePosition(stepEventArgs.NumSteps);
		}

		private void additionalStepsCounted(object sender, StepEventArgs stepEventArgs)
		{
			System.Diagnostics.Debug.WriteLine($"Counted {stepEventArgs.NumSteps} additional steps in the {axis} axis");
			updatePosition(stepEventArgs.NumSteps);
			estimatedExtraSteps = stepEventArgs.NumSteps;

			handleMoveEnd();
		}

		private void moveTimedOut(object sender, StepEventArgs stepEventArgs)
		{
			motorDrv.SetState(MotorState.Stopped);
			System.Diagnostics.Debug.WriteLine($"Move timed out after {stepEventArgs.NumSteps} steps in the {axis} axis.");
			updatePosition(stepEventArgs.NumSteps);
			if (state == MoveState.Moving)
			{
				state = MoveState.Stopped;
				throw new Exception("Motors timed out during a regular move.");
			}
			else
			{
				// Probably in calibration, use regular move logic
				handleMoveEnd();
			}
		}

		private void handleMoveEnd()
		{
			if (state == MoveState.HomingForwards)
			{
				System.Diagnostics.Debug.WriteLine($"{axis} axis didn't get interrupted while HomingForwards, so is turning around.");
				// Didn't find the home location going one way, try the other way.
				state = MoveState.Stopped;
				moveAsync(gridMin - gridMax).Wait();
				state = MoveState.HomingBackwards;
			}
			else if (state == MoveState.HomingBackwards)
			{
				System.Diagnostics.Debug.WriteLine($"ERROR: {axis} axis never encountered interrupts while homing.");
				isCalibrating = false;
			}
			else if (state == MoveState.PreparingToCalibrate)
			{
				System.Diagnostics.Debug.WriteLine($"{axis} axis finished PreparingToCalibrate.");
				// Ready to turn around to calibrate
				calibrationStartPosition = stepPosition;
				// Turn around and go "calibrationSteps" number of steps
				moveInSteps(CALIBRATION_STEPS * currentMovePolarity * -1);
				state = MoveState.Calibrating;
				return;
			}
			else if (state == MoveState.Calibrating)
			{
				System.Diagnostics.Debug.WriteLine($"{axis} axis finished Calibrating moves.");
				// Done calibration
				isCalibrating = false;
			}

			state = MoveState.Stopped;
		}

		private void updatePosition(int numSteps)
		{
			stepPosition += numSteps * currentMovePolarity;
			GridPosition = convertStepsToGridUnits(stepPosition);
		}

		private void topEdgeDetected(object sender, InterruptEventArgs interruptEventArgs)
		{
			var pos = stepCounter.Position*currentMovePolarity + calibrationStartPosition;
			switch (state)
			{
				case MoveState.HomingForwards:
					if (interruptEventArgs.Edge == InterruptEdge.RisingEdge)
					{
						// Passed the upper top edge, time to turn around and calibrate
						// Stop the motor
						moveAsync(0).Wait();
						state = MoveState.PreparingToCalibrate;
					}
					break;
				case MoveState.HomingBackwards:
					// Reached the upper part of the top interrupt, enter calibration without stopping
					if (interruptEventArgs.Edge == InterruptEdge.FallingEdge)
					{
						upperTopPosition = pos;
						state = MoveState.Calibrating;
						moveInSteps(CALIBRATION_STEPS * currentMovePolarity);
					}
					break;
				case MoveState.Calibrating:
					// Update position
					if (interruptEventArgs.Edge == InterruptEdge.FallingEdge)
					{
						if (currentMovePolarity > 0)
						{
							lowerTopPosition = pos;
						}
						else
						{
							upperTopPosition = pos;
						}
					}
					// Update position and finish calibrating if done
					else
					{
						if (currentMovePolarity < 0)
						{
							lowerTopPosition = pos;
						}
						else
						{
							upperTopPosition = pos;
							// Done calibrating, time to stop
							moveAsync(0).Wait();
						}
					}
					break;
				case MoveState.Moving:
					if (interruptEventArgs.Edge == InterruptEdge.FallingEdge)
					{
						if (currentMovePolarity > 0)
						{
							checkInterruptLocation(lowerTopPosition.Value, pos);
						}
						else
						{
							checkInterruptLocation(upperTopPosition.Value, pos);
						}
					}
					else
					{
						if (currentMovePolarity > 0)
						{
							checkInterruptLocation(upperTopPosition.Value, pos);
						}
						else
						{
							checkInterruptLocation(lowerTopPosition.Value, pos);
						}
					}
					break;
			}
		}

		private void bottomEdgeDetected(object sender, InterruptEventArgs interruptEventArgs)
		{
			var pos = stepCounter.Position * currentMovePolarity + calibrationStartPosition;
			switch (state)
			{
				
				case MoveState.HomingForwards:
					// Reached the upper part of the top interrupt, enter calibration without stopping
					if (interruptEventArgs.Edge == InterruptEdge.FallingEdge)
					{
						upperTopPosition = pos;
						state = MoveState.Calibrating;
						moveInSteps(CALIBRATION_STEPS * currentMovePolarity);
					}
					break;
				case MoveState.HomingBackwards:
					if (interruptEventArgs.Edge == InterruptEdge.RisingEdge)
					{
						// Passed the upper top edge, time to turn around and calibrate
						// Stop the motor
						moveAsync(0).Wait();
						state = MoveState.PreparingToCalibrate;
					}
					break;
				case MoveState.Calibrating:
					// Update position
					if (interruptEventArgs.Edge == InterruptEdge.FallingEdge)
					{
						if (currentMovePolarity > 0)
						{
							lowerBottomPosition = pos;
						}
						else
						{
							upperBottomPosition = pos;
						}
					}
					// Update position and finish calibrating if done
					else
					{
						if (currentMovePolarity < 0)
						{
							lowerBottomPosition = pos;
							// Done calibrating, time to stop
							moveAsync(0).Wait();
						}
						else
						{
							upperBottomPosition = pos;
						}
					}
					break;
				case MoveState.Moving:
					if (interruptEventArgs.Edge == InterruptEdge.FallingEdge)
					{
						if (currentMovePolarity > 0)
						{
							checkInterruptLocation(lowerBottomPosition.Value, pos);
						}
						else
						{
							checkInterruptLocation(upperBottomPosition.Value, pos);
						}
					}
					else
					{
						if (currentMovePolarity > 0)
						{
							checkInterruptLocation(upperBottomPosition.Value, pos);
						}
						else
						{
							checkInterruptLocation(lowerBottomPosition.Value, pos);
						}
					}
					break;
			}
		}

		private void checkInterruptLocation(int expectedPos, int measuredPos)
		{
			System.Diagnostics.Debug.WriteLine($"{axis} axis passed interrupter with measured position {measuredPos} and expected position {expectedPos}.");
			if (Math.Abs(measuredPos - expectedPos) > INTERRUPT_TOLERANCE)
			{
				System.Diagnostics.Debug.WriteLine("Interrupt position is outside the normal bounds!!");
			}
		}
		
		private int stepPosition;
		private int gridMax;
		private int gridMin;
		private double stepsPerGridUnit;
		private int estimatedExtraSteps;
		private int currentMovePolarity;

		private const int INTERRUPT_TOLERANCE = 75;
		private const int CALIBRATION_STEPS = 650;

		private bool isCalibrating;
		private int calibrationStartPosition;
		private int? upperTopPosition;
		private int? lowerTopPosition;
		private int? upperBottomPosition;
		private int? lowerBottomPosition;

		private Axis axis;
		private MoveState state;

		private IMotorDrv motorDrv;
		private IStepCounter stepCounter;

		private IPhotoInterrupter topInterrupt;
		private IPhotoInterrupter bottomInterrupt;
	}
}
