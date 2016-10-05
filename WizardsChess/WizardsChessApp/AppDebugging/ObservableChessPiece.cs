using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media;
using WizardsChessApp.Chess.Pieces;

namespace WizardsChessApp.AppDebugging
{
	public class ObservableChessPiece
	{
		public ObservableChessPiece(ChessPiece piece, int row, int col)
		{
			Piece = piece;
			Symbol = Piece.ToShortString();
			switch (piece.Team)
			{
				case Chess.ChessTeam.Black:
					Color = new SolidColorBrush(Windows.UI.Colors.Black);
					break;
				case Chess.ChessTeam.White:
					Color = new SolidColorBrush(Windows.UI.Colors.White);
					break;
				default:
					break;
			}
			UpdatePosition(row, col);
		}

		public string Symbol { get; }

		public ChessPiece Piece { get; }

		public SolidColorBrush Color { get; }
		
		public int GridRow { get; private set; }
		public int GridColumn { get; private set; }

		public void UpdatePosition(int row, int col)
		{
			GridRow = 7 - row;
			GridColumn = col;
		}
	}
}
