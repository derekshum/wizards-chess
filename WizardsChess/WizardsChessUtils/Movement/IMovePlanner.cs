using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WizardsChess.Movement
{
	public interface IMovePlanner
	{
		/// <summary>
		/// Provides movement paths for preforming en passant moves.
		/// All input positions are given in ChessBoard logical units.
		/// Output positions are in movement logical units.
		/// </summary>
		/// <param name="start">The current location of the piece to be moved.</param>
		/// <param name="end">The destination for the chess piece.</param>
		/// <param name="captured">The location of the piece being captured, if different from 'end'.</param>
		/// <returns>
		/// A list of List<Point2D>s. Each List<Point2D> indicates a piece move from start to finish.
		/// Point2Ds are in movement logical units (between ChessBoard units and motor step units).
		/// </returns>
		List<IList<Point2D>> PlanMove(Point2D start, Point2D end, Point2D? captured=null);
		List<IList<Point2D>> PlanCastle(Point2D rookStart, int kingCol);
	}
}
