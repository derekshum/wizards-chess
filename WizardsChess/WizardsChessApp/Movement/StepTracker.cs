using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Gpio;

namespace WizardsChess
{
	public class StepTracker : IDisposable, INotifyPropertyChanged
	{
		/// <summary>
		/// Track the steps read at the specified input pin. Position is updated on the falling edge.
		/// </summary>
		/// <param name="pinNum">The GPIO pin to read steps from.</param>
		public StepTracker(int pinNum)
		{
			var gpio = GpioController.GetDefault();
			pin = gpio.OpenPin(pinNum);
			pin.SetDriveMode(GpioPinDriveMode.InputPullDown);
			Position = 0;
			pin.ValueChanged += countStep;
			dispatcher = Windows.UI.Core.CoreWindow.GetForCurrentThread().Dispatcher;
		}

		public int Position { get { return position; }
			protected set
			{
				if (position != value)
				{
					position = value;
					if (position % 100 == 0)
					{
						dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Low, () => {
							NotifyPropertyChanged();
						});
					}
				}
			}
		}

		#region INotifyPropertyChanged
		public event PropertyChangedEventHandler PropertyChanged;

		private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
		#endregion

		protected virtual void countStep(GpioPin p, GpioPinValueChangedEventArgs args)
		{
			if (args.Edge == GpioPinEdge.FallingEdge)
			{
				Position++;
			}
		}

		private GpioPin pin;
		private int position;
		private Windows.UI.Core.CoreDispatcher dispatcher;

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
