using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WizardsChess.Movement;
using WizardsChess.Movement.Drv;
using WizardsChessTest.Mocks.Movement.Drv;

namespace WizardsChessTest.Mocks.Movement
{
	class MockMotorMover : MotorMover
	{
		public MockMotorMover(IMotorDrv motor, IMotorLocator locator, IPositionSignaler signaler) : base(4, signaler, locator, motor)
		{
			this.motor = motor;
			this.locator = locator;
			this.signaler = signaler;
		}

		public static MockMotorMover Create()
		{
			var motor = MockMotor.Create();
			var locator = new MotorLocator(new MockGpio(), motor.Information);
			var signaler = new PositionSignaler(locator);
			return new MockMotorMover(motor, locator, signaler);
		}

		public IMotorDrv motor;
		public IMotorLocator locator;
		public IPositionSignaler signaler;
	}
}
