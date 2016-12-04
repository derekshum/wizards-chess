using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WizardsChess.Movement.Drv;
using WizardsChess.Movement.Drv.Events;

namespace WizardsChess.Movement
{
	public class MovePerformer : IMovePerformer
	{
		public MovePerformer(CalibratedMotorMover calXMover, CalibratedMotorMover calYMover, IMagnetDrv magnetDrv)
		{
			xMover = calXMover;
			yMover = calYMover;

			magnet = magnetDrv;
		}

		public async Task MovePieceAsync(IList<Point2D> steps)
		{
			var start = steps[0];
			steps.RemoveAt(0);

			var startVector = convertAbsoluteToRelative(start);
			System.Diagnostics.Debug.WriteLine($"MovePerformer sending relative move {startVector}");
			await xMover.MoveAsync(startVector.X);
			await yMover.MoveAsync(startVector.Y);

			magnet.TurnOn();

			foreach(var point in steps)
			{
				var moveVector = convertAbsoluteToRelative(point);
				System.Diagnostics.Debug.WriteLine($"MovePerformer sending relative move {moveVector}");
				await xMover.MoveAsync(moveVector.X);
				await yMover.MoveAsync(moveVector.Y);
			}

			magnet.TurnOff();
		}

		public async Task MoveMotorAsync(Axis axis, int gridUnits)
		{
			switch (axis)
			{
				case Axis.X:
					await xMover.MoveAsync(gridUnits);
					break;
				case Axis.Y:
					await yMover.MoveAsync(gridUnits);
					break;
				default:
					System.Diagnostics.Debug.WriteLine("MovePerformer.MoveMotor() received an invalid axis.");
					break;
			}
		}

		public async Task GoHomeAsync()
		{
			await xMover.MoveAsync(-xMover.GridPosition);
			await yMover.MoveAsync(-yMover.GridPosition);
		}

		public async Task CalibrateAsync()
		{
			await xMover.CalibrateAsync();
			await yMover.CalibrateAsync();
			await GoHomeAsync();
		}

		public void EnableMagnet(bool enable)
		{
			if (enable)
			{
				magnet.TurnOn();
			}
			else
			{
				magnet.TurnOff();
			}
		}

		private Vector2D convertAbsoluteToRelative(Point2D absoluteEnd)
		{
			int xDiff = absoluteEnd.X - xMover.GridPosition;
			int yDiff = absoluteEnd.Y - yMover.GridPosition;
			return new Vector2D(xDiff, yDiff);
		}

		private CalibratedMotorMover xMover;
		private CalibratedMotorMover yMover;
		private IMagnetDrv magnet;
	}
}
