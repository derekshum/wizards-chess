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

		public Task MovePiece(List<Point2D> steps)
		{
			throw new NotImplementedException();
		}

		public void GoHome()
		{
			throw new NotImplementedException();
		}

		public void MoveMotor(Axis axis, int gridUnits)
		{
			switch (axis)
			{
				case Axis.X:
					xMover.Move(gridUnits);
					break;
				case Axis.Y:
					yMover.Move(gridUnits);
					break;
				default:
					System.Diagnostics.Debug.WriteLine("MovePerformer.MoveMotor() received an invalid axis.");
					break;
			}
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

		private CalibratedMotorMover xMover;
		private CalibratedMotorMover yMover;
		private IMagnetDrv magnet;
	}
}
