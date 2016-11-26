using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WizardsChess.Chess;
using WizardsChess.Chess.Pieces;
using WizardsChess.Movement;

namespace WizardsChess.Chess
{
	public class ChessLogic
	{
		public ChessLogic ()
		{
			board = new ChessBoard();
		}

		//Checks for pieces of a certain type that can move to 
		public ISet<Position> FindPotentialPiecesForMove(PieceType piece, Position destination)
		{
			var pieceLocationList = board.PieceLocationsByType[piece];

			var potentialPiecePositions = new HashSet<Position>();

			foreach (var location in pieceLocationList)
			{
				if (IsMoveValid(new Position(location), destination))   //TODO: it requires "isMoveValid"
				{
					potentialPiecePositions.Add(new Position(location));
				}
			}

			return potentialPiecePositions;
		}

		/// <summary>
		/// Moves the piece from startPosition to endPosition. Kills the piece at endPosition if it exists.
		/// Throws an InvalidOperationException if this is an invalid move.
		/// </summary>
		public void MovePiece (Position startPosition, Position endPosition)
		{
			if (!IsMoveValid(startPosition, endPosition))
			{
				throw new InvalidOperationException($"Cannot complete invalid move from {startPosition} to {endPosition}");
			}

			board.MovePiece(startPosition, endPosition);

			if (WhoseTurn == ChessTeam.Black)
			{
				WhoseTurn = ChessTeam.White;
			}
			else
			{
				WhoseTurn = ChessTeam.Black;
			}
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
		public bool IsMoveValid(Position startPosition, Position endPosition)
		{
			// Get piece at input location
			ChessPiece startPiece = board.PieceAt(startPosition);
			ChessPiece endPiece = board.PieceAt(endPosition);

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

		public bool IsMoveValid(Point2D startPoint, Point2D endPoint)
		{
			// Get piece at input location
			ChessPiece startPiece = board.PieceAt(startPoint);
			ChessPiece endPiece = board.PieceAt(endPoint);

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

			var requestedMoveVector = endPoint - startPoint;

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

			return isPathClear(startPoint, endPoint);
		}

		public bool isPathClear(Point2D startPosition, Point2D endPosition)
		{
			var requestedMoveVector = endPosition - startPosition;

			// Increment from the startPosition to the endPosition, checking nothing is in the way
			var unitVector = requestedMoveVector.GetUnitVector();
			var nextPosition = startPosition + unitVector;
			while (nextPosition != endPosition)
			{
				if (board.PieceAt(nextPosition) != null)
				{
					return false;
				}
				unitVector = (endPosition - nextPosition).GetUnitVector();
				nextPosition = nextPosition + unitVector;
			}

			return true;
		}

		//TODO: detect Checkmate

		//TODO: castle

		//TODO: en passant

		// Checks if the player to move's king is in check, overloaded
		public bool inCheck()
		{
			Point2D kingLocation;
			var kingLocations = board.PieceLocationsByType[PieceType.King];
			foreach(var aKingLocation in kingLocations)
			{
				if (board.PieceAt(aKingLocation).Team == WhoseTurn)
				{
					kingLocation = aKingLocation;
				}
			}
			return inCheck(kingLocation, WhoseTurn);
		}

		// Checks if the specified location is in check for the specified team 
		public bool inCheck(Point2D checkPoint, ChessTeam Turn)
		{
			int i, j;
			for (i = 0; i < ChessBoard.Size; i++)
			{
				for (j = 0; j < ChessBoard.Size, j++)
				{
					var piece = board.PieceAt(i, j);
					if (piece != null && piece.Team != Turn)
					{
						if (IsMoveValid(new Point2D(i,j), checkPoint))
						{
							return true;
						}
					}
				}
			}
			return false;	//TODO: remove this
		}

		private ChessBoard board;
		public IChessBoard Board
		{
			get
			{
				return board;
			}
		}
		private ChessTeam WhoseTurn;
	}
}
