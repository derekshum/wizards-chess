using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WizardsChess.Movement.Events;

namespace WizardsChess.Movement.Drv
{
	enum MoverState
	{
		Ready,
		PerformingMove,
		Adjusting
	}

	public class MotorMover : IMotorMover
	{
		public MotorMover(IPositionSignaler posSignaler, IMotorLocator motorLocator, IMotorDrv motorDrv)
		{
			signaler = posSignaler;
			locator = motorLocator;
			motor = motorDrv;

			signaler.FinishedCounting += finishedCounting;
			signaler.AdditionalStepsCounted += additionalStepsCounted;
			signaler.MoveTimedOut += moveTimedOut;

			isMoving = false;
			state = MoverState.Ready;
		}

		public async Task<int> GoToPositionAsync(int targetPosition)
		{
			lock (lockObject)
			{
				if (state != MoverState.Ready)
				{
					resetState();
				}
				state = MoverState.PerformingMove;
			}
			await goToPositionAsync(targetPosition);
			lock (lockObject)
			{
				state = MoverState.Ready;
			}
			return locator.Position;
		}

		public void CancelMove()
		{
			motor.Direction = MoveDirection.Stopped;

			lock (lockObject)
			{
				resetState();
			}
		}

		private volatile bool isMoving;
		private volatile MoverState state;
		private object lockObject = new object();

		private IPositionSignaler signaler;
		private IMotorLocator locator;
		private IMotorDrv motor;

		private void resetState()
		{
			signaler.CancelSignal();
			isMoving = false;
			state = MoverState.Ready;
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

			signaler.CountToPosition(position, TimeSpan.FromMilliseconds(position * 4));
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
			bool isMovingCopy;
			lock (lockObject)
			{
				isMovingCopy = isMoving;
			}
			while (isMovingCopy)
			{
				await Task.Delay(45);
				lock (lockObject)
				{
					isMovingCopy = isMoving;
				}
			}
		}

		private bool isAtPosition(int position)
		{
			return Math.Abs(position - locator.Position) < 5;
		}

		private bool isMoveCanceled()
		{
			return state == MoverState.Ready;
		}

		private void finishedCounting(object sender, PositionChangedEventArgs e)
		{
			motor.Direction = MoveDirection.Stopped;

			lock (lockObject)
			{
				if (isMoveCanceled())
				{
					return;
				}
			}
			System.Diagnostics.Debug.WriteLine("Finished counting.");
		}

		private void additionalStepsCounted(object sender, PositionChangedEventArgs e)
		{
			lock (lockObject)
			{
				if (isMoveCanceled())
				{
					return;
				}

				isMoving = false;
			}
			System.Diagnostics.Debug.WriteLine("Additional steps counted.");
		}

		private void moveTimedOut(object sender, PositionChangedEventArgs e)
		{
			motor.Direction = MoveDirection.Stopped;

			lock (lockObject)
			{
				if (isMoveCanceled())
				{
					return;
				}

				isMoving = false;
			}
			System.Diagnostics.Debug.WriteLine("Move timed out");
		}
	}
}
