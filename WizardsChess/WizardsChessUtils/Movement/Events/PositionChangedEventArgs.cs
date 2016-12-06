using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WizardsChess.Movement.Events
{
	public class PositionChangedEventArgs : EventArgs
	{
		public int Position { get; }

		public PositionChangedEventArgs(int pos)
		{
			Position = pos;
		}
	}
}
