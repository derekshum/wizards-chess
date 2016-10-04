using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WizardsChessApp.CommandConversion
{
	enum Action
	{
		Move,
		Reset,
		Undo,
		Yes,
		No,
		ConfirmPiece
	}

	static class ActionMethods
	{
		public static Action Parse(string actionStr)
		{
			switch (actionStr)
			{
				case "move":
					return Action.Move;
				case "reset":
					return Action.Reset;
				case "undo":
					return Action.Undo;
				case "yes":
					return Action.Yes;
				case "no":
					return Action.No;
				case "pieceConfirmation":
					return Action.ConfirmPiece;
				default:
					throw new ArgumentException($"Cannot convert string \"{action}\" to an Action enum.");
			}
		}
	}
}
