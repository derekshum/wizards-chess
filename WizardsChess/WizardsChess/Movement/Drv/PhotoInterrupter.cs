using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Gpio;
using WizardsChess.Movement.Drv.Events;

namespace WizardsChess.Movement.Drv
{
	class PhotoInterrupter : IPhotoInterrupter
	{
		public int GridPosition { get; }
		public event InterruptEventHandler EdgeDetected;

		public PhotoInterrupter(int pinNum, int gridPosition)
		{
			var gpio = GpioController.GetDefault();
			pin = gpio.OpenPin(pinNum);
			pin.SetDriveMode(GpioPinDriveMode.InputPullUp);

			GridPosition = gridPosition;

			pin.ValueChanged += detectEdge;
		}

		private void detectEdge(GpioPin p, GpioPinValueChangedEventArgs args)
		{
			switch (args.Edge)
			{
				case GpioPinEdge.FallingEdge:
					onEdgeDetected(InterruptEdge.FallingEdge);
					break;
				case GpioPinEdge.RisingEdge:
				default:
					onEdgeDetected(InterruptEdge.RisingEdge);
					break;
			}
		}

		private void onEdgeDetected(InterruptEdge edge)
		{
			EdgeDetected?.Invoke(this, new InterruptEventArgs(edge));
		}

		private GpioPin pin;
	}
}
