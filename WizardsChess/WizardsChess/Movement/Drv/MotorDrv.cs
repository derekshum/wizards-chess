using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Gpio;

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
			
		}

		public void SetState(MotorState state)
		{
			if (state == State)
				return;

			switch(state)
			{
				case MotorState.Forward:
					System.Diagnostics.Debug.WriteLine("Moving the motor forwards");
					backwardPin.Write(GpioPinValue.Low);
					forwardPin.Write(GpioPinValue.High);
					State = MotorState.Forward;
					break;
				case MotorState.Backward:
					System.Diagnostics.Debug.WriteLine("Moving the motor backwards");
					forwardPin.Write(GpioPinValue.Low);
					backwardPin.Write(GpioPinValue.High);
					State = MotorState.Backward;
					break;
				case MotorState.Stopped:
				default:
					System.Diagnostics.Debug.WriteLine("Stopping the motor");
					forwardPin.Write(GpioPinValue.Low);
					backwardPin.Write(GpioPinValue.Low);
					State = MotorState.Stopped;
					break;
			}
		}

		public MotorState State { get; private set; }

		private GpioPin forwardPin;
		private GpioPin backwardPin;
	}
}
