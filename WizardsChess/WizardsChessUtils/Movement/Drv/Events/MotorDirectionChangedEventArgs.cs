using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WizardsChess.Movement.Drv.Events
{
	public class MotorDirectionChangedEventArgs : EventArgs
	{
		public MoveDirection Direction { get; }
		public MotorDirectionChangedEventArgs(MoveDirection direction)
		{
			Direction = direction;
		}
	}
}
