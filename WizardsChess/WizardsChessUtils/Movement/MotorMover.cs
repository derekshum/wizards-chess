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

	public class MotorMover : IMotorMover
	{
		public MotorMover(IPositionSignaler posSignaler, IMotorLocator motorLocator, IMotorDrv motorDrv)
		{
			signaler = posSignaler;
			locator = motorLocator;
			motor = motorDrv;

			signaler.ReachedPosition += finishedCounting;
			motorDrv.Information.DirectionChanged += directionChanged;

			isMoving = false;
			state = MoverState.Ready;
		}

		public async Task<int> GoToPositionAsync(int targetPosition)
		{
			lock (lockObject)
			{
				if (state != MoverState.Ready)
				{
					System.Diagnostics.Debug.WriteLine($"{motor.Information.Axis} called GoToPositionAsync when in state {state}, resetting.");
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
			state = MoverState.Ready;
			signaler.CancelSignal();
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
				motor.Direction = MoveDirection.Stopped;
				isMoving = false;
				System.Diagnostics.Debug.WriteLine($"{motor.Information.Axis} motor has stopped.");
			}
		}
	}
}
