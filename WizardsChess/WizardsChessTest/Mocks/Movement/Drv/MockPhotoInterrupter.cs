using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WizardsChess.Movement;
using WizardsChess.Movement.Drv;
using WizardsChess.Movement.Drv.Events;
using WizardsChess.Movement.Events;

namespace WizardsChessTest.Mocks.Movement.Drv
{
	class MockPhotoInterrupter : MockGpio, IPhotoInterrupter
	{
		public int GridPosition { get; }

		public int StepPosition { get; set; }

		public MockPhotoInterrupter(int gridPosition, int lowerMotorStepPosition, int upperMotorStepPosition, IMotorLocator mtrLocator, MockMotor mtr) : base()
		{
			GridPosition = gridPosition;
			StepPosition = (upperMotorStepPosition + lowerMotorStepPosition) / 2;
			UpperStepPosition = upperMotorStepPosition;
			LowerStepPosition = lowerMotorStepPosition;

			value = GpioValue.High;

			locator = mtrLocator;
			motor = mtr;
			locator.PositionChanged += positionChanged;
		}

		public int UpperStepPosition;
		public int LowerStepPosition;
		private IMotorLocator locator;
		private MockMotor motor;
		
		private void positionChanged(object sender, PositionChangedEventArgs e)
		{
			if (locator.Position == LowerStepPosition)
			{
				if (motor.Direction == MoveDirection.Forward)
				{
					value = GpioValue.Low;
				}
				else if (motor.Direction == MoveDirection.Backward)
				{
					value = GpioValue.High;
				}
			}
			else if (locator.Position == UpperStepPosition)
			{
				if (motor.Direction == MoveDirection.Forward)
				{
					value = GpioValue.High;
				}
				else if (motor.Direction == MoveDirection.Backward)
				{
					value = GpioValue.Low;
				}
			}
		}
	}
}
