using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using WizardsChess.Movement;

namespace WizardsChessTest.Mocks.Movement.Drv
{
	[TestClass]
	public class MockMotorTest
	{
		[TestMethod]
		public void TestMockMotor()
		{
			var motor = new MockMotor();
			var pos = 0;
			motor.Direction = MoveDirection.Forward;
			Task.Delay(200).Wait();
			pos = motor.NumTicks;
			Assert.IsTrue(pos > 0, "Motor did not increase NumTicks when going forwards");
			motor.Direction = MoveDirection.Stopped;
			Task.Delay(300).Wait();
			Assert.IsTrue(motor.NumTicks > pos, "Motor did no extra ticks after stopping");
			pos = 0;
			motor.Direction = MoveDirection.Backward;
			Task.Delay(200).Wait();
			Assert.IsTrue(motor.NumTicks > pos, "Motor did not increase NumTicks when going backwards");
			pos = motor.NumTicks;
			motor.Direction = MoveDirection.Stopped;
			Task.Delay(300).Wait();
			Assert.IsTrue(motor.NumTicks > pos, "Motor did no extra ticks after stopping");
		}
	}
}
