using System;
using System.Collections;
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
			capturedPiecesByTeam[ChessTeam.White] = new List<ChessPiece>();
			capturedPiecesByTeam[ChessTeam.Black] = new List<ChessPiece>();
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

		/// <summary>
		/// Moves the piece from startPosition to endPosition. Kills the piece at endPosition if it exists.
		/// </summary>
		/// <param name="startPosition"></param>
		/// <param name="endPosition"></param>
		public void MovePiece(Position startPosition, Position endPosition)
		{
			MoveSpecification move = new MoveSpecification(startPosition, endPosition);
			
			// Kill the piece at the destination, if there is one
			var endPiece = internalPieceAt(endPosition);
			if (endPiece != null)
			{
#if DEBUG
				changeTurn();
				System.Diagnostics.Debug.WriteLine("num in captured before capture on captured team " + WhoseTurn + " is " + capturedPiecesByTeam[WhoseTurn].Count);
				if (capturedPiecesByTeam[WhoseTurn].Count > 0)
					System.Diagnostics.Debug.WriteLine("piece at count - 1 before this capture: " + capturedPiecesByTeam[WhoseTurn][capturedPiecesByTeam[WhoseTurn].Count - 1].ToShortString());
				foreach (var piece in capturedPiecesByTeam[WhoseTurn])
				{
					System.Diagnostics.Debug.Write(piece.ToShortString() + ", ");
				}
				System.Diagnostics.Debug.WriteLine("are the pieces captured for " + WhoseTurn);
				changeTurn();
#endif
				move.Capture = endPosition;
				capturedPiecesByTeam[endPiece.Team].Add(endPiece);
				
				// Remove a killed piece from our valid pieceLocationsByType list
				var listOfEndPieceType = pieceLocationsByType[endPiece.Type];
				listOfEndPieceType.Remove(new Point2D(endPosition));
#if DEBUG
				changeTurn();
				System.Diagnostics.Debug.WriteLine("num in captured after capture on captured team " + WhoseTurn + " is " + capturedPiecesByTeam[WhoseTurn].Count);
				System.Diagnostics.Debug.WriteLine("piece at count - 1 after this capture: " + capturedPiecesByTeam[WhoseTurn][capturedPiecesByTeam[WhoseTurn].Count - 1].ToShortString());
				foreach (var piece in capturedPiecesByTeam[WhoseTurn])
				{
					System.Diagnostics.Debug.Write(piece.ToShortString() + ", ");
				}
				System.Diagnostics.Debug.WriteLine("are the pieces captured for " + WhoseTurn);
				changeTurn();
#endif
			} //TODO: else if en passant

			var movingPiece = internalPieceAt(startPosition);
			//Move the moving piece in boardMatrix
			SetPieceAt(endPosition, movingPiece);
			SetPieceAtToNull(startPosition);

			if (!movingPiece.HasMoved)  //note changes if this is a piece's first move in it and in pastMoves
			{
				movingPiece.HasMoved = true;
				move.HasMovedChange = true;
			}
			// Replace the old position for this piece with the new position in the pieceLocationsByType list
			var listOfMovingPieceType = pieceLocationsByType[movingPiece.Type];
			listOfMovingPieceType.Remove(new Point2D(startPosition));
			listOfMovingPieceType.Add(new Point2D(endPosition));

			pastMoves.Add(move);
			changeTurn();
		}

		public void Castle(Point2D initialRookPoint)
		{
			int rookDir = Math.Sign(initialRookPoint.X - KingCol);	//calculates the direction of the rook from the king initially

			//Move the King in boardMatrix
			Position initialKingPos = new Position(KingCol, initialRookPoint.Y);
			var king = internalPieceAt(initialKingPos);
			Position finalKingPos = new Position(KingCol + 2 * rookDir, initialRookPoint.Y);
			SetPieceAt(finalKingPos, king);
			SetPieceAtToNull(initialKingPos);

			king.HasMoved = true;    //update King's HasMoved
			//Replace the old position for the King with the new position in the pieceLocationsByType list
			var listOfPieceType = pieceLocationsByType[PieceType.King];
			listOfPieceType.Remove(new Point2D(initialKingPos));
			listOfPieceType.Add(new Point2D(finalKingPos));

			//Move this Rook in boardMatrix
			var rook = internalPieceAt(initialRookPoint);
			Position finalRookPos = new Position(KingCol + rookDir, initialRookPoint.Y);
			SetPieceAt(finalRookPos, rook);
			SetPieceAtToNull(initialRookPoint);

			rook.HasMoved = true;  //update Rook's HasMoved
			//Replace the old position for this Rook with the new position in the pieceLocationsByType list
			listOfPieceType = pieceLocationsByType[PieceType.Rook];
			listOfPieceType.Remove(initialRookPoint);
			listOfPieceType.Add(new Point2D(finalRookPos));	//TODO: figure out if this creation is unnecessary and casting is done automatically.

			MoveSpecification move = new MoveSpecification(new Position(initialRookPoint), finalRookPos, null, true, true);
			pastMoves.Add(move);
			changeTurn();
		}
		public void changeTurn()
		{
			if (WhoseTurn == ChessTeam.Black)
			{
				WhoseTurn = ChessTeam.White;
			}
			else
			{
				WhoseTurn = ChessTeam.Black;
			}
		}

		//undoes the last move (can be called repeatedly)
		public void UndoMove()
		{
			MoveSpecification lastMove = pastMoves[pastMoves.Count - 1];
			if (lastMove.Castle)
			{
				undoCastle(lastMove);
			}
			else //not a castle
			{
				undoRegularMove(lastMove);
			}
			pastMoves.RemoveAt(pastMoves.Count - 1);
			changeTurn();
		}

		//undoes non-castle moves
		private void undoRegularMove(MoveSpecification lastMove)
		{
			var movingPiece = internalPieceAt(lastMove.End);    //get the move that is being undone
			if (lastMove.HasMovedChange)    //set a piece to unmoved if this was a move that changed that status
			{
				movingPiece.HasMoved = false;
			}

			//Perform reverse move upon boardMatrix
			SetPieceAt(lastMove.Start, internalPieceAt(lastMove.End));
			SetPieceAtToNull(lastMove.End);

			//Replace the old position for this piece with the new position in the pieceLocationsByType list
			var listOfMovingPieceType = pieceLocationsByType[internalPieceAt(lastMove.Start).Type];
			listOfMovingPieceType.Remove(lastMove.End);
			listOfMovingPieceType.Add(lastMove.Start);

			if (lastMove.Capture != null)   //if move being undone has a capture location, restore the most recently taken other team's piece to that location
			{
#if DEBUG
				System.Diagnostics.Debug.WriteLine("num in captured before restore for " + WhoseTurn + " is " + capturedPiecesByTeam[WhoseTurn].Count);
				System.Diagnostics.Debug.WriteLine("restored captured piece: " + capturedPiecesByTeam[WhoseTurn][capturedPiecesByTeam[WhoseTurn].Count - 1].ToShortString());
				foreach (var piece in capturedPiecesByTeam[WhoseTurn])
				{
					System.Diagnostics.Debug.Write(piece.ToShortString() + ", ");
				}
				System.Diagnostics.Debug.WriteLine("are the pieces captured for " + WhoseTurn);
#endif
				SetPieceAt((Position)lastMove.Capture, capturedPiecesByTeam[WhoseTurn][capturedPiecesByTeam[WhoseTurn].Count - 1]); //restores captured piece to boardMatrix
#if DEBUG
				System.Diagnostics.Debug.WriteLine("num in captured mid restore for " + WhoseTurn + " is " + capturedPiecesByTeam[WhoseTurn].Count);
				System.Diagnostics.Debug.WriteLine("piece being restored is " + capturedPiecesByTeam[WhoseTurn][capturedPiecesByTeam[WhoseTurn].Count - 1]);
				System.Diagnostics.Debug.Write(ToString());
#endif
				capturedPiecesByTeam[WhoseTurn].RemoveAt(capturedPiecesByTeam[WhoseTurn].Count - 1);   //removes restored captured piece from the list of capturedPiecesByTeam for WhoseTurn, which hasn't been changed yet

				// Add a captured piece to our valid pieceLocationsByType list
				var listOfCapturedPieceType = pieceLocationsByType[internalPieceAt((Position)lastMove.Capture).Type];
				listOfCapturedPieceType.Add((Position)lastMove.Capture);
#if DEBUG
				System.Diagnostics.Debug.WriteLine("num in captured after restore for " + WhoseTurn + " is " + capturedPiecesByTeam[WhoseTurn].Count);
				if (capturedPiecesByTeam[WhoseTurn].Count > 0)
					System.Diagnostics.Debug.WriteLine("piece at count - 1: " + capturedPiecesByTeam[WhoseTurn][capturedPiecesByTeam[WhoseTurn].Count - 1].ToShortString());
				foreach (var piece in capturedPiecesByTeam[WhoseTurn])
				{
					System.Diagnostics.Debug.Write(piece.ToShortString() + ", ");
				}
				System.Diagnostics.Debug.WriteLine("are the pieces captured for " + WhoseTurn);
#endif
			}

		}

		//undoes castle moves
		private void undoCastle(MoveSpecification lastMove)
		{
			int rookSideDir = Math.Sign(lastMove.Start.Column - KingCol);    //calculates the direction of the rook from the king initially

			//Move King back in BoardMatrix
			Position movedKingPos = new Position(KingCol + 2 * rookSideDir, lastMove.Start.Row);
			var king = internalPieceAt(movedKingPos);
			Position unMovedKingPos = new Position(KingCol, lastMove.Start.Row);
			SetPieceAt(unMovedKingPos, king);
			SetPieceAtToNull(movedKingPos);

			king.HasMoved = false;  //set King HasMoved to false
									//Update King Location in pieceLocationsByType
			var listOfPieceType = pieceLocationsByType[PieceType.King];
			listOfPieceType.Remove(movedKingPos);
			listOfPieceType.Add(unMovedKingPos);

			//Move Rook back
			var rook = internalPieceAt(lastMove.End);
			SetPieceAt(new Point2D(lastMove.Start), rook);
			SetPieceAtToNull(new Point2D(lastMove.End));

			rook.HasMoved = false;  //set Rook HasMoved to false
									//Update Rook Location in pieceLocationsByType
			listOfPieceType = pieceLocationsByType[PieceType.Rook];
			listOfPieceType.Remove(new Point2D(lastMove.End));
			listOfPieceType.Add(new Point2D(lastMove.Start));
		}

		//TODO: board reset

		private void SetPieceAt(Position p, ChessPiece piece)
		{
			boardMatrix[p.Row, p.Column] = piece;
		}

		private void SetPieceAt(Point2D p2, ChessPiece piece)
		{
			boardMatrix[p2.Y, p2.X] = piece;
		}

		private void SetPieceAtToNull(Position p)
		{
			boardMatrix[p.Row, p.Column] = null;
		}

		private void SetPieceAtToNull(Point2D p2)
		{
			boardMatrix[p2.Y, p2.X] = null;
		}


		//Prints a representation of the board.
		public override string ToString()
		{
			int presentedRow;
			StringBuilder strBuild = new StringBuilder();
			strBuild.Append("\n\tA\tB\tC\tD\tE\tF\tG\tH\n");
			for (int row = Size - 1; row >= 0; row--)
			{
				presentedRow = row + 1;
				strBuild.Append(presentedRow).Append("\t");
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
		public ChessPiece internalPieceAt(int x, int y)
		{
			return boardMatrix[y, x];
		}

		//piece accessor by Point2D
		public ChessPiece internalPieceAt(Point2D point)
		{
			return boardMatrix[point.Y, point.X];
		}

		//piece accessor by Position
		private ChessPiece internalPieceAt(Position pos)
		{
			return boardMatrix[pos.Row, pos.Column];
		}

		//piece accessor by x and y indexes
		public ChessPiece PieceAt(int x, int y)
        {
			if (boardMatrix[y,x] != null)
			{
				return boardMatrix[y, x].DeepCopy();
			}
			else
			{
				return null;
			}
		}

		//piece accessor by Point2D
        public ChessPiece PieceAt(Point2D point)
        {
			if (boardMatrix[point.Y, point.X] != null)
			{
				return boardMatrix[point.Y, point.X].DeepCopy();
			}
			else
			{
				return null;
			}
		}

		//piece accessor by Position
		public ChessPiece PieceAt(Position pos)
		{
			if (boardMatrix[pos.Row, pos.Column] != null) {
				return boardMatrix[pos.Row, pos.Column].DeepCopy();
			}
			else
			{
				return null;
			}
		}
		
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
		private IDictionary<ChessTeam, IList<ChessPiece>> capturedPiecesByTeam = new Dictionary<ChessTeam, IList<ChessPiece>>();
		public IDictionary<ChessTeam, IList<ChessPiece>> CapturedPiecesByTeam
		{
			get
			{
				var temp = capturedPiecesByTeam;
				return temp;
			}
		}
		private List<MoveSpecification> pastMoves = new List<MoveSpecification>();
		public IList<MoveSpecification> PastMoves
		{
			get
			{
				var temp = pastMoves;
				return temp;
			}
		}
		public const int WhiteBackRow = 0;
		public const int WhiteFrontRow = 1;
		public const int BlackBackRow = 7;
		public const int BlackFrontRow = 6;
		public const int LeftRookCol = 0;
		public const int LeftKnightCol = 1;
		public const int LeftBishopCol = 2;
		public const int KingCol = 3;
		public const int QueenCol = 4;
		public const int RightBishopCol = 5;
		public const int RightKnightCol = 6;
		public const int RightRookCol = 7;
		public int GetWhiteBackRow()
		{
			return WhiteBackRow;
		}
		public int GetWhiteFrontRow()
		{
			return WhiteFrontRow;
		}
		public int GetBlackBackRow()
		{
			return BlackBackRow;
		}
		public int GetBlackFrontRow()
		{
			return BlackFrontRow;
		}
		public int GetLeftRookCol()
		{
			return LeftRookCol;
		}
		public int GetLeftKnightCol()
		{
			return LeftKnightCol;
		}
		public int GetLeftBishopCol()
		{
			return LeftBishopCol;
		}
		public int GetKingCol()
		{
			return KingCol;
		}
		public int GetQueenCol()
		{
			return QueenCol;
		}
		public int GetRightBishopCol()
		{
			return RightBishopCol;
		}
		public int GetRightKnightCol()
		{
			return RightKnightCol;
		}
		public int GetRightRookCol()
		{
			return RightRookCol;
		}
	}
}
