﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WizardsChess.Chess.Pieces;
using WizardsChess.Movement;

namespace WizardsChess.Chess
{
	class ChessLogic
	{
		public ChessLogic (ChessBoard b)
		{
			board = b;
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

		private bool isPathClear(Point2D startPosition, Point2D endPosition)
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

		ChessBoard board;
		private ChessTeam WhoseTurn;	//TODO: verify the encapsulation of this

	}
}
