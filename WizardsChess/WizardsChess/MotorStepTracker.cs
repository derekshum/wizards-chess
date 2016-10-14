using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Gpio;

namespace WizardsChess
{
	public class MotorStepTracker : IDisposable
	{
		/// <summary>
		/// Track the steps read at the specified input pin. Position is updated on the falling edge.
		/// </summary>
		/// <param name="pinNum">The GPIO pin to read steps from.</param>
		public MotorStepTracker(int pinNum)
		{
			var gpio = GpioController.GetDefault();
			pin = gpio.OpenPin(pinNum);
			pin.SetDriveMode(GpioPinDriveMode.InputPullDown);
			Position = 0;
			pin.ValueChanged += countStep;
		}

		public int Position { get; private set; }

		private void countStep(GpioPin p, GpioPinValueChangedEventArgs args)
		{
			if (args.Edge == GpioPinEdge.FallingEdge)
			{
				Position++;
			}
		}

		private GpioPin pin;

		#region IDisposable Support
		private bool disposedValue = false; // To detect redundant calls

		protected virtual void Dispose(bool disposing)
		{
			if (!disposedValue)
			{
				if (disposing)
				{
					pin.ValueChanged -= countStep;
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
