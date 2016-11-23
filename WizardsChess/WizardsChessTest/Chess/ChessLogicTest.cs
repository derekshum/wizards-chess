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
			ChessLogic planner = new ChessLogic();
			ChessBoard board = new ChessBoard();
			Assert.AreEqual(planner.Board.ToString(), board.ToString());
			planner.MovePiece(new Position("C", "2"), new Position("C", "4"));
			board.MovePiece(new Position("C", "2"), new Position("C", "4"));
			Assert.AreEqual(planner.Board.ToString(), board.ToString());
			planner.MovePiece(new Position("B", "8"), new Position("C", "6"));
			board.MovePiece(new Position("B", "8"), new Position("C", "6"));
			Assert.AreEqual(planner.Board.ToString(), board.ToString());
			//planner.MovePiece(new Position("C", "4"), new Position("C", "4")); //threw appropriate exception
			//board.MovePiece(new Position("C", "4"), new Position("C", "4"));
			Assert.AreEqual(planner.Board.ToString(), board.ToString());
			Assert.AreEqual(planner.Board.ToString(), "a");	//display board
		}
		public void TestMethod2()
		{
			ChessLogic Logic = new ChessLogic();
			var pawnPlaces = Logic.FindPotentialPiecesForMove(PieceType.Rook, new Position("C","3"));
			Assert.AreEqual(pawnPlaces.Count, 1);
		}
	}
}
