using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WizardsChess.Movement.Drv
{
	public interface IMotorDrv
	{
		MoveDirection Direction { get; set; }
		MoveDirection PreviousDirection { get; }

		/// <summary>
		/// Returns the most-recent move that was not Stopped.
		/// </summary>
		/// <returns></returns>
		MoveDirection GetLatestActiveMoveDirection();
	}
}
