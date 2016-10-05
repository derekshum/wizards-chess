using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WizardsChessApp.Movement;

namespace WizardsChessApp.Chess.Pieces
{
	class Knight : ChessPiece
	{
		public Knight(ChessTeam team) : base(team)
		{
			type = PieceType.Knight;
			CanJump = true;
		}

		public static IReadOnlyList<Vector2D> allowedMotionVectors = new List<Vector2D>()
		{
			new Vector2D(-2, 1),
			new Vector2D(-2, -1),
			new Vector2D(2, 1),
			new Vector2D(2, -1),
			new Vector2D(1, 2),
			new Vector2D(1, -2),
			new Vector2D(-1, 2),
			new Vector2D(-1, -2)
		};

		public override string ToShortString()
		{
			{
				return "Kn";
			}
		}

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
