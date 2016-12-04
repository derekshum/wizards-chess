using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WizardsChess.Movement.Drv;
using WizardsChess.Movement.Drv.Events;

namespace WizardsChessTest.Mocks.Movement.Drv
{
	class MockMotor : MockGpio, IMotorDrv
	{
		public MockMotor() : base()
		{
			State = MotorState.Stopped;
			Position = 0;
			NumTicks = 0;
			timer = new Timer(motorTick, null, Timeout.Infinite, Timeout.Infinite);
		}

		public MotorState State
		{
			get; private set;
		}

		public int Position
		{
			get;
			set;
		}

		public int NumTicks
		{
			get; set;
		}

		public void SetState(MotorState state)
		{
			if (State == state)
				return;

			System.Diagnostics.Debug.WriteLine($"Setting the mock motor to {state}.");

			State = state;
			if (state != MotorState.Stopped)
			{
				lastMoveState = state;
				runTheMotor();
			}
			else
			{
				Task.Delay(200).ContinueWith((prev) => {
					timer.Change(Timeout.Infinite, Timeout.Infinite);
				});
			}
		}

		private MotorState lastMoveState;
		private Timer timer;

		private void runTheMotor()
		{
			timer.Change(0, 1);
		}

		private void motorTick(object state)
		{
			if (edge == GpioEdge.FallingEdge)
			{
				edge = GpioEdge.RisingEdge;
			}
			else
			{
				if (lastMoveState == MotorState.Forward)
				{
					Position++;
				}
				else if (lastMoveState == MotorState.Backward)
				{
					Position--;
				}
				NumTicks++;
				edge = GpioEdge.FallingEdge;
			}
		}
	}
}
