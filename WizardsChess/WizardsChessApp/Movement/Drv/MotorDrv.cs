using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Gpio;

namespace WizardsChessApp.Movement.Drv
{
	public enum MotorState
	{
		Stopped,
		Forward,
		Backward
	}

	public sealed class MotorDrv
	{
		public MotorDrv(int forwardGpio, int backwardGpio)
		{
			var gpio = GpioController.GetDefault();
			forwardPin = gpio.OpenPin(forwardGpio);
			backwardPin = gpio.OpenPin(backwardGpio);
			ChangeState(MotorState.Stopped);
			forwardPin.SetDriveMode(GpioPinDriveMode.Output);
			backwardPin.SetDriveMode(GpioPinDriveMode.Output);
		}

		public void ChangeState(MotorState state)
		{
			if (state == State)
				return;

			switch(state)
			{
				case MotorState.Forward:
					backwardPin.Write(GpioPinValue.Low);
					forwardPin.Write(GpioPinValue.High);
					State = MotorState.Forward;
					break;
				case MotorState.Backward:
					forwardPin.Write(GpioPinValue.Low);
					backwardPin.Write(GpioPinValue.High);
					State = MotorState.Backward;
					break;
				case MotorState.Stopped:
				default:
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
