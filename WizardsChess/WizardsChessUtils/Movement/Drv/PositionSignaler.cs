using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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

	public class PositionSignaler : IDisposable, IPositionSignaler
	{
		public event PositionChangedEventHandler FinishedCounting;
		public event PositionChangedEventHandler AdditionalStepsCounted;
		public event PositionChangedEventHandler MoveTimedOut;

		/// <summary>
		/// Signal specified position updates and extra steps.
		/// </summary>
		/// <param name="locator">The IMotorLocator maintaining motor position.</param>
		/// <param name="clearCounterPin">The pin used to clear the counter.</param>
		public PositionSignaler(IMotorLocator locator, IGpioPin clearCounterPin)
		{
			motorLocator = locator;
			clearPin = clearCounterPin;

			state = CounterState.Ready;

			motorLocator.PositionChanged += positionChanged;

			additionalStepsCancellationSource = new CancellationTokenSource();
			timeoutCancellationSource = new CancellationTokenSource();
		}

		public int Position { get; private set; }

		public void CountToPosition(int position, TimeSpan timeout)
		{
			lock (lockObject)
			{
				targetPosition = position;
				switch (state)
				{
					case CounterState.WaitingForExtraSteps:
						additionalStepsCancellationSource.Cancel();
						break;
					case CounterState.Counting:
					case CounterState.Ready:
						if (targetPosition == motorLocator.Position)
						{
							onTargetReached();
							onAdditionalStepsCounted();
							return;
						}
						break;
				}

				timeoutCancellationSource.Cancel();
				timeoutCancellationSource = new CancellationTokenSource();
				var token = timeoutCancellationSource.Token;

				var startTime = DateTime.Now;

				Task.Run(async () => {
					var updatedTimeout = timeout - (DateTime.Now - startTime);
					if (updatedTimeout < TimeSpan.Zero)
					{
						if (!token.IsCancellationRequested)
						{
							onMoveTimeOut();
						}
					}
					await Task.Delay(updatedTimeout, token)
						.ContinueWith((prev) => {
							if (!token.IsCancellationRequested)
							{
								onMoveTimeOut();
							}
						}, token);
				}, token);

				state = CounterState.Counting;
			}
		}

		public void CancelSignal()
		{
			lock (lockObject)
			{
				additionalStepsCancellationSource.Cancel();
				timeoutCancellationSource.Cancel();
				state = CounterState.Ready;
			}
		}

		protected virtual void positionChanged(object locator, PositionChangedEventArgs args)
		{
			lock (lockObject)
			{
				if (state == CounterState.Counting)
				{
					if (args.Position == targetPosition
						|| (args.Direction == MoveDirection.Forward && args.Position > targetPosition)
						|| (args.Direction == MoveDirection.Backward && args.Position < targetPosition))
					{

						state = CounterState.WaitingForExtraSteps;
						onTargetReached();
						// Prepare to send extra steps here so it is guaranteed to send even if the motor stalls
						sendAdditionalSteps();
					}
				}
				else if (state == CounterState.WaitingForExtraSteps)
				{
					sendAdditionalSteps();
				}
			}
		}

		/// <summary>
		/// This function prepares a callback in 60ms, which will only run if no additional steps were counted in that time.
		/// When the callback runs, it fires the AdditionalStepsCounted event.
		/// </summary>
		private void sendAdditionalSteps()
		{
			additionalStepsCancellationSource.Cancel();
			additionalStepsCancellationSource = new CancellationTokenSource();
			var token = additionalStepsCancellationSource.Token;

			var startTime = DateTime.Now;

			Task.Run(async () => {
				var updatedTime = TimeSpan.FromMilliseconds(60) - (DateTime.Now - startTime);
				if (updatedTime < TimeSpan.Zero)
				{
					if (!token.IsCancellationRequested)
					{
						onAdditionalStepsCounted();
					}
				}
				await Task.Delay(60, token)
					.ContinueWith((prev) => {
						if (!token.IsCancellationRequested)
						{
							onAdditionalStepsCounted();
						}
					}, token);
			}, token);
		}

		private void onTargetReached()
		{
			FinishedCounting?.Invoke(this, new PositionChangedEventArgs(motorLocator.Position, motorLocator.LastMoveDirection));
		}

		private void onAdditionalStepsCounted()
		{
			AdditionalStepsCounted?.Invoke(this, new PositionChangedEventArgs(motorLocator.Position, motorLocator.LastMoveDirection));
			state = CounterState.Ready;
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
		private object lockObject = new object();
		private CancellationTokenSource additionalStepsCancellationSource;
		private CancellationTokenSource timeoutCancellationSource;

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