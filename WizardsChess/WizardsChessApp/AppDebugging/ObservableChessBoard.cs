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
			ICollection<ObservableChessPiece> copiedCollection = this;
			for (int row = 0; row < ChessBoard.Size; row++)
			{
				for (int col = 0; col < ChessBoard.Size; col++)
				{
					if (board.boardMatrix[row, col] != null)
					{
						var piece = this.First(p => p.Piece == board.boardMatrix[row, col]);
						copiedCollection.Remove(piece);
						piece.UpdatePosition(row, col);
					}
				}
			}
			foreach (var piece in copiedCollection)
			{
				this.Remove(piece);
			}
		}

		private ChessBoard board;
	}
}
