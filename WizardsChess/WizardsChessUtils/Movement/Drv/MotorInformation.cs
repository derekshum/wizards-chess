using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WizardsChess.Movement.Drv.Events;

namespace WizardsChess.Movement.Drv
{
	public class MotorInformation : IMotorInformation, IDisposable
	{
		public MotorInformation(Axis axis, IGpioPin motorStepPin)
		{
			SteppingPin = motorStepPin;
			SteppingPin.ValueChanged += pinValueChanged;
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
		public IGpioPin SteppingPin { get; }

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

		private Timer stepTimer;
		private volatile MoveDirection direction;
		private volatile int ticksSinceLastStep;
		private const int MAX_TICKS_BETWEEN_STEPS = 10;


		private void pinValueChanged(object sender, GpioValueChangedEventArgs e)
		{
			ticksSinceLastStep = 0;
			if (Direction == MoveDirection.Stopped)
			{
				System.Diagnostics.Debug.WriteLine($"{Axis} motor moved when in Stopped State!");
			}
		}

		private void timerTickCallback(object state)
		{
			// Check this class hasn't been disposed
			if (disposedValue)
			{
				return;
			}

			ticksSinceLastStep++;
			if (ticksSinceLastStep > MAX_TICKS_BETWEEN_STEPS)
			{
				Direction = MoveDirection.Stopped;
				stepTimer.Change(Timeout.Infinite, Timeout.Infinite);
			}
		}

		#region IDisposable Support
		private bool disposedValue = false; // To detect redundant calls

		protected virtual void Dispose(bool disposing)
		{
			if (!disposedValue)
			{
				if (disposing)
				{
					stepTimer.Dispose();
					SteppingPin.ValueChanged -= pinValueChanged;
				}

				disposedValue = true;
			}
		}

		// This code added to correctly implement the disposable pattern.
		public void Dispose()
		{
			// Do not change this code. Put cleanup code in Dispose(bool disposing) above.
			Dispose(true);
		}
		#endregion
	}
}
