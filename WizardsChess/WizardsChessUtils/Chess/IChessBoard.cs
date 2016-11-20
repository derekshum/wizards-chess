using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WizardsChess.Chess.Pieces;
using WizardsChess.Movement;

namespace WizardsChess.Chess
{
	public interface IChessBoard
	{
		ChessPiece PieceAt(int x, int y);
		ChessPiece PieceAt(Point2D location);
		ChessPiece PieceAt(Position location);
		int NumCapturedPieces(ChessTeam team);
		IDictionary<ChessTeam, ISet<ChessPiece>> capturedPiecesByTeam { get; }

	}
}
