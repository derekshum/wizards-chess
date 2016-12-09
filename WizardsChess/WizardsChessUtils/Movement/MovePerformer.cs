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
		public MovePerformer(IMotorMover calXMover, IMotorMover calYMover, IMagnetDrv magnetDrv)
		{
			xMover = calXMover;
			yMover = calYMover;

			magnet = magnetDrv;
		}

		public async Task MovePieceAsync(IList<Point2D> steps)
		{
			var start = steps[0];
			steps.RemoveAt(0);

			System.Diagnostics.Debug.WriteLine($"MovePerformer sending move {start}");
			await xMover.GoToPositionAsync(start.X);
			await yMover.GoToPositionAsync(start.Y);

			magnet.TurnOn();

			foreach(var point in steps)
			{
				System.Diagnostics.Debug.WriteLine($"MovePerformer sending move {point}");
				await xMover.GoToPositionAsync(point.X);
				await yMover.GoToPositionAsync(point.Y);
			}

			magnet.TurnOff();
		}

		public async Task MoveMotorAsync(Axis axis, int gridUnits)
		{
			switch (axis)
			{
				//case Axis.X:
				//	await xMover.MoveAsync(gridUnits);
				//	break;
				//case Axis.Y:
				//	await yMover.MoveAsync(gridUnits);
				//	break;
				default:
					System.Diagnostics.Debug.WriteLine("MovePerformer.MoveMotor() received an invalid axis.");
					break;
			}
		}

		public async Task GoHomeAsync()
		{
			await xMover.GoToPositionAsync(0);
			await yMover.GoToPositionAsync(0);
		}

		public async Task CalibrateAsync()
		{
			//await xMover.CalibrateAsync();
			//await yMover.CalibrateAsync();
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

		private IMotorMover xMover;
		private IMotorMover yMover;
		private IMagnetDrv magnet;
	}
}
