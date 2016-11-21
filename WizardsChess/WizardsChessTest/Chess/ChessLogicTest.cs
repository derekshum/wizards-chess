using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WizardsChess.Chess;
using WizardsChess.Chess.Pieces;
using System.Collections.Generic;

namespace WizardsChessTest.Chess
{
	[TestClass]
	public class ChessLogicTest
	{
		/*[TestMethod]
		public void TestMethod1()	//test against move that already exsists in chessboard
		{
			ChessBoard board1 = new ChessBoard();	//chessboard not wrapped
			ChessBoard board2 = new ChessBoard();	//chessboard fed into ChessLogic
			ChessLogic planner = new ChessLogic(board2);

		}*/
		public void TestMethod2()
		{
			ChessLogic Logic = new ChessLogic();
			var pawnPlaces = Logic.FindPotentialPiecesForMove(PieceType.Rook, new Position("C","3"));
			Assert.AreEqual(pawnPlaces.Count, 1);
		}
	}
}
