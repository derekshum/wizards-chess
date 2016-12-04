using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WizardsChess.Chess;
using WizardsChess.Movement;
using WizardsChess.Movement.Drv;
using WizardsChess.VoiceControl;
using WizardsChess.VoiceControl.Commands;
using WizardsChess.VoiceControl.Events;

namespace WizardsChessTest.Movement
{
	[TestClass]
	public class MoveTesting
	{
		[TestMethod]
		public void BasicCompilationTesting()
		{
			ChessLogic logic = new ChessLogic();
			var movePlanner = new MovePlanner(logic.Board);
			var moveManager = new MoveManager(movePlanner, new MovePerformer(null, null, null));
		}
	}
}
