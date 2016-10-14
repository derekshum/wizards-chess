using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Gpio;

namespace WizardsChess
{
	public sealed class GpioToggler
	{
		public GpioToggler(TimeSpan period, int pinNum)
		{
			this.period = period;
			var gpio = GpioController.GetDefault();
			pin = gpio.OpenPin(pinNum);
		}

		public void Start()
		{
			pin.Write(GpioPinValue.High);
			currentPinVal = GpioPinValue.High;
			pin.SetDriveMode(GpioPinDriveMode.Output);

			toggleTask = Task.Run(async () => {
				while (true)
				{
					await Task.Delay(period);
					toggle();
				}
			});
		}
		
		private void toggle()
		{
			System.Diagnostics.Debug.WriteLine("Toggling");
			switch (currentPinVal)
			{
				case GpioPinValue.High:
					pin.Write(GpioPinValue.Low);
					currentPinVal = GpioPinValue.Low;
					break;
				case GpioPinValue.Low:
				default:
					pin.Write(GpioPinValue.High);
					currentPinVal = GpioPinValue.High;
					break;
			}
		}

		private TimeSpan period;
		private GpioPinValue currentPinVal;
		private GpioPin pin;
		private Task toggleTask;
	}
}
