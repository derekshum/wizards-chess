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

		public async Task GoToPositionAsync(int position)
		{
			lock (lockObject)
			{
				state = MoverState.PerformingMove;
			}
			await goToPositionAsync(position);
			lock (lockObject)
			{
				if (isMoveCanceled())
				{
					return;
				}
				state = MoverState.Adjusting;
			}
			await adjustIfNecessary(position);
			lock (lockObject)
			{
				state = MoverState.Ready;
			}
		}

		public void CancelMove()
		{
			lock (lockObject)
			{
				signaler.CancelSignal();
				isMoving = false;
				state = MoverState.Ready;
			}
		}

		private volatile bool isMoving;
		private volatile MoverState state;
		private object lockObject = new object();

		private IPositionSignaler signaler;
		private IMotorLocator locator;
		private IMotorDrv motor;

		private async Task goToPositionAsync(int position)
		{
			if (isAtPosition(position))
			{
				return;
			}

			// TODO: MOVE

			await waitForMoveToFinishAsync();
		}

		private async Task adjustIfNecessary(int position)
		{
			if (isAtPosition(position))
			{
				return;
			}

			// TODO: MOVE

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
			lock (lockObject)
			{
				if (isMoveCanceled())
					return;

				System.Diagnostics.Debug.WriteLine("Finished counting");
			}
		}

		private void additionalStepsCounted(object sender, PositionChangedEventArgs e)
		{
			lock (lockObject)
			{
				if (isMoveCanceled())
					return;

				System.Diagnostics.Debug.WriteLine("Additional steps");
			}
		}

		private void moveTimedOut(object sender, PositionChangedEventArgs e)
		{
			lock (lockObject)
			{
				if (isMoveCanceled())
					return;

				System.Diagnostics.Debug.WriteLine("Move timed out");
			}
		}
	}
}
