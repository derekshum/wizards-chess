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
		public void LogicMoveTesting()	//test against move that already exsists in chessboard
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
		public void LogicPieceByLocationTesting()
		{
			ChessLogic logic = new ChessLogic();
			var pawnPlaces = logic.FindPotentialPiecesForMove(PieceType.Pawn, new Position("C","3"));
			Assert.AreEqual(pawnPlaces.Count, 1);
		}
		[TestMethod]
		public void LogicUndo()
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
		[TestMethod]
		public void LogicCastleTesting()
		{
			ChessLogic logic = new ChessLogic();
			logic.MovePiece(new Position("E", "2"), new Position("E", "4"));
			logic.MovePiece(new Position("D", "7"), new Position("D", "6"));
			logic.MovePiece(new Position("G", "1"), new Position("F", "3"));
			logic.MovePiece(new Position("C", "8"), new Position("E", "6"));
			logic.MovePiece(new Position("F", "1"), new Position("D", "3"));
			logic.MovePiece(new Position("B", "8"), new Position("C", "6"));
			logic.MovePiece(new Position("E", "1"), new Position("E", "2"));
			System.Diagnostics.Debug.WriteLine(logic.Board.ToString());
			System.Diagnostics.Debug.WriteLine("!!!!!!!!!!!! About to get rook for Black Castle Kingside");
			var rookPos = logic.validRookLocationsForCastling();	//TODO: this is the problem line
			System.Diagnostics.Debug.WriteLine("!!!!!!!!!!!! About to Black Castle Kingside with rook at " + rookPos[0]);
			logic.Castle(rookPos[0]);
			System.Diagnostics.Debug.WriteLine(logic.Board.ToString());
			System.Diagnostics.Debug.WriteLine("!!!!!!!!!!!! About to get rook for White Castle Queenside");
			rookPos = logic.validRookLocationsForCastling();
			System.Diagnostics.Debug.WriteLine("!!!!!!!!!!!! About to White Castle Queenside");
			logic.Castle(rookPos[0]);
		}

	}
}
