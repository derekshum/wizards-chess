using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WizardsChess.CommandConversion
{
	public enum CommandType
	{
		Move,
		Reset,
		Undo,
		Yes,
		No,
		ConfirmPiece
	}

	public static class CommandTypeMethods
	{
		public static CommandType Parse(IReadOnlyDictionary<string, IReadOnlyList<string>> commandParams)
		{
			IReadOnlyList<string> paramsList;
			if (commandParams.TryGetValue("action", out paramsList))
			{
				return Parse(paramsList.FirstOrDefault());
			}
			else
			{
				throw new ArgumentException($"Cannot create a Command without an action parameter. Received {commandParams.Count} parameters.");
}
		}

		public static CommandType Parse(string actionStr)
		{
			switch (actionStr.ToLowerInvariant())
			{
				case "move":
					return CommandType.Move;
				case "reset":
					return CommandType.Reset;
				case "undo":
					return CommandType.Undo;
				case "yes":
					return CommandType.Yes;
				case "no":
					return CommandType.No;
				case "piececonfirmation":
					return CommandType.ConfirmPiece;
				default:
					throw new ArgumentException($"Cannot convert string \"{actionStr}\" to an Action enum.");
			}
		}
	}
}
