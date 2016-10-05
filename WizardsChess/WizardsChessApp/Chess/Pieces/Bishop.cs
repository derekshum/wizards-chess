using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WizardsChessApp.Movement;

namespace WizardsChessApp.Chess.Pieces
{
	class Bishop : ChessPiece
	{
		public Bishop(ChessTeam team) : base(team)
		{
			type = PieceType.Bishop;
			if (allowedMotionVectors.Count == 0)
			{
				for (int move = 1; move <= ChessBoard.Size; move++)
				{
					allowedMotionVectors.Add(new Vector2D(move, move));
					allowedMotionVectors.Add(new Vector2D(move, -move));
					allowedMotionVectors.Add(new Vector2D(-move, move));
					allowedMotionVectors.Add(new Vector2D(-move, -move));
				}
			}
		}

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
