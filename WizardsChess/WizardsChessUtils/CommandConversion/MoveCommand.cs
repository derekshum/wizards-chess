using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WizardsChess.Chess;

namespace WizardsChess.CommandConversion
{
	public class MoveCommand : ICommand
	{
		public CommandType Type { get; }
		public PieceType? Piece { get; }
		public Position Position { get; }
		public Position Destination { get; }

		private MoveCommand()
		{
			Type = CommandType.Move;
		}

		public MoveCommand(ICommand command) : this()
		{
			var mvCmd = command as MoveCommand;
			if (mvCmd != null)
			{
				Piece = mvCmd.Piece;
				Position = new Position(mvCmd.Position);
				Destination = new Position(mvCmd.Destination);
			}
		}

		public MoveCommand(IReadOnlyDictionary<string, IReadOnlyList<string>> commandParams) : this()
		{
			IReadOnlyList<string> paramsList;

			if (Type == CommandType.Move)
			{
				if (commandParams.TryGetValue("piece", out paramsList))
				{
					Piece = PieceTypeMethods.Parse(paramsList.FirstOrDefault());
				}
				var destLetter = commandParams["destinationLetter"];
				var destNumber = commandParams["destinationNumber"];
				Destination = new Position(destLetter.FirstOrDefault(), destNumber.FirstOrDefault());
			}

			if (Type == CommandType.Move || Type == CommandType.ConfirmPiece)
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
	}
}
