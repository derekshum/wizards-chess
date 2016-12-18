using System;
using System.Collections;
using System.Collections.Generic;
using WizardsChess.Movement;

// Board arranged in A-H, 1-8. where A-H is replaced by 9-16
namespace WizardsChess.Chess.Pieces {
	public abstract class ChessPiece{

		public ChessPiece(ChessTeam team){
			Team = team;
			HasMoved = false;
			CanJump = false;
		}

		/*
		public ChessPiece(ChessPiece piece)
		{
			Type = piece.Type;
			Team = piece.Team;
			HasMoved = piece.HasMoved;
			CanJump = piece.CanJump;
		}
		*/

		public abstract ChessPiece DeepCopy();

		public PieceType Type { get; protected set; }

		public ChessTeam Team { get; }

		public bool HasMoved { get; set; }

		public bool CanJump { get; protected set; }

		public virtual string ToShortString()
		{
			string piece = Type.ToString().Substring(0, 1);
			if (Team == ChessTeam.White)
			{
				return piece + "w";
			}
			else if (Team == ChessTeam.Black)
			{
				return piece + "b";
			}
			else
			{
				return piece;
			}
		}

		public override string ToString()
		{
			return Team.ToString() + "-" + Type.ToString();
		}

		public abstract IReadOnlyList<Vector2D> GetAllowedMotionVectors();

		public abstract IReadOnlyList<Vector2D> GetAttackMotionVectors();
	}
}