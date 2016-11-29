using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Gpio;

namespace WizardsChess.Movement.Drv
{
	class MagnetDrv : IMagnetDrv
	{
		public MagnetDrv(int pinNum)
		{
			var gpio = GpioController.GetDefault();
			pin = gpio.OpenPin(pinNum);
			pin.Write(GpioPinValue.Low);
			pin.SetDriveMode(GpioPinDriveMode.Output);
		}

		private GpioPin pin;

		public bool IsOn
		{
			get
			{
				return pin.Read() == GpioPinValue.High;
			}
		}

		public void TurnOn()
		{
			pin.Write(GpioPinValue.High);
		}

		public void TurnOff()
		{
			pin.Write(GpioPinValue.Low);
		}
	}
}
