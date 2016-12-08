using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WizardsChess.Chess
{
	public enum PieceType
	{
		King,
		Queen,
		Knight,
		Bishop,
		Rook,
		Pawn
	}

	public static class PieceTypeMethods
	{
		public static PieceType Parse(string pieceName)
		{
			if (pieceName.Equals("castle", StringComparison.OrdinalIgnoreCase))
			{
				return PieceType.Rook;
			}
			var piece = Enum.Parse(typeof(PieceType), pieceName, true) as PieceType?;
			if (piece == null)
			{
				throw new ArgumentException($"Could not convert string \"{pieceName}\" to a {nameof(PieceType)} enum.");
			}
			return piece.Value;
		}
	}
	
}
