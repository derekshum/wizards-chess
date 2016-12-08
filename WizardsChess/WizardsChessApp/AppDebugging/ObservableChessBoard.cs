using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WizardsChess.Chess;

namespace WizardsChess.AppDebugging
{
	public class ObservableChessBoard : ObservableCollection<ObservableChessPiece>
	{
		public ObservableChessBoard(IChessBoard board, Windows.UI.Core.CoreDispatcher uiDispatcher)
		{
			this.uiDispatcher = uiDispatcher;
			this.board = board;
			for (int row = 0; row < ChessBoard.Size; row++)
			{
				for (int col = 0; col < ChessBoard.Size; col++)
				{
					if (board.PieceAt(col,row) != null)
					{
						Add(new ObservableChessPiece(board.PieceAt(col, row), row, col, uiDispatcher));
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
					if (board.PieceAt(col,row) != null)
					{
						var piece = this.First(p => p.Piece == board.PieceAt(col,row));
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

		private IChessBoard board;
		private Windows.UI.Core.CoreDispatcher uiDispatcher;
	}
}
