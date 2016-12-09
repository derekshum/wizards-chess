using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WizardsChess.Movement.Events;

namespace WizardsChess.Movement
{
	public interface IMotorLocator
	{
		int Position { get; }
		MoveDirection LastMoveDirection { get; }

		void ShiftPosition(int shift);

		event PositionChangedEventHandler PositionChanged;
	}

	public delegate void PositionChangedEventHandler(object sender, PositionChangedEventArgs e);
}
