using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WizardsChess.Movement.Exceptions;

namespace WizardsChess.Movement
{
	public class PreciseMotorMover : IPreciseMotorMover
	{
		public PreciseMotorMover(IMotorMover motorMover)
		{
			mover = motorMover;
		}

		public int Position { get { return mover.Locator.Position; } }

		public async Task GoToPositionAsync(int position)
		{
			// Don't check for calibration at the start to allow us to force it if necessary
			await goToPositionAsync(position);
			// Run again to adjust for overshoot
			await goToPositionAsync(position);
		}

		private IMotorMover mover;

		private async Task goToPositionAsync(int position)
		{
			if (isAtPosition(position))
			{
				// Cancel a move in case the motor is moving.
				mover.CancelMove();
				return;
			}

			var drivePosition = calcDrivePosition(position);
			await mover.GoToPositionAsync(drivePosition);
		}

		private bool isAtPosition(int position)
		{
			var offset = mover.Locator.Position - position;
			return Math.Abs(offset) < 5;
		}

		private int calcDrivePosition(int position)
		{
			var requiredOffset = position - mover.Locator.Position;

			if (Math.Abs(requiredOffset) > mover.EstimatedOvershoot * 2)
			{
				var overshoot = requiredOffset > 0 ? mover.EstimatedOvershoot : -mover.EstimatedOvershoot;
				return requiredOffset - overshoot + mover.Locator.Position;
			}

			return requiredOffset / 2 + mover.Locator.Position;
		}
	}
}
