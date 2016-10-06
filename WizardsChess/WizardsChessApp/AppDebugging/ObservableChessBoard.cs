using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WizardsChessApp.Chess;

namespace WizardsChessApp.AppDebugging
{
	public class ObservableChessBoard : ObservableCollection<ObservableChessPiece>
	{
		public ObservableChessBoard(ChessBoard board)
		{
			this.board = board;
			for (int row = 0; row < ChessBoard.Size; row++)
			{
				for (int col = 0; col < ChessBoard.Size; col++)
				{
					if (board.boardMatrix[row,col] != null)
					{
						Add(new ObservableChessPiece(board.boardMatrix[row, col], row, col));
					}
				}
			}
		}

		public void UpdatePieceLocations()
		{
			ObservableChessPiece[] remainingPieces = new ObservableChessPiece[ChessBoard.Size * 4];
			int pieceCounter = 0;
			for (int row = 0; row < ChessBoard.Size; row++)
			{
				for (int col = 0; col < ChessBoard.Size; col++)
				{
					if (board.boardMatrix[row, col] != null)
					{
						var piece = this.First(p => p.Piece == board.boardMatrix[row, col]);
						piece.UpdatePosition(row, col);
						remainingPieces[pieceCounter++] = piece;
					}
				}
			}

			if (pieceCounter != this.Count)
			{
				for (int i = 0; i < this.Count; i++)
				{
					if (!remainingPieces.Contains(this[i]))
					{
						this.RemoveItem(i);
					}
				}
			}
		}

		private ChessBoard board;
	}
}
