using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WizardsChess.Chess;
using WizardsChess.Movement.Drv;

namespace WizardsChess.Movement
{
	public class MoveManager : IMoveManager
	{
		public MoveManager(IMovePlanner movePlanner, IMovePerformer movePerformer)
		{
			planner = movePlanner;
			performer = movePerformer;
			previousMove = new List<IList<Point2D>>();
		}

		public void Move(Point2D start, Point2D end, Point2D? captured = null)
		{
			var moveSteps = planner.PlanMove(start, end, captured);

			previousMove.Clear();

			foreach (var setOfSteps in moveSteps)
			{
				performer.MovePiece(setOfSteps);
				previousMove.Add(setOfSteps);
			}
			performer.GoHome();
		}

		private IMovePlanner planner;
		private IMovePerformer performer;
		private IList<IList<Point2D>> previousMove;
	}
}
