using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WizardsChess.Movement
{
	public class GridMotorMover : IGridMotorMover
	{
		public GridMotorMover(IPreciseMotorMover preciseMover)
		{
			mover = preciseMover;
		}

		public int GridPosition { get; private set; }

		public async Task GoToPositionAsync(int gridPos)
		{
			// Update GridPositin first in case the move throws a CalibrationException afterwards
			GridPosition = gridPos;
			await mover.GoToPositionAsync(convertGridUnitsToStepPosition(gridPos));
		}

		public async Task CalibrateAsync()
		{
			await mover.CalibrateAsync();
		}

		private IPreciseMotorMover mover;

		private int convertGridUnitsToStepPosition(int gridPos)
		{
			return (int)Math.Round((float)gridPos * mover.StepsPerGridUnit);
		}
	}
}
