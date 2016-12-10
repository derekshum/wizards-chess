using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WizardsChess.Movement.Drv;

namespace WizardsChess.Movement
{
	public class MotorCalibrator : IMotorCalibrator
	{
		public MotorCalibrator(int gridMinValue, int gridMaxValue
			, IMotorMover motorMover, IMotorLocator motorLocator
			, IPhotoInterrupter topInterupter, IPhotoInterrupter bottomInterrupter)
		{
			gridMin = gridMinValue;
			gridMax = gridMaxValue;

			motorMover = mover;
			top = topInterupter;
			bottom = bottomInterrupter;

			// Estimate the steps per grid unit
			StepsPerGridUnit = (float)(top.StepPosition - bottom.StepPosition) / (top.GridPosition - bottom.GridPosition);
			State = CalibrationState.NeedsCalibrating;
		}

		public float StepsPerGridUnit { get; private set; }
		public CalibrationState State { get; private set; }

		public async Task CalibrateAsync()
		{
			State = CalibrationState.NeedsCalibrating;
			clearUpperAndLowerPos();

			var maxMoveDistance = convertGridUnitToSteps(gridMax - gridMin);
			await mover.GoToPositionAsync(locator.Position + maxMoveDistance / 2);

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
		private IPhotoInterrupter top;
		private IPhotoInterrupter bottom;

		private Dictionary<MoveDirection, int> topPositionEst;
		private Dictionary<MoveDirection, int> bottomPositionEst;

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
		}

		private void calibrateFirstPass()
		{ 
			var topPos = (upperTopPos.Value + lowerTopPos.Value) / 2;
			var bottomPos = (upperTopPos.Value + lowerTopPos.Value) / 2;
			var originPos = (topPos + bottomPos) / 2;

			locator.ShiftPosition(-originPos);
			top.StepPosition = topPos - originPos;
			bottom.StepPosition = bottomPos - originPos;
			updateStepsPerGridUnit();
		}

		private void calibrateSecondPass()
		{
			var topPos = (upperTopPos.Value + lowerTopPos.Value) / 2;
			var bottomPos = (upperTopPos.Value + lowerTopPos.Value) / 2;
			var originPos = (topPos + bottomPos) / 2;

			// Shift by half the offset to average with the first pass
			locator.ShiftPosition(-originPos / 2);
			top.StepPosition += (topPos - originPos);
			bottom.StepPosition += (topPos - originPos);
			top.StepPosition /= 2;
			bottom.StepPosition /= 2;
			updateStepsPerGridUnit();
		}

		private void onTopInterrupt(object sender, GpioValueChangedEventHandler e)
		{

		}

		private void onBottomInterrupt(object sender, GpioValueChangedEventHandler e)
		{

		}
	}
}
