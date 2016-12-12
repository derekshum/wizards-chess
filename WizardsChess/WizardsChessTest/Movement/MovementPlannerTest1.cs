using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WizardsChess.Chess;
using WizardsChess.Chess.Pieces;
using WizardsChess.Movement;
using System.Threading.Tasks;

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
			//String printString = "";
			ChessBoard board = new ChessBoard();
			MovePlanner planner = new MovePlanner(board);
			paths = planner.PlanMove(moveW1Start, moveW1End);
			board.MovePiece(new Position(moveW1Start), new Position(moveW1End));
			//System.Diagnostics.Debug.Write(printString);
			Assert.AreEqual("[1, -5]", paths[0][0].ToString());
			Assert.AreEqual("[1, -1]", paths[0][1].ToString());
			//Assert.AreEqual(correctPoints, printString);

		}

		[TestMethod]
		public void CapturingMovementTest() //tests move taking by enacting an impossible pawn zoom
		{
			//White King Pawn to E2 (magic!)
			Point2D moveW1Start = new Point2D(4, 1);    //start location of the first white move (0-7, 0-7)
			Point2D moveW1End = new Point2D(4, 6);

			List<IList<Point2D>> paths = new List<IList<Point2D>>();
			ChessBoard board = new ChessBoard();
			MovePlanner planner = new MovePlanner(board);
			System.Diagnostics.Debug.WriteLine("about to get path");
			paths = planner.PlanMove(moveW1Start, moveW1End);
			//board.MovePiece(new Position(moveW1Start), new Position(moveW1End));

			Assert.AreEqual("[1, 5]", paths[0][0].ToString());
			Assert.AreEqual("[1, 6]", paths[0][1].ToString());
			Assert.AreEqual("[-9, 6]", paths[0][2].ToString());
			Assert.AreEqual("[-9, 7]", paths[0][3].ToString());
			Assert.AreEqual("[-11, 7]", paths[0][4].ToString());
			Assert.AreEqual("[1, -5]", paths[1][0].ToString());
			Assert.AreEqual("[1, 5]", paths[1][1].ToString());
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
		[TestMethod]
		public async Task VisualizerTest1()
		{
			ChessLogic logic = new ChessLogic();
			MovePlanner planner = new MovePlanner(logic.Board);
			MovePerformerVisualizer visualizer = new MovePerformerVisualizer();
			MoveManager manager = new MoveManager(planner, visualizer);

			visualizer.ResetBoardRep();
			visualizer.PrintBoardRep();

			List<Position[]> moves = new List<Position[]>();

			Position[] move = new Position[2];	//can't reuse move like that, apparently
			move[0] = new Position("E", "2");
			move[1] = new Position("E", "4");
			moves.Add(move);
			foreach (var movement in moves)
			{
				logic.IsMoveValid(movement[0], movement[1]);
				await manager.MoveAsync(movement[0], movement[1]);
				logic.MovePiece(movement[0], movement[1]);
			}
		}

		[TestMethod]
		public async Task VisualizerTest2CastlingAndTaking()
		{
			ChessLogic logic = new ChessLogic();
			MovePlanner planner = new MovePlanner(logic.Board);
			MovePerformerVisualizer visualizer = new MovePerformerVisualizer();
			MoveManager manager = new MoveManager(planner, visualizer);

			visualizer.ResetBoardRep();
			visualizer.PrintBoardRep();

			List<Position[]> moves = new List<Position[]>();

			Position[] move1 = new Position[2];
			move1[0] = new Position("E", "2");
			move1[1] = new Position("E", "4");
			moves.Add(move1);
			Position[] move2 = new Position[2];
			move2[0] = new Position("D", "7");
			move2[1] = new Position("D", "5");
			moves.Add(move2);
			Position[] move3 = new Position[2];
			move3[0] = new Position("G", "1");
			move3[1] = new Position("F", "3");
			moves.Add(move3);
			Position[] move4 = new Position[2];
			move4[0] = new Position("C", "8");
			move4[1] = new Position("E", "6");
			moves.Add(move4);
			Position[] move5 = new Position[2];
			move5[0] = new Position("F", "1");
			move5[1] = new Position("C", "4");
			moves.Add(move5);
			Position[] move6 = new Position[2];
			move6[0] = new Position("B", "8");
			move6[1] = new Position("C", "6");
			moves.Add(move6);
			Position[] move7 = new Position[2];
			move7[0] = new Position("E", "1");
			move7[1] = new Position("E", "2");
			moves.Add(move7);

			System.Diagnostics.Debug.WriteLine(logic.Board.ToString());

			foreach (var movement in moves)
			{
				Assert.AreEqual(logic.validRookLocationsForCastling().Count, 0);    //assert castling invalid when things in the way

				System.Diagnostics.Debug.WriteLine(movement[0].ToString() + "\t" + movement[1].ToString());
				Assert.AreEqual(logic.IsMoveValid(movement[0], movement[1]),true);
				await manager.MoveAsync(movement[0], movement[1]);
				logic.MovePiece(movement[0], movement[1]);
				System.Diagnostics.Debug.WriteLine(logic.Board.ToString());
			}

			moves.Clear();
			Position[] move8 = new Position[2];
			move8[0] = new Position("D", "5");
			move8[1] = new Position("E", "4");
			moves.Add(move8);
			Position[] move9 = new Position[2];
			move9[0] = new Position("C", "4");
			move9[1] = new Position("E", "6");
			moves.Add(move9);
			Position[] move10 = new Position[2];
			move10[0] = new Position("E", "4");
			move10[1] = new Position("F", "3");
			moves.Add(move10);
			Position[] move11 = new Position[2];
			move11[0] = new Position("A", "2");
			move11[1] = new Position("A", "3");
			moves.Add(move11);

			foreach (var movement in moves)
			{
				System.Diagnostics.Debug.WriteLine(movement[0].ToString() + "\t" + movement[1].ToString());
				Assert.AreEqual(logic.IsMoveValid(movement[0], movement[1]), true);
				await manager.MoveAsync(movement[0], movement[1]);
				logic.MovePiece(movement[0], movement[1]);
				System.Diagnostics.Debug.WriteLine(logic.Board.ToString());
			}

			//verify that castling while in check stops castling
			Assert.AreEqual(logic.validRookLocationsForCastling().Count, 0);

			moves.Clear();
			Position[] move12 = new Position[2];
			move12[0] = new Position("F", "7");
			move12[1] = new Position("E", "6");
			moves.Add(move12);
			Position[] move13 = new Position[2];
			move13[0] = new Position("E", "2");
			move13[1] = new Position("F", "3");
			moves.Add(move13);

			foreach (var movement in moves)
			{
				System.Diagnostics.Debug.WriteLine(movement[0].ToString() + "\t" + movement[1].ToString());
				Assert.AreEqual(logic.IsMoveValid(movement[0], movement[1]), true);
				await manager.MoveAsync(movement[0], movement[1]);
				logic.MovePiece(movement[0], movement[1]);
				System.Diagnostics.Debug.WriteLine(logic.Board.ToString());
			}

			System.Diagnostics.Debug.WriteLine("Black castles kingside.");
			var rookLocationForBlackCastle = logic.validRookLocationsForCastling();
			Assert.AreEqual(rookLocationForBlackCastle.Count, 1);
			await manager.CastleAsync(rookLocationForBlackCastle[0], logic.Board.GetKingCol());
			logic.Castle(rookLocationForBlackCastle[0]);
			System.Diagnostics.Debug.WriteLine(logic.Board.ToString());

			System.Diagnostics.Debug.WriteLine("White castles queenside.");
			var rookLocationForWhiteCastle = logic.validRookLocationsForCastling();
			Assert.AreEqual(rookLocationForWhiteCastle.Count, 1);
			await manager.CastleAsync(rookLocationForWhiteCastle[0], logic.Board.GetKingCol());
			logic.Castle(rookLocationForWhiteCastle[0]);
			System.Diagnostics.Debug.WriteLine(logic.Board.ToString());
		}

		[TestMethod]
		public async Task VisualizerTest3BasicUndo()
		{
			ChessLogic logic = new ChessLogic();
			MovePlanner planner = new MovePlanner(logic.Board);
			MovePerformerVisualizer visualizer = new MovePerformerVisualizer();
			MoveManager manager = new MoveManager(planner, visualizer);

			visualizer.ResetBoardRep();
			visualizer.PrintBoardRep();

			List<Position[]> moves = new List<Position[]>();

			Position[] move1 = new Position[2];
			move1[0] = new Position("E", "2");
			move1[1] = new Position("E", "4");
			moves.Add(move1);
			Position[] move2 = new Position[2];
			move2[0] = new Position("D", "7");
			move2[1] = new Position("D", "5");
			moves.Add(move2);
			Position[] move3 = new Position[2];
			move3[0] = new Position("G", "1");
			move3[1] = new Position("F", "3");
			moves.Add(move3);
			Position[] move4 = new Position[2];
			move4[0] = new Position("C", "8");
			move4[1] = new Position("E", "6");
			moves.Add(move4);
			Position[] move5 = new Position[2];
			move5[0] = new Position("F", "1");
			move5[1] = new Position("C", "4");
			moves.Add(move5);
			Position[] move6 = new Position[2];
			move6[0] = new Position("B", "8");
			move6[1] = new Position("C", "6");
			moves.Add(move6);
			Position[] move7 = new Position[2];
			move7[0] = new Position("E", "1");
			move7[1] = new Position("E", "2");
			moves.Add(move7);
			Position[] move8 = new Position[2];
			move8[0] = new Position("D", "5");
			move8[1] = new Position("E", "4");
			moves.Add(move8);
			Position[] move9 = new Position[2];
			move9[0] = new Position("C", "4");
			move9[1] = new Position("E", "6");
			moves.Add(move9);
			Position[] move10 = new Position[2];
			move10[0] = new Position("E", "4");
			move10[1] = new Position("F", "3");
			moves.Add(move10);
			Position[] move11 = new Position[2];
			move11[0] = new Position("A", "2");
			move11[1] = new Position("A", "3");
			moves.Add(move11);
			Position[] move12 = new Position[2];
			move12[0] = new Position("F", "7");
			move12[1] = new Position("E", "6");
			moves.Add(move12);
			Position[] move13 = new Position[2];
			move13[0] = new Position("E", "2");
			move13[1] = new Position("F", "3");
			moves.Add(move13);

			System.Diagnostics.Debug.WriteLine(logic.Board.ToString());

			foreach (var movement in moves)
			{
				System.Diagnostics.Debug.WriteLine(movement[0].ToString() + "\t" + movement[1].ToString());
				Assert.AreEqual(logic.IsMoveValid(movement[0], movement[1]), true);
				await manager.MoveAsync(movement[0], movement[1]);
				logic.MovePiece(movement[0], movement[1]);
				System.Diagnostics.Debug.WriteLine(logic.Board.ToString());
			}

			System.Diagnostics.Debug.WriteLine("Black castles kingside.");
			var rookLocationForBlackCastle = logic.validRookLocationsForCastling();
			Assert.AreEqual(rookLocationForBlackCastle.Count, 1);
			await manager.CastleAsync(rookLocationForBlackCastle[0], logic.Board.GetKingCol());
			logic.Castle(rookLocationForBlackCastle[0]);
			System.Diagnostics.Debug.WriteLine(logic.Board.ToString());

			System.Diagnostics.Debug.WriteLine("White castles queenside.");
			var rookLocationForWhiteCastle = logic.validRookLocationsForCastling();
			Assert.AreEqual(rookLocationForWhiteCastle.Count, 1);
			await manager.CastleAsync(rookLocationForWhiteCastle[0], logic.Board.GetKingCol());
			logic.Castle(rookLocationForWhiteCastle[0]);
			System.Diagnostics.Debug.WriteLine(logic.Board.ToString());

			for (int i = 0; i < moves.Count + 2; i++)	//+2 for the two castles
			{
				await manager.UndoMoveAsync();
				logic.UndoMove();
				System.Diagnostics.Debug.WriteLine(logic.Board.ToString());
			}

		}
	}
}
