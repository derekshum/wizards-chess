using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WizardsChess.Movement;

namespace WizardsChess.Chess.Pieces
{
	class King : ChessPiece
	{
		public King(ChessTeam team) : base(team)
		{
			Type = PieceType.King;
		}

		public override ChessPiece DeepCopy()
		{
			King piece = new King(this.Team);
			piece.HasMoved = this.HasMoved;
			piece.HasMoved = this.HasMoved;
			piece.CanJump = this.CanJump;
			return piece;
		}

		private static IReadOnlyList<Vector2D> allowedMotionVectors = new List<Vector2D>()
		{
			new Vector2D(-1,-1),
			new Vector2D(-1, 0),
			new Vector2D(-1, 1),
			new Vector2D(0, -1),
			new Vector2D(0, 1),
			new Vector2D(1, -1),
			new Vector2D(1, 0),
			new Vector2D(1, 1)
		};

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
