using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace WizardsChessTest.Mocks.Movement.Drv
{
	[TestClass]
	public class MockMotorTest
	{
		[TestMethod]
		public void TestMotor()
		{
			var motor = new MockMotor();
			var pos = 0;
			motor.SetState(WizardsChess.Movement.Drv.MotorState.Forward);
			Task.Delay(200).Wait();
			pos = motor.Position;
			Assert.IsTrue(pos > 0, "Motor did not increase Position when going forwards");
			motor.SetState(WizardsChess.Movement.Drv.MotorState.Stopped);
			Task.Delay(500).Wait();
			Assert.IsTrue(motor.Position > pos, "Motor went backwards after stopping");
			pos = motor.Position;
			motor.SetState(WizardsChess.Movement.Drv.MotorState.Backward);
			Task.Delay(200).Wait();
			Assert.IsTrue(motor.Position < pos, "Motor did not decrease Position when going backwards");
			pos = motor.Position;
			motor.SetState(WizardsChess.Movement.Drv.MotorState.Stopped);
			Task.Delay(500).Wait();
			Assert.IsTrue(motor.Position < pos, "Motor went forwards after stopping");
		}
	}
}
