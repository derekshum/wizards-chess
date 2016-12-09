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
		Counting
	}

	public class PositionSignaler : IDisposable, IPositionSignaler
	{
		public event PositionChangedEventHandler FinishedCounting;
		public event PositionChangedEventHandler MoveTimedOut;

		/// <summary>
		/// Signal specified position updates and extra steps.
		/// </summary>
		/// <param name="locator">The IMotorLocator maintaining motor position.</param>
		/// <param name="clearCounterPin">The pin used to clear the counter.</param>
		public PositionSignaler(IMotorLocator locator)
		{
			state = CounterState.Ready;

			motorLocator = locator;
			motorLocator.PositionChanged += positionChanged;

			timeoutCancellationSource = new CancellationTokenSource();
		}

		public int Position { get; private set; }

		public void CountToPosition(int position, TimeSpan timeout)
		{
			lock (lockObject)
			{
				targetPosition = position;
				if (targetPosition == motorLocator.Position)
				{
					onTargetReached();
					return;
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
						onTargetReached();
					}
				}
			}
		}

		private void onTargetReached()
		{
			state = CounterState.Ready;
			FinishedCounting?.Invoke(this, new PositionChangedEventArgs(motorLocator.Position, motorLocator.LastMoveDirection));
		}

		private void onMoveTimeOut()
		{
			if (state != CounterState.Counting)
			{
				return;
			}

			state = CounterState.Ready;
			MoveTimedOut?.Invoke(this, new PositionChangedEventArgs(motorLocator.Position, motorLocator.LastMoveDirection));
		}

		private volatile CounterState state;
		private IMotorLocator motorLocator;
		private int targetPosition;
		private object lockObject = new object();
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