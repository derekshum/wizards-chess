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
		[TestMethod]
		public void TestMethod1()	//test against move that already exsists in chessboard
		{
			ChessLogic logic = new ChessLogic();
			ChessBoard board = new ChessBoard();
			Assert.AreEqual(logic.Board.ToString(), board.ToString());
			logic.MovePiece(new Position("C", "2"), new Position("C", "4"));
			board.MovePiece(new Position("C", "2"), new Position("C", "4"));
			Assert.AreEqual(logic.Board.ToString(), board.ToString());
			logic.MovePiece(new Position("B", "8"), new Position("C", "6"));
			board.MovePiece(new Position("B", "8"), new Position("C", "6"));
			Assert.AreEqual(logic.Board.ToString(), board.ToString());
			//logic.MovePiece(new Position("C", "4"), new Position("C", "4")); //threw appropriate exception
			//board.MovePiece(new Position("C", "4"), new Position("C", "4"));
			Assert.AreEqual(logic.Board.ToString(), board.ToString());
			//Assert.AreEqual(logic.Board.ToString(), "a");	//display board
		}
		[TestMethod]
		public void TestMethod2()
		{
			ChessLogic logic = new ChessLogic();
			var pawnPlaces = logic.FindPotentialPiecesForMove(PieceType.Pawn, new Position("C","3"));
			Assert.AreEqual(pawnPlaces.Count, 1);
		}
		[TestMethod]
		public void TestMethod3()
		{
			ChessLogic logic = new ChessLogic();
			ChessLogic logic2 = new ChessLogic();
			ChessLogic logic3 = new ChessLogic();
			logic.MovePiece(new Position("C", "2"), new Position("C", "4"));

			logic2.MovePiece(new Position("C", "2"), new Position("C", "4"));
			logic2.MovePiece(new Position("B", "8"), new Position("C", "6"));
			logic2.MovePiece(new Position("C", "4"), new Position("C", "5"));
			logic2.UndoMove();

			logic3.MovePiece(new Position("C", "2"), new Position("C", "4"));
			logic3.MovePiece(new Position("B", "8"), new Position("C", "6"));
			Assert.AreEqual(logic2.Board.ToString(), logic3.Board.ToString());

			logic2.UndoMove();
			Assert.AreEqual(logic2.Board.ToString(), logic.Board.ToString());

			bool invalidMove = logic2.IsMoveValid(new Position("C", "4"), new Position("C", "5"));
			Assert.AreEqual(invalidMove, false);

			logic2.MovePiece(new Position("B", "8"), new Position("C", "6"));
			Assert.AreEqual(logic2.Board.ToString(), logic3.Board.ToString());
			logic2.MovePiece(new Position("C", "4"), new Position("C", "5"));
			logic2.UndoMove();
			Assert.AreEqual(logic2.Board.ToString(), logic3.Board.ToString());
			logic2.UndoMove();
			Assert.AreEqual(logic2.Board.ToString(), logic.Board.ToString());


		}
	}
}
