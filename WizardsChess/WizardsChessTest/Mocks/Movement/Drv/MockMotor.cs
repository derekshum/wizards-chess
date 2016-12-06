using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WizardsChess.Movement;
using WizardsChess.Movement.Drv;
using WizardsChess.Movement.Drv.Events;

namespace WizardsChessTest.Mocks.Movement.Drv
{
	class MockMotor : MockGpio, IMotorDrv
	{
		public MockMotor() : base()
		{
			direction = MoveDirection.Stopped;
			previousDirection = MoveDirection.Stopped;
			NumTicks = 0;
			timer = new Timer(motorTick, null, Timeout.Infinite, Timeout.Infinite);
		}

		public MoveDirection Direction
		{
			get
			{
				lock(lockObject)
				{
					return direction;
				}
			}
			set
			{
				if (direction == value)
				{
					return;
				}
				System.Diagnostics.Debug.WriteLine($"Setting the mock motor to {direction}.");
				lock (lockObject)
				{
					previousDirection = direction;
					direction = value;
				}
				handleMotorDirectionChanged();
			}
		}

		public MoveDirection PreviousDirection
		{
			get
			{
				lock(lockObject)
				{
					return previousDirection;
				}
			}
		}

		public int NumTicks
		{
			get; set;
		}

		public MoveDirection GetLatestActiveMoveDirection()
		{
			lock(lockObject)
			{
				if (direction != MoveDirection.Stopped)
				{
					return direction;
				}
				else
				{
					return previousDirection;
				}
			}
		}

		private void handleMotorDirectionChanged()
		{
			if (direction != MoveDirection.Stopped)
			{
				runTheMotor();
			}
			else
			{
				Task.Delay(200).ContinueWith((prev) => {
					timer.Change(Timeout.Infinite, Timeout.Infinite);
				});
			}
		}

		private MoveDirection direction;
		private MoveDirection previousDirection;
		private Timer timer;
		private object lockObject = new object();

		private void runTheMotor()
		{
			NumTicks = 0;
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
				NumTicks++;
				edge = GpioEdge.FallingEdge;
			}
		}
	}
}
