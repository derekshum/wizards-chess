using System;
using System.Collections;
using System.Collections.Generic;
using WizardsChessApp.Movement;

// Board arranged in A-H, 1-8. where A-H is replaced by 9-16
namespace WizardsChessApp.Game.Pieces {
	abstract class ChessPiece{
		protected PieceType type;

		public ChessPiece(ChessTeam team){
			Team = team;
			HasMoved = false;
			CanJump = false;
		}

		public ChessTeam Team { get; }

		public bool HasMoved { get; set; }

		public bool CanJump { get; protected set; }

		public override string ToString()
		{
			return Team.ToString() + " " + type.ToString();
		}

		public abstract IReadOnlyList<Vector2D> GetAllowedMotionVectors();

		public abstract IReadOnlyList<Vector2D> GetAttackMotionVectors();
	}
}