using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WizardsChessApp.Chess;

namespace WizardsChessApp.AppDebugging
{
	public class VisualGameManager
	{
		public VisualGameManager()
		{
			State = "Ok";
			IsError = false;

			chessBoard = new ObservableChessBoard(board);
		}

		public string State { get; private set; }
		public bool IsError { get; }

		public ObservableChessBoard Pieces { get { return chessBoard; } }

		private ObservableChessBoard chessBoard;
		private ChessBoard board = new ChessBoard();
	}
}
