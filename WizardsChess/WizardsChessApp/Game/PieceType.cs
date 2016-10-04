using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WizardsChessApp.Game
{
	enum PieceType
	{
		King,
		Queen,
		Knight,
		Bishop,
		Rook,
		Pawn
	}

	static class PieceTypeMethods
	{
		public static PieceType Parse(string pieceName)
		{
			var piece = Enum.Parse(typeof(PieceType), pieceName, true) as PieceType?;
			if (piece == null)
			{
				throw new ArgumentException($"Could not convert string \"{pieceName}\" to a {nameof(PieceType)} enum.");
			}
			return piece.Value;
		}
	}
	
}
