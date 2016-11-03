using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WizardsChess.Movement;

namespace WizardsChess.Chess.Pieces
{
	class Rook : ChessPiece
	{
		public Rook(ChessTeam team) : base(team)
		{
			Type = PieceType.Rook;

			if (allowedMotionVectors.Count == 0)
			{
				for (int move = 1; move <= ChessBoard.Size; move++)
				{
					allowedMotionVectors.Add(new Vector2D(move, 0));
					allowedMotionVectors.Add(new Vector2D(-move, 0));
					allowedMotionVectors.Add(new Vector2D(0, move));
					allowedMotionVectors.Add(new Vector2D(0, -move));
				}
			}
		}

		// contains motions of allowed chess piece
		private static List<Vector2D> allowedMotionVectors = new List<Vector2D>();

		public override IReadOnlyList<Vector2D> GetAllowedMotionVectors()
		{
			return allowedMotionVectors;
		}

		public override IReadOnlyList<Vector2D> GetAttackMotionVectors()
		{
			return allowedMotionVectors;
		}
	}
}
