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
			previousMoves = new List<IList<IList<Point2D>>>();
		}

		public async Task MoveAsync(Point2D start, Point2D end, Point2D? captured = null)
		{
			var moveSteps = planner.PlanMove(start, end, captured);
			foreach (var setOfSteps in moveSteps)
			{
				await performer.MovePieceAsync(setOfSteps);
				//TODO: remove: previousMoves[previousMoves.Count - 1].Add(setOfSteps);
			}
			previousMoves.Add(moveSteps);
			await performer.GoHomeAsync();
			
		}

		public async Task UndoMoveAsync()
		{
			var lastMove = previousMoves[previousMoves.Count - 1];
			lastMove.Reverse();
			foreach (var setOfSteps in lastMove)
			{
				setOfSteps.Reverse();
				await performer.MovePieceAsync(setOfSteps);
			}
			previousMoves.RemoveAt(previousMoves.Count - 1);
		}

		private IMovePlanner planner;
		private IMovePerformer performer;
		private IList<IList<IList<Point2D>>> previousMoves;
	}
}
