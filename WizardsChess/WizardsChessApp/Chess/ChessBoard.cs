using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WizardsChessApp.CommandConversion;
using WizardsChessApp.Chess.Pieces;
using WizardsChessApp.Movement;

namespace WizardsChessApp.Chess
{
	public class ChessBoard
	{
		public ChessBoard()
		{
			boardMatrix = new ChessPiece[Size, Size];
			WhoseTurn = ChessTeam.White;
			setBoard();
		}

		public ISet<Position> FindPotentialPiecesForMove(PieceType piece, Position destination)
		{
			var pieceLocationList = pieceLocationsByType[piece];

			var potentialPiecePositions = new HashSet<Position>();

			foreach (var location in pieceLocationList)
			{
				if (isMoveValid(new Position(location), destination))
				{
					potentialPiecePositions.Add(new Position(location));
				}
			}

			return potentialPiecePositions;
		}

		/// <summary>
		/// Checks if the move from startPosition to endPosition is valid.
		/// Assumes that startPosition and endPosition are valid parameters.
		/// Returns false if there is no piece at startPosition, or the piece otherwise
		/// cannot complete the requested move.
		/// </summary>
		/// <param name="startPosition">The position of the piece to move.</param>
		/// <param name="endPosition">The destination of the piece.</param>
		/// <returns></returns>
		private bool isMoveValid(Position startPosition, Position endPosition)
		{
			// Get piece at input location
			ChessPiece startPiece = boardMatrix[startPosition.Row, startPosition.Column];
			ChessPiece endPiece = boardMatrix[endPosition.Row, endPosition.Column];

			// If there is no piece at the requested start position, return false
			if (startPiece == null)
			{
				return false;
			}

			// It's not this pieces turn to move
			if (startPiece.Team != WhoseTurn)
			{
				return false;
			}

			IReadOnlyList<Vector2D> pieceMovementVectors;
			if (endPiece == null)
			{
				pieceMovementVectors = startPiece.GetAllowedMotionVectors();
			}
			else
			{
				// If there is a piece in the way and it is a friendly piece, then we can't move there
				if (endPiece.Team == startPiece.Team)
				{
					return false;
				}
				pieceMovementVectors = startPiece.GetAttackMotionVectors();
			}

			var requestedMoveVector = (Point2D)endPosition - startPosition;

			try
			{
				var matchingMove = pieceMovementVectors.Single(v => v == requestedMoveVector);
			}
			catch (InvalidOperationException)
			{
				// Could not retrieve a matching vector from the allowed moves
				return false;
			}
			
			// If the piece can jump, it doesn't matter if something is in the way
			if (startPiece.CanJump)
			{
				return true;
			}

			return isPathClear(startPosition, endPosition);
		}

		/// <summary>
		/// Moves the piece from startPosition to endPosition. Kills the piece at endPosition if it exists.
		/// Throws an InvalidOperationException if this is an invalid move.
		/// </summary>
		/// <param name="startPosition"></param>
		/// <param name="endPosition"></param>
		public void MovePiece(Position startPosition, Position endPosition)
		{
			if (!isMoveValid(startPosition, endPosition))
			{
				throw new InvalidOperationException($"Cannot complete invalid move from {startPosition} to {endPosition}");
			}

			// Kill the piece at the destination, if there is one
			var endPiece = boardMatrix[endPosition.Row, endPosition.Column];
			if (endPiece != null)
			{
				deadPiecesByTeam[endPiece.Team].Add(endPiece);
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

			if (WhoseTurn == ChessTeam.Black)
			{
				WhoseTurn = ChessTeam.White;
			}
			else
			{
				WhoseTurn = ChessTeam.Black;
			}
		}

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

		/// <summary>
		/// Checks if there are any pieces in the way between the start and endPositions.
		/// This check does not include the start and end positions themselves.
		/// </summary>
		/// <param name="startPosition"></param>
		/// <param name="endPosition"></param>
		/// <returns></returns>
		private bool isPathClear(Point2D startPosition, Point2D endPosition)
		{
			var requestedMoveVector = endPosition - startPosition;

			// Increment from the startPosition to the endPosition, checking nothing is in the way
			var unitVector = requestedMoveVector.GetUnitVector();
			var nextPosition = startPosition + unitVector;
			while (nextPosition != endPosition)
			{
				if (boardMatrix[nextPosition.Y, nextPosition.X] != null)
				{
					return false;
				}
				unitVector = (endPosition - nextPosition).GetUnitVector();
				nextPosition = nextPosition + unitVector;
			}

			return true;
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

		public const int Size = 8;

		public ChessTeam WhoseTurn;

		internal ChessPiece[,] boardMatrix; // TODO: This probably shouldn't be internal. Just for debugging for P1.
		internal Dictionary<PieceType, IList<Point2D>> pieceLocationsByType = new Dictionary<PieceType, IList<Point2D>>();
		private IDictionary<ChessTeam, ISet<ChessPiece>> deadPiecesByTeam = new Dictionary<ChessTeam, ISet<ChessPiece>>()
		{
			{ChessTeam.White, new HashSet<ChessPiece>()},
			{ChessTeam.Black, new HashSet<ChessPiece>()}
		};

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
