using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WizardsChess.Chess;
using WizardsChess.Chess.Pieces;
using WizardsChess.Movement;

namespace WizardsChessTest
{
	[TestClass]
	public class MovementPlannerTest1
	{
		[TestMethod]
		public void MovePlannerInitializationCheck()
		{
			ChessBoard board = new ChessBoard();
			MovePlanner planner = new MovePlanner(board);
			ChessPiece TestPiece = board.PieceAt(4, 1);
			Assert.AreEqual(TestPiece.Type, PieceType.Pawn);
		}

		[TestMethod]
		public void SampleMoveCheckingPawnMoving()
		{
			//String correctPoints = "start move\n[12, 3]\n[12, 7]\nend move \n";
			//Set moves
			//White King Pawn to E5
			Point2D moveW1Start = new Point2D(4, 1);    //start location of the first white move (0-7, 0-7)
			Point2D moveW1End = new Point2D(4, 3);

			List<IList<Point2D>> paths = new List<IList<Point2D>>();
			String printString = "";
			ChessBoard board = new ChessBoard();
			MovePlanner planner = new MovePlanner(board);
			paths = planner.PlanMove(moveW1Start, moveW1End);
			board.MovePiece(new Position(moveW1Start), new Position(moveW1End));
			//System.Diagnostics.Debug.Write(printString);
			Assert.AreEqual("[12, 3]", paths[0][0].ToString());
			Assert.AreEqual("[12, 7]", paths[0][1].ToString());
			//Assert.AreEqual(correctPoints, printString);

		}

		[TestMethod]
		public void CapturingMovementTest()	//tests move taking by enacting an impossible pawn zoom
		{
			//White King Pawn to E2 (magic!)
			Point2D moveW1Start = new Point2D(4, 1);    //start location of the first white move (0-7, 0-7)
			Point2D moveW1End = new Point2D(4, 6);

			List<IList<Point2D>> paths = new List<IList<Point2D>>();
			ChessBoard board = new ChessBoard();
			MovePlanner planner = new MovePlanner(board);
			paths = planner.PlanMove(moveW1Start, moveW1End);
			//board.MovePiece(new Position(moveW1Start), new Position(moveW1End));

			Assert.AreEqual("[12, 13]", paths[0][0].ToString());
			Assert.AreEqual("[12, 14]", paths[0][1].ToString());
			Assert.AreEqual("[2, 14]", paths[0][2].ToString());
			Assert.AreEqual("[2, 15]", paths[0][3].ToString());
			Assert.AreEqual("[0, 15]", paths[0][4].ToString());
			Assert.AreEqual("[12, 3]", paths[1][0].ToString());
			Assert.AreEqual("[12, 13]", paths[1][1].ToString());
		}

		/*
		[TestMethod]
		public void SampleMoveCheckingKnightTaking()	//won't work without board updating
		{
			//Set moves
			//White King Side Knight to F6
			Point2D moveW1Start = new Point2D(6, 0);	//start location of the first white move (0-7, 0-7)
			Point2D moveW1End = new Point2D(5, 2);
			//Black King Pawn to E4
			Point2D moveB1Start = new Point2D(4, 6);
			Point2D moveB1End = new Point2D(4, 4);
			//White Knight takes Black Pawn at E4
			Point2D moveW2Start = moveW1End;
			Point2D moveW2End = moveB1End;

			List<List<List<Point2D>>> listOfPaths = new List<List<List<Point2D>>>();
			//String printString = "";
			ChessBoard board = new ChessBoard();
			MovementPlanner planner = new MovementPlanner(board);
			listOfPaths.Add(planner.Move(moveW1Start, moveW1End));
			listOfPaths.Add(planner.Move(moveB1Start, moveB1End));
			listOfPaths.Add(planner.Move(moveW2Start, moveW2End));

			Assert.AreEqual("[16, 1]", listOfPaths[0][0][0].ToString());
			Assert.AreEqual("[15, 1]", listOfPaths[0][0][1].ToString());
			Assert.AreEqual("[15, 5]", listOfPaths[0][0][2].ToString());
			Assert.AreEqual("[14, 5]", listOfPaths[0][0][3].ToString());
			Assert.AreEqual("[12, 13]", listOfPaths[1][0][0].ToString());
			Assert.AreEqual("[12, 9]", listOfPaths[1][0][1].ToString());
			Assert.AreEqual("[12, 9]", listOfPaths[2][0][0].ToString());
			Assert.AreEqual("[12, 10]", listOfPaths[2][0][1].ToString());
			Assert.AreEqual("[2, 10]", listOfPaths[2][0][2].ToString());
			Assert.AreEqual("[2, 15]", listOfPaths[2][0][3].ToString());
			Assert.AreEqual("[0, 15]", listOfPaths[2][0][4].ToString());
			Assert.AreEqual("[14, 5]", listOfPaths[2][1][0].ToString());
			Assert.AreEqual("[13, 5]", listOfPaths[2][1][1].ToString());
			Assert.AreEqual("[13, 9]", listOfPaths[2][1][2].ToString());
			Assert.AreEqual("[12, 9]", listOfPaths[2][1][3].ToString());
			
		}*/
	}
}
