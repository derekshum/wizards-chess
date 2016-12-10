using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WizardsChess.Movement.Drv.Events;

namespace WizardsChess.Movement
{
	public interface IPositionSignaler
	{
		int Position { get; }

		void SignalOnPosition(int numSteps);
		void CancelSignal();

		event PositionChangedEventHandler ReachedPosition;
	}

	public delegate void StepEventHandler(Object sender, StepEventArgs e);
}
