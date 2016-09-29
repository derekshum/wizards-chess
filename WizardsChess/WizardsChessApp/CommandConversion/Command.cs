using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WizardsChessApp.CommandConversion
{
	class Command
	{
		public Command(Command command)
		{
			Action = command.Action;
		}

		public Command(IReadOnlyDictionary<string, IReadOnlyList<string>> commandParams)
		{
			IReadOnlyList<string> paramsList;
			if (commandParams.TryGetValue("action", out paramsList))
			{
				Action = getAction(paramsList.FirstOrDefault());
			}
			else
			{
				throw new ArgumentException("Cannot create a Command without an action parameter");
			}
		}

		public Action Action { get; set; }

		private Action getAction(string action)
		{
			switch (action)
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
