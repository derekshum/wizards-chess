using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WizardsChess.Movement.Drv;
using WizardsChess.Movement.Drv.Events;

namespace WizardsChess.Movement
{
	public class MotorCalibrator : IMotorCalibrator, IDisposable
	{
		public MotorCalibrator(int gridMinValue, int gridMaxValue
			, IMotorMover motorMover, IMotorLocator motorLocator, IMotorInformation motorInformation
			, IPhotoInterrupter topInterupter, IPhotoInterrupter bottomInterrupter)
		{
			gridMin = gridMinValue;
			gridMax = gridMaxValue;

			locator = motorLocator;
			mover = motorMover;
			motorInfo = motorInformation;
			top = topInterupter;
			bottom = bottomInterrupter;

			top.ValueChanged += topInterruptDetected;
			bottom.ValueChanged += bottomInterruptDetected;

			// Estimate the steps per grid unit
			StepsPerGridUnit = (float)(top.StepPosition - bottom.StepPosition) / (top.GridPosition - bottom.GridPosition);
			State = CalibrationState.NeedsCalibrating;
		}

		public float StepsPerGridUnit { get; private set; }
		public CalibrationState State { get; private set; }

		public async Task CalibrateAsync()
		{
			State = CalibrationState.PreparingToCalibrate;
			clearUpperAndLowerPos();

			var maxMoveDistance = convertGridUnitToSteps(gridMax - gridMin);
			var finalPos = await mover.GoToPositionAsync(locator.Position + maxMoveDistance / 2);

			if (hasAllPositionEstimates())
			{
				await calibrateFirstAndSecondPassAsync();
				return;
			}

			await mover.GoToPositionAsync(locator.Position - maxMoveDistance);

			await calibrateFirstAndSecondPassAsync();
		}

		private int gridMin;
		private int gridMax;

		private IMotorMover mover;
		private IMotorLocator locator;
		private IMotorInformation motorInfo;
		private IPhotoInterrupter top;
		private IPhotoInterrupter bottom;

		private int? upperTopPos;
		private int? lowerTopPos;
		private int? upperBottomPos;
		private int? lowerBottomPos;

		private int convertGridUnitToSteps(int gridUnit)
		{
			return (int)Math.Round(gridUnit * StepsPerGridUnit);
		}

		private int convertStepsToGridUnit(int steps)
		{
			return (int)Math.Round((float)steps / StepsPerGridUnit);
		}

		private void clearUpperAndLowerPos()
		{
			upperTopPos = null;
			lowerTopPos = null;
			upperBottomPos = null;
			lowerBottomPos = null;
		}

		private bool hasAllPositionEstimates()
		{
			return upperTopPos.HasValue && lowerTopPos.HasValue && upperBottomPos.HasValue && lowerBottomPos.HasValue;
		}

		private void updateStepsPerGridUnit()
		{
			StepsPerGridUnit = (float)(top.StepPosition - bottom.StepPosition) / (top.GridPosition - bottom.GridPosition);
		}

		private async Task calibrateFirstAndSecondPassAsync()
		{
			calibrateFirstPass();

			var distToMove = (top.StepPosition - bottom.StepPosition) + mover.EstimatedOvershoot * 2;
			if (locator.Position > top.StepPosition)
			{
				// Move down for second pass
				await mover.GoToPositionAsync(locator.Position - distToMove);
			}
			else
			{
				// Move up for second pass
				await mover.GoToPositionAsync(locator.Position + distToMove);
			}

			calibrateSecondPass();

			State = CalibrationState.Ready;
		}

		private void calibrateFirstPass()
		{ 
			var topPos = (upperTopPos.Value + lowerTopPos.Value) / 2;
			var bottomPos = (upperBottomPos.Value + lowerBottomPos.Value) / 2;
			var originPos = (topPos + bottomPos) / 2;

			locator.ShiftPosition(-originPos);
			top.StepPosition = topPos - originPos;
			bottom.StepPosition = bottomPos - originPos;
			updateStepsPerGridUnit();
		}

		private void calibrateSecondPass()
		{
			var topPos = (upperTopPos.Value + lowerTopPos.Value) / 2;
			var bottomPos = (upperBottomPos.Value + lowerBottomPos.Value) / 2;
			var originPos = (topPos + bottomPos) / 2;

			// Shift by half the offset to average with the first pass
			locator.ShiftPosition(-originPos / 2);
			top.StepPosition += (topPos - originPos);
			bottom.StepPosition += (bottomPos - originPos);
			top.StepPosition /= 2;
			bottom.StepPosition /= 2;
			updateStepsPerGridUnit();
		}

		private void topInterruptDetected(object sender, GpioValueChangedEventArgs e)
		{
			var pos = locator.Position;
			switch (State)
			{
				case CalibrationState.PreparingToCalibrate:
					// Check if can enter the Calibrating state
					if (e.Edge == GpioEdge.FallingEdge)
					{
						if (motorInfo.Direction == MoveDirection.Backward)
						{
							State = CalibrationState.Calibrating;
							upperTopPos = pos;
						}
					}
					else
					{
						if (motorInfo.Direction == MoveDirection.Forward)
						{
							mover.CancelMove();
						}
					}
					break;
				case CalibrationState.Calibrating:
					// Update nullable positions
					if (e.Edge == GpioEdge.FallingEdge)
					{
						if (motorInfo.Direction == MoveDirection.Backward)
						{
							upperTopPos = pos;
						}
						else
						{
							lowerTopPos = pos;
						}
					}
					else
					{
						if (motorInfo.Direction == MoveDirection.Forward)
						{
							upperTopPos = pos;
							mover.CancelMove();
						}
						else
						{
							lowerTopPos = pos;
						}
					}
					break;
				case CalibrationState.Ready:
					// Check we're still calibrated
					var offset = Math.Abs(pos - top.StepPosition);
					var tolerance = (upperTopPos.Value - lowerTopPos.Value) / 2 + 0.25 * StepsPerGridUnit;
					if (offset > tolerance)
					{
						System.Diagnostics.Debug.WriteLine($"Need to calibrate {motorInfo.Axis} motor.");
						State = CalibrationState.NeedsCalibrating;
					}
					break;
			}
		}

		private void bottomInterruptDetected(object sender, GpioValueChangedEventArgs e)
		{
			var pos = locator.Position;
			switch (State)
			{
				case CalibrationState.PreparingToCalibrate:
					// Check if can enter the Calibrating state
					if (e.Edge == GpioEdge.FallingEdge)
					{
						if (motorInfo.Direction == MoveDirection.Forward)
						{
							State = CalibrationState.Calibrating;
							lowerBottomPos = pos;
						}
					}
					else
					{
						if (motorInfo.Direction == MoveDirection.Backward)
						{
							mover.CancelMove();
						}
					}
					break;
				case CalibrationState.Calibrating:
					// Update nullable positions
					if (e.Edge == GpioEdge.FallingEdge)
					{
						if (motorInfo.Direction == MoveDirection.Forward)
						{
							lowerBottomPos = pos;
						}
						else
						{
							upperBottomPos = pos;
						}
					}
					else
					{
						if (motorInfo.Direction == MoveDirection.Backward)
						{
							lowerBottomPos = pos;
							mover.CancelMove();
						}
						else
						{
							upperBottomPos = pos;
						}
					}
					break;
				case CalibrationState.Ready:
					// Check we're still calibrated
					var offset = Math.Abs(pos - bottom.StepPosition);
					var tolerance = (upperBottomPos.Value - lowerBottomPos.Value) / 2 + 0.25 * StepsPerGridUnit;
					if (offset > tolerance)
					{
						System.Diagnostics.Debug.WriteLine($"Need to calibrate {motorInfo.Axis} motor.");
						State = CalibrationState.NeedsCalibrating;
					}
					break;
			}
		}

		#region IDisposable Support
		private bool disposedValue = false; // To detect redundant calls

		protected virtual void Dispose(bool disposing)
		{
			if (!disposedValue)
			{
				if (disposing)
				{
					top.ValueChanged -= topInterruptDetected;
					bottom.ValueChanged -= bottomInterruptDetected;
				}

				disposedValue = true;
			}
		}

		// This code added to correctly implement the disposable pattern.
		public void Dispose()
		{
			// Do not change this code. Put cleanup code in Dispose(bool disposing) above.
			Dispose(true);
		}
		#endregion
	}
}
