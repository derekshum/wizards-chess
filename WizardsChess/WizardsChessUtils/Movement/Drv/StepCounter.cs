﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WizardsChess.Movement.Drv.Events;
using WizardsChess.Movement.Events;

namespace WizardsChess.Movement.Drv
{
	enum CounterState
	{
		Ready,
		Counting,
		WaitingForExtraSteps
	}

	public class StepCounter : IDisposable, IStepCounter
	{
		public event PositionChangedEventHandler FinishedCounting;
		public event PositionChangedEventHandler AdditionalStepsCounted;
		public event PositionChangedEventHandler MoveTimedOut;

		/// <summary>
		/// Count the steps read at the specified input pin. Position is updated on the falling edge.
		/// </summary>
		/// <param name="countPin">The pin used for counting.</param>
		/// <param name="clearCounterPin">The pin used to clear the counter.</param>
		public StepCounter(IMotorLocator locator, IGpioPin clearCounterPin)
		{
			motorLocator = locator;
			clearPin = clearCounterPin;

			state = CounterState.Ready;
			isAdditionalStepsCanceled = false;

			motorLocator.PositionChanged += positionChanged;
		}

		public int Position { get; private set; }

		public void CountToPosition(int position, TimeSpan timeout)
		{
			targetPosition = position;
			switch (state)
			{
				case CounterState.WaitingForExtraSteps:
					isAdditionalStepsCanceled = true;
					break;
				case CounterState.Counting:
				case CounterState.Ready:
					if (targetPosition == motorLocator.Position)
					{
						onTargetReached(motorLocator.Position, motorLocator.LastMoveDirection);
						onAdditionalStepsCounted(motorLocator.Position, motorLocator.LastMoveDirection);
						return;
					}
					break;
			}
			timeoutCallback = Task.Delay(timeout);
			timeoutCallback.ContinueWith((prev) => {
				if (prev == this.timeoutCallback)
				{
					onMoveTimeOut();
				}
			});
			state = CounterState.Counting;
		}

		protected virtual void positionChanged(object locator, PositionChangedEventArgs args)
		{
			if (state == CounterState.Counting)
			{
				if (args.Position == targetPosition
					|| (args.Direction == MoveDirection.Forward && args.Position > targetPosition)
					|| (args.Direction == MoveDirection.Backward && args.Position < targetPosition))
				{
					state = CounterState.WaitingForExtraSteps;
					onTargetReached(args.Position, args.Direction);
					// Prepare to send extra steps here so it is guaranteed to send even if the motor stalls
					sendAdditionalSteps();
				}
			}
		}

		private void sendAdditionalSteps()
		{
			if (additionalStepsCallback == null || (int)additionalStepsCallback.Status > 4)
			{
				additionalStepsCallback = Task.Delay(800);
				additionalStepsCallback.ContinueWith((prev) => {
					onAdditionalStepsCounted(motorLocator.Position, motorLocator.LastMoveDirection);
				});
			}
		}

		private void onTargetReached(int position, MoveDirection direction)
		{
			FinishedCounting?.Invoke(this, new PositionChangedEventArgs(position, direction));
		}

		private void onAdditionalStepsCounted(int position, MoveDirection direction)
		{
			if (isAdditionalStepsCanceled)
			{
				isAdditionalStepsCanceled = false;
				return;
			}
			AdditionalStepsCounted?.Invoke(this, new PositionChangedEventArgs(position, direction));
		}

		private void onMoveTimeOut()
		{
			if (state != CounterState.Counting)
			{
				return;
			}

			MoveTimedOut?.Invoke(this, new PositionChangedEventArgs(motorLocator.Position, motorLocator.LastMoveDirection));
		}

		private volatile CounterState state;
		private IMotorLocator motorLocator;
		private IGpioPin clearPin;
		private int targetPosition;
		private Task additionalStepsCallback;
		private Task timeoutCallback;
		private bool isAdditionalStepsCanceled;

		#region IDisposable Support
		private bool disposedValue = false; // To detect redundant calls

		protected virtual void Dispose(bool disposing)
		{
			if (!disposedValue)
			{
				if (disposing)
				{
					motorLocator.PositionChanged -= positionChanged;
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