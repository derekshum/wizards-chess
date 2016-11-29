using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Gpio;

namespace WizardsChess.Movement.Drv
{
	public class StepCounter : IDisposable, IStepCounter
	{
		public event StepEventHandler FinishedCounting;
		public event StepEventHandler AdditionalStepsCounted;

		/// <summary>
		/// Count the steps read at the specified input pin. Position is updated on the falling edge.
		/// </summary>
		/// <param name="pinNum">The GPIO pin to read steps from.</param>
		public StepCounter(int pinNum, int clearPinNum)
		{
			var gpio = GpioController.GetDefault();
			pin = gpio.OpenPin(pinNum);
			pin.SetDriveMode(GpioPinDriveMode.InputPullUp);
			clearPin = gpio.OpenPin(clearPinNum);
			clearPin.Write(GpioPinValue.Low);
			clearPin.SetDriveMode(GpioPinDriveMode.Output);
			Position = 0;
			targetPosition = 0;
			targetNumSteps = 0;

			pin.ValueChanged += countStep;
		}

		public int Position
		{
			get;
			set;
		}

		public void CountSteps(int numSteps)
		{
			if (additionalStepsCallback != null && (int)additionalStepsCallback.Status < 5)
			{
				additionalStepsCallback.ContinueWith((prev) => { this.CountSteps(numSteps); });
			}
			else
			{
				targetPosition = Position + numSteps;
				targetNumSteps = numSteps;
			}
		}

		protected virtual void countStep(GpioPin p, GpioPinValueChangedEventArgs args)
		{
			if (args.Edge == GpioPinEdge.FallingEdge)
			{
				Position++;
				if (Position == targetPosition)
				{
					onTargetReached();
				}
				else if (Position > targetPosition)
				{
					sendAdditionalSteps();
				}
			}
		}

		private void onTargetReached()
		{
			FinishedCounting?.Invoke(this, new Events.StepEventArgs(targetNumSteps));
		}

		private void onAdditionalStepsCounted()
		{
			AdditionalStepsCounted?.Invoke(this, new Events.StepEventArgs(Position - targetPosition));
		}

		private void sendAdditionalSteps()
		{
			if (additionalStepsCallback == null || (int)additionalStepsCallback.Status > 4)
			{
				additionalStepsCallback = Task.Delay(200);
				additionalStepsCallback.ContinueWith((prev) => {
					onAdditionalStepsCounted();
				});
			}
		}

		private GpioPin pin;
		private GpioPin clearPin;
		private int targetPosition;
		private int targetNumSteps;
		private Task additionalStepsCallback;

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