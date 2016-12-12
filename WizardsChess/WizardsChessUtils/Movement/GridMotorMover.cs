using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WizardsChess.Movement.Exceptions;

namespace WizardsChess.Movement
{
	public class GridMotorMover : IGridMotorMover
	{
		public GridMotorMover(IPreciseMotorMover preciseMover, IMotorCalibrator motorCalibrator)
		{
			mover = preciseMover;
			calibrator = motorCalibrator;
		}

		public int GridPosition { get; private set; }

		public async Task GoToPositionAsync(int gridPos)
		{
			// Update GridPositin first in case the move throws a CalibrationException afterwards
			GridPosition = gridPos;
			await mover.GoToPositionAsync(convertGridUnitsToStepPosition(gridPos));
			if (calibrator.State == CalibrationState.NeedsCalibrating)
			{
				throw new CalibrationException("Need to calibrate this motor after move.");
			}
		}

		public async Task CalibrateAsync()
		{
			await calibrator.CalibrateAsync();
		}

		private IPreciseMotorMover mover;
		private IMotorCalibrator calibrator;

		private int convertGridUnitsToStepPosition(int gridPos)
		{
			return (int)Math.Round((float)gridPos * calibrator.StepsPerGridUnit);
		}
	}
}
