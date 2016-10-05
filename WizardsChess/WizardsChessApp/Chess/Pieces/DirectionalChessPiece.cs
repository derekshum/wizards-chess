using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WizardsChessApp.Movement;

namespace WizardsChessApp.Chess.Pieces
{
	abstract class DirectionalChessPiece : ChessPiece
	{
		public DirectionalChessPiece(ChessTeam team) : base(team)
		{
			switch (team)
			{
				case ChessTeam.White:
					ForwardDirection = WhiteForwardDirection;
					break;
				case ChessTeam.Black:
					ForwardDirection = BlackForwardDirection;
					break;
				default:
					// Default is impossible
					break;
			}
		}

		public Vector2D ForwardDirection { get; }

		protected static readonly Vector2D WhiteForwardDirection = new Vector2D(0, 1);
		protected static readonly Vector2D BlackForwardDirection = -WhiteForwardDirection;
	}
}
