using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WizardsChess.Movement.Drv.Events
{
	public enum InterruptEdge
	{
		FallingEdge,
		RisingEdge
	}

	public class InterruptEventArgs : EventArgs
	{
		public InterruptEdge Edge { get; }

		public InterruptEventArgs(InterruptEdge edge)
		{
			Edge = edge;
		}
	}
}
