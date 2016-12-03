using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WizardsChess.Movement
{
	public interface IMoveManager
	{
		/// <summary>
		/// Perform a chess move.
		/// </summary>
		/// <param name="start">The current location of the piece to be moved.</param>
		/// <param name="end">The destination for the chess piece.</param>
		/// <param name="captured">The location of the piece being captured, if different from 'end'.</param>
		Task MoveAsync(Point2D start, Point2D end, Point2D? captured=null);
	}
}
