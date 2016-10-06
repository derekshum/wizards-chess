using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WizardsChessApp.Movement;

namespace WizardsChessApp.Chess.Pieces
{
	class Pawn : DirectionalChessPiece
	{
		public Pawn(ChessTeam team) : base(team)
		{
			Type = PieceType.Pawn;
		}

		// contains motions of allowed chess piece
		private static IReadOnlyList<Vector2D> allowedMotionVectors = new List<Vector2D>()
		{
			new Vector2D(0, 1)
		};
		// contains special motion vectors where additional check is required
		private static IReadOnlyList<Vector2D> specialMotionVectors = new List<Vector2D>()
		{
			new Vector2D(0, 2)
		};

		// contains attack motion vectors
		private static IReadOnlyList<Vector2D> attackMotionVectors = new List<Vector2D>()
		{
			new Vector2D(1, 1),
			new Vector2D(-1, 1)
		};

		public override IReadOnlyList<Vector2D> GetAllowedMotionVectors()
		{
			var allowedMoves = new List<Vector2D>(allowedMotionVectors);
			if (!HasMoved)
			{
				allowedMoves.AddRange(specialMotionVectors);
			}

			if (ForwardDirection == WhiteForwardDirection)
			{
				return allowedMoves;
			}
			else if (ForwardDirection == BlackForwardDirection)
			{
				return allowedMoves.Select(v => v.FlipY()).ToList();
			}
			else
			{
				throw new InvalidOperationException($"Tried to get allowed moves for {nameof(Pawn)} when ForwardDirection was {ForwardDirection}");
			}
		}

		public override IReadOnlyList<Vector2D> GetAttackMotionVectors()
		{
			if (ForwardDirection == WhiteForwardDirection)
			{
				return attackMotionVectors;
			}
			else if (ForwardDirection == BlackForwardDirection)
			{
				return attackMotionVectors.Select(v => v.FlipY()).ToList();
			}
			else
			{
				throw new InvalidOperationException($"Tried to get attack moves for {nameof(Pawn)} when ForwardDirection was {ForwardDirection}");
			}
		}

	}
}
