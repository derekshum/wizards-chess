using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WizardsChess.Movement.Drv.Events;

namespace WizardsChess.Movement.Drv
{
	public class MotorInformation : IMotorInformation
	{
		public MotorInformation(Axis axis, IGpioPin motorStepPin)
		{
			motorStepPin.ValueChanged += pinValueChanged;
			Axis = axis;
			Direction = MoveDirection.Stopped;

			stepTimer = new Timer(timerTickCallback, null, Timeout.Infinite, Timeout.Infinite);
		}

		public Axis Axis { get; }
		public MoveDirection Direction
		{
			get { return direction; }

			private set
			{
				if (direction != value)
				{
					direction = value;
					DirectionChanged?.Invoke(this, new MotorDirectionChangedEventArgs(direction));
				}
			}
		}

		public event MotorDirectionChangedEventHandler DirectionChanged;

		public void SetDirection(MoveDirection dir)
		{
			Direction = dir;
			if (dir != MoveDirection.Stopped)
			{
				ticksSinceLastStep = 0;
				stepTimer.Change(0, 15);
			}
		}

		private IGpioPin steppingPin;
		private Timer stepTimer;
		private volatile MoveDirection direction;
		private volatile int ticksSinceLastStep;
		private const int MAX_TICKS_BETWEEN_STEPS = 5;


		private void pinValueChanged(object sender, GpioValueChangedEventArgs e)
		{
			ticksSinceLastStep = 0;
			if (Direction == MoveDirection.Stopped)
			{
				System.Diagnostics.Debug.WriteLine("${Axis} motor moved when in Stopped State!");
			}
		}

		private void timerTickCallback(object state)
		{
			ticksSinceLastStep++;
			if (ticksSinceLastStep > MAX_TICKS_BETWEEN_STEPS)
			{
				Direction = MoveDirection.Stopped;
				stepTimer.Change(Timeout.Infinite, Timeout.Infinite);
			}
		}
	}
}
