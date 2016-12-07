using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WizardsChess.VoiceControl.Commands
{
	public enum CommandType
	{
		Move,
		Reset,
		Undo,
		Yes,
		No,
		ConfirmPiece,
		Cancel,
		MotorMove,
		Magnet,
		Castle
	}

	public static class CommandTypeMethods
	{
		public static CommandFamily GetFamily(this CommandType type)
		{
			switch (type)
			{
				case CommandType.Move:
				case CommandType.Reset:
				case CommandType.Undo:
					return CommandFamily.Move;
				case CommandType.ConfirmPiece:
					return CommandFamily.PieceConfirmation;
				case CommandType.Yes:
				case CommandType.No:
					return CommandFamily.YesNo;
				case CommandType.MotorMove:
				case CommandType.Magnet:
					return CommandFamily.Debug;
				case CommandType.Castle:
				default:
					return CommandFamily.Other;
			}
		}
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
				case "cancel":
					return CommandType.Cancel;
				case "motormove":
					return CommandType.MotorMove;
				case "magnet":
					return CommandType.Magnet;
				case "castle":
					return CommandType.Castle;
				default:
					throw new ArgumentException($"Cannot convert string \"{actionStr}\" to an Action enum.");
			}
		}
	}
}
