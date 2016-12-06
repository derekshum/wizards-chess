using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WizardsChess.Movement.Drv;
using WizardsChess.Movement.Drv.Events;
using WizardsChess.Movement.Events;

namespace WizardsChess.Movement
{
	public class MotorLocator : IMotorLocator
	{
		public MotorLocator(IGpioPin counter, IMotorDrv motor)
		{
			stepCounter = counter;
			motorDrv = motor;
			position = 0;

			stepCounter.ValueChanged += pinValueChanged;
		}

		public int Position { get { return position; } }

		public event PositionChangedEventHandler PositionChanged;

		public void ShiftPosition(int shift)
		{
			lock (lockObject)
			{
				position += shift;
			}
		}

		private volatile int position;
		private IGpioPin stepCounter;
		private IMotorDrv motorDrv;
		private object lockObject = new object();

		private void pinValueChanged(object sender, GpioValueChangedEventArgs e)
		{
			if (e.Edge == GpioEdge.FallingEdge)
			{
				// If motor hasn't move and latestActiveMoveDirection is still Stopped, this will not impact anything because the Stopped value is 0.
				var pos = 0;
				lock (lockObject)
				{
					position += (int)motorDrv.GetLatestActiveMoveDirection();
					pos = position;
				}
				onStepCounted(pos);
			}
		}

		private void onStepCounted(int pos)
		{
			PositionChanged?.Invoke(this, new PositionChangedEventArgs(pos));
		}
	}
}
