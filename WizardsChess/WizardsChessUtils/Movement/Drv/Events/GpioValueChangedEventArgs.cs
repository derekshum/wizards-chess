using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WizardsChess.Movement.Drv.Events
{
	public enum GpioEdge
	{
		FallingEdge,
		RisingEdge
	}

	public class GpioValueChangedEventArgs : EventArgs
	{
		public GpioEdge Edge { get; }

		public GpioValueChangedEventArgs(GpioEdge e)
		{
			Edge = e;
		}
	}
}
