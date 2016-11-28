using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WizardsChess.Movement
{
	interface IMovementPlanner
	{
		/// <summary>
		/// Provides movement paths for preforming en passant moves.
		/// All positions are given in ChessBoard logical units.
		/// </summary>
		/// <param name="start">The current location of the piece to be moved.</param>
		/// <param name="end">The destination for the chess piece.</param>
		/// <param name="captured">The location of the piece being captured, if different from 'end'.</param>
		/// <returns></returns>
		List<List<Point2D>> Move(Point2D start, Point2D end, Point2D? captured=null);
	}
}
