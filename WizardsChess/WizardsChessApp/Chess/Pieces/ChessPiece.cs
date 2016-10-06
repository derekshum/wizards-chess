using System;
using System.Collections;
using System.Collections.Generic;
using WizardsChessApp.Movement;

// Board arranged in A-H, 1-8. where A-H is replaced by 9-16
namespace WizardsChessApp.Chess.Pieces {
	public abstract class ChessPiece{

		public ChessPiece(ChessTeam team){
			Team = team;
			HasMoved = false;
			CanJump = false;
		}

		public PieceType Type { get; protected set; }

		public ChessTeam Team { get; }

		public bool HasMoved { get; set; }

		public bool CanJump { get; protected set; }

		public virtual string ToShortString()
		{
			return /*Team.ToString().Substring(0, 1) + "-" + */Type.ToString().Substring(0, 1);
		}

		public override string ToString()
		{
			return Team.ToString() + "-" + Type.ToString();
		}

		public abstract IReadOnlyList<Vector2D> GetAllowedMotionVectors();

		public abstract IReadOnlyList<Vector2D> GetAttackMotionVectors();
	}
}