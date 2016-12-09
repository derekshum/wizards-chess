using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WizardsChess.Movement.Drv.Events;

namespace WizardsChess.Movement.Drv
{
	public interface IPositionSignaler
	{
		int Position { get; }

		void CountToPosition(int numSteps, TimeSpan timeout);
		void CancelSignal();

		event PositionChangedEventHandler FinishedCounting;
		event PositionChangedEventHandler MoveTimedOut;
	}

	public delegate void StepEventHandler(Object sender, StepEventArgs e);
}
