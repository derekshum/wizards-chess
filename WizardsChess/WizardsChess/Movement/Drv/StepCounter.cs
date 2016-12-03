using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Gpio;

namespace WizardsChess.Movement.Drv
{
	enum CounterState
	{
		Ready,
		Counting,
		WaitingForExtraSteps
	}

	class StepCounter : IDisposable, IStepCounter
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
			startPosition = 0;
			targetNumSteps = 0;

			state = CounterState.Ready;
			isAdditionalStepsCanceled = false;

			pin.ValueChanged += countStep;
		}

		public int Position { get; private set; }

		public void CountSteps(int numSteps)
		{
			lock (lockObject)
			{
				switch (state)
				{
					case CounterState.WaitingForExtraSteps:
						isAdditionalStepsCanceled = true;
						var extraSteps = Position - (startPosition + targetNumSteps);
						startPosition = Position - extraSteps;
						targetNumSteps = numSteps + extraSteps;
						break;
					case CounterState.Counting:
						var stepsSoFar = Position - startPosition;
						targetNumSteps = stepsSoFar + numSteps;
						break;
					case CounterState.Ready:
						startPosition = Position;
						targetNumSteps = numSteps;
						break;
				}
				state = CounterState.Counting;
			}
		}

		protected virtual void countStep(GpioPin p, GpioPinValueChangedEventArgs args)
		{
			if (args.Edge == GpioPinEdge.FallingEdge)
			{
				int numSteps;
				lock (lockObject)
				{
					Position++;
					numSteps = Position - startPosition;

					if (numSteps == targetNumSteps)
					{
						onTargetReached();
					}
				}

				if (numSteps > targetNumSteps)
				{
					sendAdditionalSteps();
				}
			}
		}

		private void onTargetReached()
		{
			FinishedCounting?.Invoke(this, new Events.StepEventArgs(targetNumSteps));
			state = CounterState.WaitingForExtraSteps;
		}

		private void onAdditionalStepsCounted()
		{
			if (isAdditionalStepsCanceled)
			{
				isAdditionalStepsCanceled = false;
				return;
			}
			lock (lockObject)
			{
				AdditionalStepsCounted?.Invoke(this, new Events.StepEventArgs(Position - (startPosition + targetNumSteps)));
				state = CounterState.Ready;
			}
		}

		private void sendAdditionalSteps()
		{
			if (additionalStepsCallback == null || (int)additionalStepsCallback.Status > 4)
			{
				additionalStepsCallback = Task.Delay(500);
				additionalStepsCallback.ContinueWith((prev) => {
					onAdditionalStepsCounted();
				});
			}
		}

		private CounterState state;
		private GpioPin pin;
		private GpioPin clearPin;
		private int startPosition;
		private int targetNumSteps;
		private Task additionalStepsCallback;
		private bool isAdditionalStepsCanceled;
		private Object lockObject;

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