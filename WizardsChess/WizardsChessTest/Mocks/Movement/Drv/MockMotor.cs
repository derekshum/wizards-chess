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
		private MockMotor() : base()
		{
			direction = MoveDirection.Stopped;
			previousDirection = MoveDirection.Stopped;
			NumTicks = 0;
			timer = new Timer(motorTick, null, Timeout.Infinite, Timeout.Infinite);
		}

		public static MockMotor Create()
		{
			var motor = new MockMotor();
			var info = new MotorInformation(Axis.X, motor);
			motor.Information = info;
			return motor;
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
				HandleMotorDirectionChanged(direction);
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

		public IMotorInformation Information
		{
			get { return information; }
			set
			{
				information = value as MotorInformation;
			}
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

		public void HandleMotorDirectionChanged(MoveDirection dir)
		{
			if (dir != MoveDirection.Stopped)
			{
				information.SetDirection(dir);
				runTheMotor();
			}
			else
			{
				Task.Delay(150).ContinueWith((prev) => {
					timer.Change(Timeout.Infinite, Timeout.Infinite);
				});
			}
		}

		private MoveDirection direction;
		private MoveDirection previousDirection;
		private MotorInformation information;
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
