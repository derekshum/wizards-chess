using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WizardsChess.Movement.Drv;
using WizardsChess.Movement.Drv.Events;
using WizardsChess.Movement.Events;

namespace WizardsChess.Movement
{
	enum MoverState
	{
		Ready,
		PerformingMove
	}

	public class MotorMover : IMotorMover, IDisposable
	{
		public MotorMover(IPositionSignaler posSignaler, IMotorLocator motorLocator, IMotorDrv motorDrv)
		{
			signaler = posSignaler;
			locator = motorLocator;
			motor = motorDrv;

			signaler.ReachedPosition += finishedCounting;
			motor.Information.DirectionChanged += directionChanged;

			isMoving = false;
			state = MoverState.Ready;
		}

		public int EstimatedOvershoot { get; private set; }

		public async Task<int> GoToPositionAsync(int position)
		{
			lock (lockObject)
			{
				if (state != MoverState.Ready)
				{
					System.Diagnostics.Debug.WriteLine($"{motor.Information.Axis} called GoToPositionAsync when in state {state}, resetting.");
					resetState();
				}
				state = MoverState.PerformingMove;
				targetPosition = position;
				shouldUpdateOvershoot = true;
			}
			await goToPositionAsync(position);
			lock (lockObject)
			{
				state = MoverState.Ready;
				if (shouldUpdateOvershoot)
				{
					updateEstimatedOvershoot(targetPosition);
				}
			}
			return locator.Position;
		}

		public void CancelMove()
		{
			motor.Direction = MoveDirection.Stopped;

			lock (lockObject)
			{
				resetState();
				shouldUpdateOvershoot = false;
			}
		}

		private int targetPosition;
		private volatile bool isMoving;
		private volatile bool shouldUpdateOvershoot;
		private volatile MoverState state;
		private object lockObject = new object();

		private IPositionSignaler signaler;
		private IMotorLocator locator;
		private IMotorDrv motor;

		private void resetState()
		{
			state = MoverState.Ready;
			signaler.CancelSignal();
		}

		private void updateEstimatedOvershoot(int targetPosition)
		{
			EstimatedOvershoot += Math.Abs(targetPosition - locator.Position);
			EstimatedOvershoot /= 2;
		}

		private async Task goToPositionAsync(int position)
		{
			if (isAtPosition(position))
			{
				return;
			}

			var offset = position - locator.Position;
			if (offset == 0)
			{
				// We are now somehow at the desired position. 
				// Shouldn't happen, but we can return if it does.
				return;
			}

			lock (lockObject)
			{
				isMoving = true;
			}
			signaler.SignalOnPosition(position);
			if (offset < 0)
			{
				motor.Direction = MoveDirection.Backward;
			}
			else
			{
				motor.Direction = MoveDirection.Forward;
			}

			await waitForMoveToFinishAsync();
		}

		private async Task waitForMoveToFinishAsync()
		{
			bool shouldWait;
			lock (lockObject)
			{
				shouldWait = isMoving;
			}
			while (shouldWait)
			{
				await Task.Delay(45);
				lock (lockObject)
				{
					shouldWait = isMoving;
				}
			}
		}

		private bool isAtPosition(int position)
		{
			return Math.Abs(position - locator.Position) < 5;
		}

		private void finishedCounting(object sender, PositionChangedEventArgs e)
		{
			motor.Direction = MoveDirection.Stopped;
			System.Diagnostics.Debug.WriteLine($"{motor.Information.Axis} motor finished counting.");
		}

		private void directionChanged(object sender, MotorDirectionChangedEventArgs e)
		{
			if (e.Direction == MoveDirection.Stopped)
			{
				if (motor.Direction != MoveDirection.Stopped)
				{
					// This is a stall!
					motor.Direction = MoveDirection.Stopped;
					shouldUpdateOvershoot = false;
					System.Diagnostics.Debug.WriteLine($"{motor.Information.Axis} motor stalled!");
				}
				isMoving = false;
				System.Diagnostics.Debug.WriteLine($"{motor.Information.Axis} motor has stopped.");
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
					signaler.ReachedPosition -= finishedCounting;
					motor.Information.DirectionChanged -= directionChanged;
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
