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

		public async Task MoveAsync(Point2D start, Point2D end, Point2D? captured = null)
		{
			var moveSteps = planner.PlanMove(start, end, captured);

			previousMove.Clear();

			foreach (var setOfSteps in moveSteps)
			{
				await performer.MovePieceAsync(setOfSteps);
				previousMove.Add(setOfSteps);
			}
			await performer.GoHomeAsync();
		}

		private IMovePlanner planner;
		private IMovePerformer performer;
		private IList<IList<Point2D>> previousMove;
	}
}
