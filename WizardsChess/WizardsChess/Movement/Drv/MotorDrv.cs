using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Gpio;
using WizardsChess.Movement;

namespace WizardsChess.Movement.Drv
{
	class MotorDrv : IMotorDrv
	{
		public MotorDrv(int forwardGpio, int backwardGpio)
		{
			var gpio = GpioController.GetDefault();
			forwardPin = gpio.OpenPin(forwardGpio);
			backwardPin = gpio.OpenPin(backwardGpio);
			forwardPin.Write(GpioPinValue.Low);
			backwardPin.Write(GpioPinValue.Low);
			forwardPin.SetDriveMode(GpioPinDriveMode.Output);
			backwardPin.SetDriveMode(GpioPinDriveMode.Output);

			direction = MoveDirection.Stopped;
			previousDirection = direction;
		}

		public MoveDirection Direction
		{
			get
			{
				lock (lockObject)
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
				lock (lockObject)
				{
					previousDirection = direction;
					direction = value;
				}
				updatePins();
			}
		}

		public MoveDirection PreviousDirection
		{
			get
			{
				lock (lockObject)
				{
					return previousDirection;
				}
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

		private object lockObject = new object();
		private MoveDirection direction;
		private MoveDirection previousDirection;
		private GpioPin forwardPin;
		private GpioPin backwardPin;

		private void updatePins()
		{
			switch (direction)
			{
				case MoveDirection.Forward:
					System.Diagnostics.Debug.WriteLine("Moving the motor forwards");
					backwardPin.Write(GpioPinValue.Low);
					forwardPin.Write(GpioPinValue.High);
					break;
				case MoveDirection.Backward:
					System.Diagnostics.Debug.WriteLine("Moving the motor backwards");
					forwardPin.Write(GpioPinValue.Low);
					backwardPin.Write(GpioPinValue.High);
					break;
				case MoveDirection.Stopped:
				default:
					System.Diagnostics.Debug.WriteLine("Stopping the motor");
					forwardPin.Write(GpioPinValue.Low);
					backwardPin.Write(GpioPinValue.Low);
					break;
			}
		}
	}
}
