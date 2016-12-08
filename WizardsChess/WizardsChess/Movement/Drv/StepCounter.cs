﻿using System;
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
		public event StepEventHandler MoveTimedOut;

		/// <summary>
		/// Count the steps read at the specified input pin. Position is updated on the falling edge.
		/// </summary>
		/// <param name="pinNum">The GPIO pin to read steps from.</param>
		public StepCounter(int pinNum, int clearPinNum)
		{
			lockObject = new object();

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

		public void CountSteps(int numSteps, TimeSpan timeout)
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
						if (numSteps == 0)
						{
							targetNumSteps = stepsSoFar;
							state = CounterState.WaitingForExtraSteps;
							onTargetReached(numSteps);
							return;
						}
						targetNumSteps = stepsSoFar + numSteps;
						break;
					case CounterState.Ready:
						Position = 0;
						startPosition = Position;
						targetNumSteps = numSteps;
						break;
				}
				timeoutCallback = Task.Delay(timeout);
				timeoutCallback.ContinueWith((prev) =>
					{
						if (prev == this.timeoutCallback)
						{
							onMoveTimeOut();
						}
				});
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

		private void onTargetReached()
		{
			int numSteps;
			lock (lockObject)
			{
				numSteps = targetNumSteps;
				state = CounterState.WaitingForExtraSteps;
			}
			onTargetReached(numSteps);
		}

		private void onTargetReached(int numSteps)
		{
			FinishedCounting?.Invoke(this, new Events.StepEventArgs(numSteps));
		}

		private void onAdditionalStepsCounted()
		{
			if (isAdditionalStepsCanceled)
			{
				isAdditionalStepsCanceled = false;
				return;
			}
			int numSteps = 0;
			lock (lockObject)
			{
				numSteps = Position - (startPosition + targetNumSteps);
				state = CounterState.Ready;
			}
			AdditionalStepsCounted?.Invoke(this, new Events.StepEventArgs(numSteps));
		}

		private void onMoveTimeOut()
		{
			int numSteps = 0;
			lock(lockObject)
			{
				if (state == CounterState.Counting)
				{
					state = CounterState.Ready;
					numSteps = Position - startPosition;
				}
				else
				{
					return;
				}
			}

			MoveTimedOut?.Invoke(this, new Events.StepEventArgs(numSteps));
		}

		private CounterState state;
		private GpioPin pin;
		private GpioPin clearPin;
		private int startPosition;
		private int targetNumSteps;
		private Task additionalStepsCallback;
		private Task timeoutCallback;
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