using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WizardsChessApp.Chess;

namespace WizardsChessApp.CommandConversion
{
	public class MoveCommand : Command
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
					Piece = PieceTypeMethods.Parse(paramsList.FirstOrDefault());
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

		public PieceType? Piece { get; set; }
		public Position Position { get; set; }
		public Position Destination { get; set; }
	}
}
