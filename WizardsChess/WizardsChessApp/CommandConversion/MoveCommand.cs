using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WizardsChessApp.CommandConversion
{
	class MoveCommand : Command
	{
		public MoveCommand(Command command) : base(command)
		{
			var mvCmd = command as MoveCommand;
			if (mvCmd != null)
			{
				Piece = mvCmd.Piece;
				Position = new Position(mvCmd.Position);
				Destination = new Position(mvCmd.Destination);
			}
		}

		public MoveCommand(IReadOnlyDictionary<string, IReadOnlyList<string>> commandParams) : base(commandParams)
		{
			IReadOnlyList<string> paramsList;

			if (Action == Action.Move)
			{
				if (commandParams.TryGetValue("piece", out paramsList))
				{
					Piece = getPiece(paramsList.FirstOrDefault());
				}
				var destLetter = commandParams["destinationLetter"];
				var destNumber = commandParams["destinationNumber"];
				Destination = new Position(destLetter.FirstOrDefault(), destNumber.FirstOrDefault());
			}

			if (Action == Action.Move || Action == Action.ConfirmPiece)
			{
				string posLetter = null;
				string posNumber = null;
				if (commandParams.TryGetValue("positionLetter", out paramsList))
				{
					posLetter = paramsList.FirstOrDefault();
				}
				if (commandParams.TryGetValue("positionNumber", out paramsList))
				{
					posNumber = paramsList.FirstOrDefault();
				}
				if (!String.IsNullOrWhiteSpace(posLetter) && !String.IsNullOrWhiteSpace(posNumber))
				{
					Position = new Position(posLetter, posNumber);
				}
			}
		}

		public bool IsMoveCommand(IReadOnlyDictionary<string, IReadOnlyList<string>> commandParams)
		{
			IReadOnlyList<string> paramsList;
			if (commandParams.TryGetValue("action", out paramsList))
			{
				return paramsList.FirstOrDefault() == "action";
			}
			return false;
		}

		public Piece? Piece { get; set; }
		public Position Position { get; set; }
		public Position Destination { get; set; }

		private Piece getPiece(string piece)
		{
			switch (piece)
			{
				case "king":
					return CommandConversion.Piece.King;
				case "queen":
					return CommandConversion.Piece.Queen;
				case "knight":
					return CommandConversion.Piece.Knight;
				case "bishop":
					return CommandConversion.Piece.Bishop;
				case "castle":
					return CommandConversion.Piece.Castle;
				case "pawn":
					return CommandConversion.Piece.Pawn;
				default:
					throw new ArgumentException($"Could not convert string \"{piece}\" to a Piece enum.");
			}
		}
	}
}
