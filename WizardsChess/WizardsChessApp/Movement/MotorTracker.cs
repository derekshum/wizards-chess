using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Gpio;
using WizardsChess;
using WizardsChessApp.Movement.Drv;

namespace WizardsChessApp.Movement
{
	public class MotorTracker : StepTracker
	{
		public MotorTracker(int pinNum, MotorDrv motorDrv)
			: base(pinNum)
		{
			this.motorDrv = motorDrv;
		}

		protected override void countStep(GpioPin p, GpioPinValueChangedEventArgs args)
		{
			if (motorDrv.State == MotorState.Forward)
			{
				if (args.Edge == GpioPinEdge.FallingEdge)
				{
					Position++;
				}
			}
			else if (motorDrv.State == MotorState.Backward)
			{
				if (args.Edge == GpioPinEdge.RisingEdge)
				{
					Position--;
				}
			}
			else
			{
				System.Diagnostics.Debug.WriteLine("Read step when motor stopped.");
			}
		}

		private MotorDrv motorDrv;
	}
}
