using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WizardsChess.Movement;
using WizardsChess.Movement.Drv;
using WizardsChessTest.Mocks.Movement.Drv;
using System.Threading.Tasks;

namespace WizardsChessTest.Mocks.Movement
{
	[TestClass]
	public class MotorMoverTest
	{
		[TestMethod]
		public void TestMotorMoverForwardMove()
		{
			constructMotorMover();

			int targetPos = 12;
			int finalPos = motorMover.GoToPositionAsync(targetPos).Result;
			Assert.AreEqual(motorLocator.Position, finalPos, $"Motor mover result {finalPos} does not match motorLocator position {motorLocator.Position}");
			Assert.IsTrue(finalPos > targetPos, $"Motor final position {finalPos} did not make sense with target {targetPos}.");
			Assert.IsTrue(motorMover.EstimatedOvershoot != 0, "Estimated overshoot was still 0 after a successful move.");
		}

		[TestMethod]
		public void TestMotorMoverForwardThenBackwardMove()
		{
			constructMotorMover();

			int targetPos = 12;
			int finalPos = motorMover.GoToPositionAsync(targetPos).Result;
			Assert.AreEqual(motorLocator.Position, finalPos, $"Motor mover result {finalPos} does not match motorLocator position {motorLocator.Position}");
			Assert.IsTrue(finalPos > targetPos, $"Motor final position {finalPos} did not make sense with target {targetPos}.");
			Assert.IsTrue(motorMover.EstimatedOvershoot != 0, "Estimated overshoot was still 0 after a successful move.");
			var previousOvershoot = motorMover.EstimatedOvershoot;

			targetPos = -12;
			finalPos = motorMover.GoToPositionAsync(targetPos).Result;
			Assert.AreEqual(motorLocator.Position, finalPos, $"Motor mover result {finalPos} does not match motorLocator position {motorLocator.Position}");
			Assert.IsTrue(finalPos < targetPos, $"Motor final position {finalPos} did not make sense with target {targetPos}.");
			Assert.IsTrue(motorMover.EstimatedOvershoot > previousOvershoot, "Estimated overshoot did not increase after a second successful move.");
		}

		[TestMethod]
		public void TestMotorMoverCancel()
		{
			constructMotorMover();

			int targetPos = 300;
			var moveTask = motorMover.GoToPositionAsync(targetPos);
			Task.Delay(30).Wait();
			motorMover.CancelMove();
			while(mockMotor.Information.Direction != MoveDirection.Stopped)
			{
				Task.Delay(30).Wait();
			}
			Task.Delay(45).Wait();
			Assert.IsTrue(moveTask.IsCompleted, "Move task did not end after cancellation.");
			int finalPos = moveTask.Result;
			Assert.AreEqual(motorLocator.Position, finalPos, $"Motor mover result {finalPos} does not match motorLocator position {motorLocator.Position}");
			Assert.IsTrue(finalPos < targetPos, $"Motor final position {finalPos} did not make sense with target {targetPos}.");
			Assert.AreEqual(0, motorMover.EstimatedOvershoot, "EstimatedOvershoot should not update if a move is canceled.");
		}

		[TestMethod]
		public void TestMotorStall()
		{
			constructMotorMover();

			int targetPos = 300;
			var moveTask = motorMover.GoToPositionAsync(targetPos);
			Task.Delay(60).Wait();
			mockMotor.HandleMotorDirectionChanged(MoveDirection.Stopped);
			while (mockMotor.Information.Direction != MoveDirection.Stopped)
			{
				Task.Delay(30).Wait();
			}
			Task.Delay(30).Wait();
			int finalPos = moveTask.Result;
			Assert.IsTrue(finalPos != targetPos, "Counted all the way to the target position instead of stalling.");
			Assert.AreEqual(motorLocator.Position, finalPos, "MotorLocator position did not match finalPos");
			Assert.AreEqual(MoveDirection.Stopped, mockMotor.Direction, "Motor was not stopped after move stalled.");
			Assert.AreEqual(0, motorMover.EstimatedOvershoot, "EstimatedOvershoot shouldn't be updated with a stall.");
		}

		[TestMethod]
		public void TestOverlappingMotorMoves()
		{
			constructMotorMover();

			int initialTargetPos = 300;
			var moveTask = motorMover.GoToPositionAsync(initialTargetPos);
			Task.Delay(60).Wait();
			int targetPos = motorLocator.Position + 10;
			var startTime = DateTime.Now;
			var secondMoveTask = motorMover.GoToPositionAsync(targetPos);
			int finalPos = secondMoveTask.Result;
			var timeDifference = DateTime.Now - startTime;
			Assert.IsTrue(timeDifference < TimeSpan.FromMilliseconds(700), "Interrupting move took longer than expected");
			Assert.IsTrue(moveTask.IsCompleted, "First move did not complete.");
			Task.Delay(15).Wait();
			Assert.AreEqual(motorLocator.Position, finalPos, "Motor locator did not match finalPos");
			Assert.IsTrue(motorLocator.Position < initialTargetPos, $"Motor went past or met the initial target position of {initialTargetPos} to {motorLocator.Position}/");
			Assert.IsTrue((finalPos - targetPos) > motorMover.EstimatedOvershoot, "Estimated offset was over its expected value.");
		}

		private IMotorMover motorMover;
		private IMotorLocator motorLocator;
		private IPositionSignaler positionSignaler;
		private MockMotor mockMotor;

		private void constructMotorMover()
		{
			mockMotor = MockMotor.Create();
			motorLocator = new MotorLocator(mockMotor, new MockGpio(), mockMotor);
			positionSignaler = new PositionSignaler(motorLocator);
			motorMover = new MotorMover(positionSignaler, motorLocator, mockMotor);
		}

		private TimeSpan convertToTimeout(int pos)
		{
			int distance = Math.Abs(pos - motorLocator.Position);
			return TimeSpan.FromMilliseconds(distance * 30 + 20);
		}
	}
}
