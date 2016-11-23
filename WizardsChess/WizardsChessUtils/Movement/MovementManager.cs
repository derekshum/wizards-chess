using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WizardsChess.Chess;
using WizardsChess.Movement.Drv;

namespace WizardsChess.Movement
{
	class MovementManager
	{
		public MovementManager(IChessBoard board, IMovePerformer movePerformer)
		{
			planner = new MovementPlanner(board);
			performer = movePerformer;
		}

		private MovementPlanner planner;
		private IMovePerformer performer;
	}
}
