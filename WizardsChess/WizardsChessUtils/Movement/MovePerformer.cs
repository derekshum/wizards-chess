using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WizardsChess.Movement.Drv;
using WizardsChess.Movement.Drv.Events;

namespace WizardsChess.Movement
{
	public enum Axis
	{
		X,
		Y
	}

	public class MovePerformer : IMovePerformer
	{
		public MovePerformer(IMotorDrv motorx, IMotorDrv motory, IMagnetDrv magnetDrv
			, IStepCounter stepx, IStepCounter stepy)
		{
			motorX = motorx;
			motorY = motory;
			magnet = magnetDrv;
			stepX = stepx;
			stepY = stepy;

			stepX.AdditionalStepsCounted += additionalStepsCounted;
			stepY.AdditionalStepsCounted += additionalStepsCounted;
		}

		public Task MovePiece(IList<Point2D> steps)
		{
			throw new NotImplementedException();
		}

		public void MoveMotor(Axis axis, int steps)
		{
			var state = steps > 0 ? MotorState.Forward : MotorState.Backward;
			if (steps == 0)
			{
				state = MotorState.Stopped;
			}
			else
			{
				switch (axis)
				{
					case Axis.X:
						stepX.CountSteps(steps);
						break;
					case Axis.Y:
					default:
						stepY.CountSteps(steps);
						break;
				}
			}

			switch (axis)
			{
				case Axis.X:
					motorX.SetState(state);
					break;
				case Axis.Y:
				default:
					motorY.SetState(state);
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

		private void finishedCounting(object sender, StepEventArgs stepEventArgs)
		{
			if (sender == stepX)
			{
				motorX.SetState(MotorState.Stopped);
			}
			else
			{
				motorY.SetState(MotorState.Stopped);
			}
		}

		private void additionalStepsCounted(object sender, StepEventArgs stepEventArgs)
		{
			if (sender == stepX)
			{
				System.Diagnostics.Debug.WriteLine($"Counted {stepEventArgs.NumSteps} additional X steps");
			}
			else
			{
				System.Diagnostics.Debug.WriteLine($"Counted {stepEventArgs.NumSteps} additional Y steps");
			}
		}
		
		private IMotorDrv motorX;
		private IMotorDrv motorY;
		private IMagnetDrv magnet;
		private IStepCounter stepX;
		private IStepCounter stepY;
	}
}
