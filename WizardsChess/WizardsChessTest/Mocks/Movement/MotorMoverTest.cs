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
			int finalPos = motorMover.GoToPositionAsync(targetPos, convertToTimeout(targetPos)).Result;
			Assert.AreEqual(motorLocator.Position, finalPos, $"Motor mover result {finalPos} does not match motorLocator position {motorLocator.Position}");
			Assert.IsTrue(finalPos > targetPos, $"Motor final position {finalPos} did not make sense with target {targetPos}.");
		}

		[TestMethod]
		public void TestMotorMoverForwardThenBackwardMove()
		{
			constructMotorMover();

			int targetPos = 12;
			int finalPos = motorMover.GoToPositionAsync(targetPos, convertToTimeout(targetPos)).Result;
			Assert.AreEqual(motorLocator.Position, finalPos, $"Motor mover result {finalPos} does not match motorLocator position {motorLocator.Position}");
			Assert.IsTrue(finalPos > targetPos, $"Motor final position {finalPos} did not make sense with target {targetPos}.");

			targetPos = -12;
			finalPos = motorMover.GoToPositionAsync(targetPos, convertToTimeout(targetPos)).Result;
			Assert.AreEqual(motorLocator.Position, finalPos, $"Motor mover result {finalPos} does not match motorLocator position {motorLocator.Position}");
			Assert.IsTrue(finalPos < targetPos, $"Motor final position {finalPos} did not make sense with target {targetPos}.");
		}

		[TestMethod]
		public void TestMotorMoverCancel()
		{
			constructMotorMover();

			int targetPos = 300;
			var moveTask = motorMover.GoToPositionAsync(targetPos, convertToTimeout(targetPos));
			Task.Delay(30).Wait();
			motorMover.CancelMove();
			while(mockMotor.Information.Direction != MoveDirection.Stopped)
			{
				Task.Delay(30).Wait();
			}
			Task.Delay(30).Wait();
			Assert.IsTrue(moveTask.IsCompleted, "Move task did not end after cancellation.");
			int finalPos = moveTask.Result;
			Assert.AreEqual(motorLocator.Position, finalPos, $"Motor mover result {finalPos} does not match motorLocator position {motorLocator.Position}");
			Assert.IsTrue(finalPos < targetPos, $"Motor final position {finalPos} did not make sense with target {targetPos}.");
		}

		[TestMethod]
		public void TestMotorStall()
		{
			constructMotorMover();

			int targetPos = 300;
			var moveTask = motorMover.GoToPositionAsync(targetPos, convertToTimeout(targetPos));
			Task.Delay(60).Wait();
			mockMotor.Direction = MoveDirection.Stopped;
			while (mockMotor.Information.Direction != MoveDirection.Stopped)
			{
				Task.Delay(30).Wait();
			}
			Task.Delay(30).Wait();
			Assert.IsTrue(moveTask.IsCompleted, "Move task did not end after cancellation.");
			int finalPos = moveTask.Result;
			Assert.IsTrue(finalPos != targetPos, "Counted all the way to the target position instead of stalling.");
			Assert.AreEqual(motorLocator.Position, finalPos, "MotorLocator position did not match finalPos");
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
