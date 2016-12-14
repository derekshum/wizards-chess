using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WizardsChessTest.Mocks.Movement.Drv;
using WizardsChess.Movement;
using WizardsChess.Movement.Drv;
using System.Threading.Tasks;

namespace WizardsChessTest.Movement
{
	[TestClass]
	public class MotorLocatorTest
	{
		[TestMethod]
		public void TestMotorLocatorForwardMove()
		{
			var mockMotor = MockMotor.Create();
			var locator = new MotorLocator(new MockGpio(), mockMotor.Information);

			mockMotor.Direction = MoveDirection.Forward;

			Task.Delay(75).Wait();

			mockMotor.Direction = MoveDirection.Stopped;

			Task.Delay(300).Wait();

			Assert.AreEqual(mockMotor.NumTicks, locator.Position, $"Locator count {locator.Position} did not match motor ticks {mockMotor.NumTicks}.");
		}

		[TestMethod]
		public void TestMotorLocatorBackwardMove()
		{
			var mockMotor = MockMotor.Create();
			var locator = new MotorLocator(new MockGpio(), mockMotor.Information);

			mockMotor.Direction = MoveDirection.Backward;

			Task.Delay(75).Wait();

			mockMotor.Direction = MoveDirection.Stopped;

			Task.Delay(300).Wait();

			Assert.AreEqual(mockMotor.NumTicks, -locator.Position, $"Locator count {locator.Position} did not match motor ticks {mockMotor.NumTicks}.");
		}

		[TestMethod]
		public void TestMotorLocatorForwardShiftBackwardMove()
		{
			var mockMotor = MockMotor.Create();
			var locator = new MotorLocator(new MockGpio(), mockMotor.Information);

			mockMotor.Direction = MoveDirection.Forward;

			Task.Delay(75).Wait();

			mockMotor.Direction = MoveDirection.Stopped;

			Task.Delay(300).Wait();

			Assert.AreEqual(mockMotor.NumTicks, locator.Position, $"Locator count {locator.Position} did not match motor ticks {mockMotor.NumTicks} on forward move.");

			var previousPosition = locator.Position;
			locator.ShiftPosition(50);
			Assert.AreEqual(previousPosition + 50, locator.Position, $"Locator count {locator.Position} did not expected position after shift.");
			mockMotor.NumTicks = 0;
			mockMotor.Direction = MoveDirection.Backward;

			Task.Delay(75).Wait();

			mockMotor.Direction = MoveDirection.Stopped;

			Task.Delay(300).Wait();

			Assert.AreEqual(-mockMotor.NumTicks + previousPosition + 50, locator.Position, $"Locator count {locator.Position} did not match motor ticks after backward move.");
		}
	}
}
