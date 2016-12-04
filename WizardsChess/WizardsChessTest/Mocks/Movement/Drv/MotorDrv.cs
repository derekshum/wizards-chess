using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WizardsChess.Movement.Drv;
using WizardsChess.Movement.Drv.Events;

namespace WizardsChessTest.Mocks.Movement.Drv
{
	class MotorDrv : MockGpio, IMotorDrv
	{
		public event GpioValueChangedEventHandler ValueChanged;

		public MotorDrv() : base()
		{
			State = MotorState.Stopped;
			Position = 0;
		}

		public MotorState State
		{
			get; private set;
		}

		public int Position
		{
			get; set;
		}

		public void SetState(MotorState state)
		{
			if (State == state)
				return;

			State = state;
			if (state != MotorState.Stopped)
			{
				runTheMotor();
			}
		}

		private Task motorTask;

		private void runTheMotor()
		{
			//motorTask = Task.Run(async () => {
			//	await Task.Delay(1).ContinueWith((prev) => { motorTick(); });
			//});
			motorTask = Task.Delay(1).ContinueWith((prev) => { motorTick(); });
		}

		private void motorTick()
		{
			System.Diagnostics.Debug.WriteLine($"{DateTime.Now.Millisecond}");
			if (edge == GpioEdge.FallingEdge)
			{
				edge = GpioEdge.RisingEdge;
			}
			else
			{
				Position++;
				edge = GpioEdge.FallingEdge;
			}
			if (State != MotorState.Stopped)
			{
				runTheMotor();
			}
		}
	}
}
