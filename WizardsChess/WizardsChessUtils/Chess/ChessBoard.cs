﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WizardsChess.VoiceControl.Commands;
using WizardsChess.Chess.Pieces;
using WizardsChess.Movement;

namespace WizardsChess.Chess
{
	public class ChessBoard: IChessBoard
	{
		public ChessBoard()
		{
			boardMatrix = new ChessPiece[Size, Size];
			WhoseTurn = ChessTeam.White;
			setBoard();
		}

		/// <summary>
		/// Moves the piece from startPosition to endPosition. Kills the piece at endPosition if it exists.
		/// Throws an InvalidOperationException if this is an invalid move.	//TODO: take this out once it's no longer done here
		/// </summary>
		/// <param name="startPosition"></param>
		/// <param name="endPosition"></param>
		public void MovePiece(Position startPosition, Position endPosition)
		{
            // Kill the piece at the destination, if there is one
            var endPiece = boardMatrix[endPosition.Row, endPosition.Column];
			if (endPiece != null)
			{
				capturedPiecesByTeam[endPiece.Team].Add(endPiece);
				// Remove a killed piece from our valid pieceLocationsByType list
				var listOfEndPieceType = pieceLocationsByType[endPiece.Type];
				listOfEndPieceType.Remove(endPosition);
			} 
			var startPiece = boardMatrix[startPosition.Row, startPosition.Column];
			startPiece.HasMoved = true;
			boardMatrix[endPosition.Row, endPosition.Column] = startPiece;
			boardMatrix[startPosition.Row, startPosition.Column] = null;

			var listOfStartPieceTypes = pieceLocationsByType[startPiece.Type];
			// Replace the old position for this piece with the new position in the pieceLocationsByType list
			listOfStartPieceTypes.Remove(startPosition);
			listOfStartPieceTypes.Add(endPosition);
		}

		/// <summary>
		/// Sets up the board with the black and white pieces in their starting arrangement.
		/// </summary>
		private void setBoard()
		{
			// initialize the rooks
			boardMatrix[WhiteBackRow, LeftRookCol] = new Rook(ChessTeam.White);
			boardMatrix[WhiteBackRow, RightRookCol] = new Rook(ChessTeam.White);
			boardMatrix[BlackBackRow, LeftRookCol] = new Rook(ChessTeam.Black);
			boardMatrix[BlackBackRow, RightRookCol] = new Rook(ChessTeam.Black);

			// initialize the knights
			boardMatrix[WhiteBackRow, LeftKnightCol] = new Knight(ChessTeam.White);
			boardMatrix[WhiteBackRow, RightKnightCol] = new Knight(ChessTeam.White);
			boardMatrix[BlackBackRow, LeftKnightCol] = new Knight(ChessTeam.Black);
			boardMatrix[BlackBackRow, RightKnightCol] = new Knight(ChessTeam.Black);

			// initialize the Bishops
			boardMatrix[WhiteBackRow, LeftBishopCol] = new Bishop(ChessTeam.White);
			boardMatrix[WhiteBackRow, RightBishopCol] = new Bishop(ChessTeam.White);
			boardMatrix[BlackBackRow, LeftBishopCol] = new Bishop(ChessTeam.Black);
			boardMatrix[BlackBackRow, RightBishopCol] = new Bishop(ChessTeam.Black);

			// initialize the Queens
			boardMatrix[WhiteBackRow, QueenCol] = new Queen(ChessTeam.White);
			boardMatrix[BlackBackRow, QueenCol] = new Queen(ChessTeam.Black);

			// initialize the Kings
			boardMatrix[WhiteBackRow, KingCol] = new King(ChessTeam.White);
			boardMatrix[BlackBackRow, KingCol] = new King(ChessTeam.Black);

			// initialize pawns
			for (int col = 0; col < Size; col++)
			{
				boardMatrix[WhiteFrontRow, col] = new Pawn(ChessTeam.White);
				boardMatrix[BlackFrontRow, col] = new Pawn(ChessTeam.Black);
			}

			setupPieceLocationsDictionary();
		}

		//sets up pieceLocationByType
		private void setupPieceLocationsDictionary()
		{
			for (int row = 0; row < Size; row ++)
			{
				for (int col = 0; col < Size; col++)
				{
					var piece = boardMatrix[row, col];
					if (piece != null)
					{
						if (!pieceLocationsByType.ContainsKey(piece.Type))
						{
							pieceLocationsByType[piece.Type] = new List<Point2D>();
						}
						pieceLocationsByType[piece.Type].Add(new Movement.Point2D(col, row));
					}
				}
			}
		}

		//TODO: board reset

		//TODO:? WhoseTurn modifiers(increment or set to a certain team's) and an accessor?- not if Whoseturn is stored in logic

		//TODO: Prints a representation of the board.
		public override string ToString()
		{
			StringBuilder strBuild = new StringBuilder();
			strBuild.Append("\tA\tB\tC\tD\tE\tF\tG\tH\n");
			for (int row = Size - 1; row >= 0; row--)
			{
				strBuild.Append(row).Append("\t");
				for (int col = 0; col < Size; col++)
				{
					var piece = boardMatrix[row, col];
					if (piece != null)
					{
						strBuild.Append(piece.ToShortString());
					}
					strBuild.Append("\t");
				}
				strBuild.Append("\n");
			}
			return strBuild.ToString();
		}


		//piece accessor by x and y indexes
		public ChessPiece PieceAt(int x, int y)
        {
            return boardMatrix[y, x];
        }

		//piece accessor by Point2D
        public ChessPiece PieceAt(Point2D location)
        {
            return boardMatrix[location.Y, location.X];
		}

		//piece accessor by Position
		public ChessPiece PieceAt(Position location)
		{
			return boardMatrix[location.Row, location.Column];
		}
		//TODO:? piece accessor by alphanumeric string

		/*//TODO: accessor (public dictionary) for positionLocationsByType (the whole thing)
		public IReadOnlyDictionary<PieceType, IReadOnlyList<Point2D>> PieceLocationByType()
		{
		   get { return pieceLocationByType; }
		}*/

        //access the number of captured pieces on a team
        public int NumCapturedPieces(ChessTeam team)
        {
            return capturedPiecesByTeam[team].Count;
        }

        public const int Size = 8;
        public ChessTeam WhoseTurn;	//TODO: move to ChessLogic? Change to Private (and add modifiers and an accessor?)

		private ChessPiece[,] boardMatrix;
		private Dictionary<PieceType, IList<Point2D>> pieceLocationsByType = new Dictionary<PieceType, IList<Point2D>>();    //TODO: figure out how to expose pieceLocationsByType properly
		public Dictionary<PieceType, IList<Point2D>> PieceLocationsByType
		{
			get
			{
				var temp = pieceLocationsByType;
				return temp;
			}
		}
		private IDictionary<ChessTeam, ISet<ChessPiece>> capturedPiecesByTeam = new Dictionary<ChessTeam, ISet<ChessPiece>>()
		{
			{ChessTeam.White, new HashSet<ChessPiece>()},
			{ChessTeam.Black, new HashSet<ChessPiece>()}
		};
		public IDictionary<ChessTeam, ISet<ChessPiece>> CapturedPiecesByTeam
		{
			get
			{
				var temp = capturedPiecesByTeam;
				return temp;
			}
		}

		private const int WhiteBackRow = 0;
		private const int WhiteFrontRow = 1;
		private const int BlackBackRow = 7;
		private const int BlackFrontRow = 6;
		private const int LeftRookCol = 0;
		private const int LeftKnightCol = 1;
		private const int LeftBishopCol = 2;
		private const int KingCol = 3;
		private const int QueenCol = 4;
		private const int RightBishopCol = 5;
		private const int RightKnightCol = 6;
		private const int RightRookCol = 7;
	}
}
