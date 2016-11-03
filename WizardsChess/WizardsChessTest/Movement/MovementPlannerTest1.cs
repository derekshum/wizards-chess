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
			MovementPlanner planner = new MovementPlanner(board);
			ChessPiece TestPiece = planner.board.PieceAt(1, 4);
			Assert.AreEqual(TestPiece.Type, PieceType.Pawn);
		}
		/*public void SampleMoveChecking1()
		{
			String correctPoints = "start move\n[12, 3]\n[12,7\nend move \n";
			//Set moves
			//White King Pawn to E5
			Point2D moveW1Start = new Point2D(4, 1);    //start location of the first white move (0-7, 0-7)
			Point2D moveW1End = new Point2D(4, 3);

			List<List<Point2D>> paths = new List<List<Point2D>>();
			String printString = "";
			ChessBoard board = new ChessBoard();
			MovementPlanner planner = new MovementPlanner(board);
			paths = planner.MoveDebug(moveW1Start, moveW1End);
			
			printString = planner.PrintDebug(paths);
			
			Assert.AreEqual(correctPoints, printString);
		}*/
		/*
		public void SampleMoveChecking2()
		{
			String correctPoints = "start move\n[16, 1]\n"; ///TODO: finish this long ass list
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
			String printString = "";
			ChessBoard board = new ChessBoard();
			MovementPlanner planner = new MovementPlanner(board);
			listOfPaths.Add(planner.MoveDebug(moveW1Start, moveW1End));
			listOfPaths.Add(planner.MoveDebug(moveB1Start, moveB1End));
			listOfPaths.Add(planner.MoveDebug(moveW2Start, moveW2End));

			listOfPaths.ForEach((p) => 
			{
				printString += planner.PrintDebug(p);
			});
			Assert.AreEqual(correctPoints, printString);
		}
		*/
	}
}
