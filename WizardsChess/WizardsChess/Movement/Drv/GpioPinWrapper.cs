using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Gpio;
using WizardsChess.Movement.Drv.Events;

namespace WizardsChess.Movement.Drv
{
	class GpioPinWrapper : IGpioPin, IDisposable
	{
		public event GpioValueChangedEventHandler ValueChanged;
		public int PinNum { get; }

		public GpioPinWrapper(int pinNum, GpioPinDriveMode mode, GpioPinValue value=GpioPinValue.Low)
		{
			var gpio = GpioController.GetDefault();
			this.PinNum = pinNum;
			pin = gpio.OpenPin(pinNum);
			pin.Write(value);
			pin.SetDriveMode(mode);

			pin.ValueChanged += realPinValueChanged;
		}

		public GpioValue Read()
		{
			return convertWindowsValueToCustomValue(pin.Read());
		}

		public void Write(GpioValue val)
		{
			pin.Write(convertCustomValueToWindowsValue(val));
		}

		private void realPinValueChanged(GpioPin sender, GpioPinValueChangedEventArgs e)
		{
			GpioValueChangedEventArgs edgeArgs = new GpioValueChangedEventArgs(GpioEdge.FallingEdge);
			if (e.Edge == GpioPinEdge.RisingEdge)
			{
				edgeArgs = new GpioValueChangedEventArgs(GpioEdge.RisingEdge);
			}
			ValueChanged?.Invoke(this, edgeArgs);
		}

		private GpioPin pin;

		private GpioValue convertWindowsValueToCustomValue(GpioPinValue windowsVal)
		{
			if (windowsVal == GpioPinValue.High)
			{
				return GpioValue.High;
			}
			else
			{
				return GpioValue.Low;
			}
		}

		private GpioPinValue convertCustomValueToWindowsValue(GpioValue customVal)
		{
			if (customVal == GpioValue.High)
			{
				return GpioPinValue.High;
			}
			else
			{
				return GpioPinValue.Low;
			}
		}

		#region IDisposable Support
		private bool disposedValue = false; // To detect redundant calls

		protected virtual void Dispose(bool disposing)
		{
			if (!disposedValue)
			{
				if (disposing)
				{
					pin.ValueChanged -= realPinValueChanged;
				}

				disposedValue = true;
			}
		}

		// This code added to correctly implement the disposable pattern.
		public void Dispose()
		{
			// Do not change this code. Put cleanup code in Dispose(bool disposing) above.
			Dispose(true);
		}
		#endregion
	}
}
