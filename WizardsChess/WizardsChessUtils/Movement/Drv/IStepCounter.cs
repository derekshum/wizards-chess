using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WizardsChess.Movement.Drv.Events;

namespace WizardsChess.Movement.Drv
{
	public interface IStepCounter
	{
		int Position { get; }

		void CountSteps(int numSteps);

		event StepEventHandler FinishedCounting;
		event StepEventHandler AdditionalStepsCounted;
	}

	public delegate void StepEventHandler(Object sender, StepEventArgs e);
}
