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
		Dictionary<PieceType, IList<Point2D>> PieceLocationsByType { get; }
		IDictionary<ChessTeam, IList<ChessPiece>> CapturedPiecesByTeam { get; }
		IList<MoveSpecification> PastMoves { get; }
		int GetWhiteBackRow();
		int GetWhiteFrontRow();
		int GetBlackBackRow();
		int GetBlackFrontRow();
		int GetLeftRookCol();
		int GetLeftKnightCol();
		int GetLeftBishopCol();
		int GetKingCol();
		int GetQueenCol();
		int GetRightBishopCol();
		int GetRightKnightCol();
		int GetRightRookCol();
	}
}
