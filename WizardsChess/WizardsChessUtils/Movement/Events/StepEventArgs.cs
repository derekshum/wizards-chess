using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WizardsChess.Movement.Drv.Events
{
	public class StepEventArgs : EventArgs
	{
		public int NumSteps { get; set; }

		public StepEventArgs(int numSteps) : base()
		{
			NumSteps = numSteps;
		}
	}
}
