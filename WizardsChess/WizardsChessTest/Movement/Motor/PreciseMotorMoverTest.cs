using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WizardsChess.Movement;
using WizardsChessTest.Mocks.Movement;

namespace WizardsChessTest.Movement.Motor
{
	[TestClass]
	public class PreciseMotorMoverTest
	{
		[TestMethod]
		public void TestPreciseMoverForwardMove()
		{
			var motorMover = MockMotorMover.Create();
			var preciseMover = new PreciseMotorMover(motorMover);

			int targetPos = 5;
			var moveTask = preciseMover.GoToPositionAsync(targetPos);
			while (motorMover.motor.Information.Direction == MoveDirection.Stopped)
			{ }
			while (motorMover.motor.Information.Direction != MoveDirection.Stopped)
			{ }
			motorMover.Locator.ShiftPosition(targetPos + targetPos);
			moveTask.Wait();

			Assert.IsTrue(Math.Abs(targetPos - motorMover.Locator.Position) < 4, "Position did not end at the target position.");
		}

		[TestMethod]
		public void TestPreciseMoverBackwardMove()
		{
			var motorMover = MockMotorMover.Create();
			var preciseMover = new PreciseMotorMover(motorMover);

			int targetPos = -5;
			var moveTask = preciseMover.GoToPositionAsync(targetPos);
			while (motorMover.motor.Information.Direction == MoveDirection.Stopped)
			{ }
			while (motorMover.motor.Information.Direction != MoveDirection.Stopped)
			{ }
			motorMover.Locator.ShiftPosition(targetPos + targetPos);
			moveTask.Wait();

			Assert.IsTrue(Math.Abs(targetPos - motorMover.Locator.Position) < 4, "Position did not end at the target position.");
		}

		[TestMethod]
		public void TestPreciseMoverUnnecessaryMove()
		{
			var motorMover = MockMotorMover.Create();
			var preciseMover = new PreciseMotorMover(motorMover);

			int targetPos = 0;
			preciseMover.GoToPositionAsync(targetPos).Wait();

			Assert.AreEqual(targetPos, motorMover.Locator.Position, "Position did not end at the target position.");
		}

		[TestMethod]
		public void TestPreciseMoverOnlyFirstMove()
		{
			var motorMover = MockMotorMover.Create();
			var preciseMover = new PreciseMotorMover(motorMover);

			int targetPos = 5;
			var moveTask = preciseMover.GoToPositionAsync(targetPos);
			while (motorMover.motor.Information.Direction == MoveDirection.Stopped)
			{}
			while (motorMover.motor.Information.Direction != MoveDirection.Stopped)
			{}
			motorMover.Locator.ShiftPosition(targetPos - motorMover.Locator.Position);
			moveTask.Wait();

			Assert.AreEqual(targetPos, motorMover.Locator.Position, "Position did not end at the target position.");
		}
	}
}
