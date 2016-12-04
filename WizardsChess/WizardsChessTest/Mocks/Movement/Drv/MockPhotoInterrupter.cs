using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WizardsChess.Movement.Drv;
using WizardsChess.Movement.Drv.Events;

namespace WizardsChessTest.Mocks.Movement.Drv
{
	class MockPhotoInterrupter : MockGpio, IPhotoInterrupter
	{
		public int GridPosition { get; }

		public MockPhotoInterrupter(int gridPosition, int lowerMotorStepPosition, int upperMotorStepPosition, MockMotor mtr) : base()
		{
			GridPosition = gridPosition;
			upperStepPosition = upperMotorStepPosition;
			lowerStepPosition = lowerMotorStepPosition;

			value = GpioValue.High;

			motor = mtr;
			motor.ValueChanged += motorTicked;
		}

		private int upperStepPosition;
		private int lowerStepPosition;
		private MockMotor motor;
		
		private void motorTicked(object sender, GpioValueChangedEventArgs e)
		{
			if (motor.Position == lowerStepPosition)
			{
				if (motor.State == MotorState.Forward)
				{
					value = GpioValue.Low;
				}
				else if (motor.State == MotorState.Backward)
				{
					value = GpioValue.High;
				}
			}
			else if (motor.Position == upperStepPosition)
			{
				if (motor.State == MotorState.Forward)
				{
					value = GpioValue.High;
				}
				else if (motor.State == MotorState.Backward)
				{
					value = GpioValue.Low;
				}
			}
		}
	}
}
