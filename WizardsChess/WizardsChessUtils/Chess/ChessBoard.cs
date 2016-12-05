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
		/// Throws an InvalidOperationException if this is an invalid move.	//TODO: take this out once it's no longer done here
		/// </summary>
		/// <param name="startPosition"></param>
		/// <param name="endPosition"></param>
		public void MovePiece(Position startPosition, Position endPosition)
		{
			MoveSpecification move = new MoveSpecification(startPosition, endPosition);
			// Kill the piece at the destination, if there is one
			var endPiece = PieceAt(endPosition);	//TODO: could be replaced with boardMatrix reference
			if (endPiece != null)
			{
				move.Capture = endPosition;
				capturedPiecesByTeam[endPiece.Team].Add(endPiece);
				// Remove a killed piece from our valid pieceLocationsByType list
				var listOfEndPieceType = pieceLocationsByType[endPiece.Type];
				listOfEndPieceType.Remove(endPosition);
			} //TODO: else if en passant
			var startPiece = PieceAt(startPosition);	//TODO: could be replaced with boardMatrix reference
			startPiece.HasMoved = true;
			SetPieceAt(endPosition, startPiece);
			SetPieceAtToNull(startPosition);

			var listOfStartPieceTypes = pieceLocationsByType[startPiece.Type];
			// Replace the old position for this piece with the new position in the pieceLocationsByType list
			listOfStartPieceTypes.Remove(startPosition);
			listOfStartPieceTypes.Add(endPosition);
			pastMoves.Add(move);
			changeTurn();
		}

		public void Castle(Point2D rookPos)
		{
			MoveSpecification move = new MoveSpecification(new Position(rookPos), new Position(1,1), null, true);
			int rookDir = Math.Sign(rookPos.X - KingCol);
			boardMatrix[rookPos.Y, KingCol].HasMoved = true;
			SetPieceAt(new Point2D(KingCol + 2 * rookDir, rookPos.Y), PieceAt(KingCol, rookPos.Y));
			SetPieceAtToNull(new Point2D(KingCol, rookPos.Y));

			var listOfStartPieceTypes = pieceLocationsByType[PieceType.King];
			listOfStartPieceTypes.Remove(new Position(KingCol, rookPos.Y));
			listOfStartPieceTypes.Add(new Position(KingCol + 2 * rookDir, rookPos.Y));

			boardMatrix[rookPos.Y, rookPos.X].HasMoved = true;
			SetPieceAt(new Point2D(KingCol + rookDir, rookPos.Y), PieceAt(rookPos));    //TODO: PieceAt could be replaced with boardMatrix reference
			SetPieceAtToNull(rookPos);

			listOfStartPieceTypes = pieceLocationsByType[PieceType.Rook];
			listOfStartPieceTypes.Remove(rookPos);
			listOfStartPieceTypes.Add(new Position(KingCol + rookDir, rookPos.Y));

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
				//TODO: write castling undos
			}
			else
			{
				SetPieceAt(lastMove.Start, PieceAt(lastMove.End));  //TODO: PieceAt could be replaced with boardMatrix reference
				SetPieceAtToNull(lastMove.End);
				if (lastMove.Capture != null)
				{
					SetPieceAt((Position)lastMove.Capture, capturedPiecesByTeam[WhoseTurn][capturedPiecesByTeam[WhoseTurn].Count - 1]);
					capturedPiecesByTeam[WhoseTurn].RemoveAt(capturedPiecesByTeam.Count - 1);
				}
				pastMoves.RemoveAt(pastMoves.Count - 1);
				changeTurn();
			}
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
	}
}
