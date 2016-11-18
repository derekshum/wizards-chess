using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WizardsChess.Chess;

namespace WizardsChess.VoiceControl.Commands
{
	public class MoveCommand : ICommand
	{
		public CommandType Type { get; }
		public PieceType? Piece { get; }
		public Position? Position { get; set; }
		public Position Destination { get; }
		public bool PositionUsedNatoAlphabet { get; }
		public bool DestinationUsedNatoAlphabet { get; }

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
				if (mvCmd.Position.HasValue)
				{
					Position = new Position(mvCmd.Position.Value);
					PositionUsedNatoAlphabet = mvCmd.PositionUsedNatoAlphabet;
				}
				Destination = new Position(mvCmd.Destination);
				DestinationUsedNatoAlphabet = mvCmd.DestinationUsedNatoAlphabet;
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
				var destLetter = commandParams["destinationLetter"].FirstOrDefault();
				var destNumber = commandParams["destinationNumber"].FirstOrDefault();
				Destination = new Position(destLetter, destNumber);
				var destUsedNato = commandParams["destinationUsedNato"].FirstOrDefault();
				DestinationUsedNatoAlphabet = destUsedNato.Equals("true", StringComparison.OrdinalIgnoreCase);
			}

			if (Type == CommandType.Move || Type == CommandType.ConfirmPiece)
			{
				string posLetter = null;
				string posNumber = null;
				string posUsedNato = null;
				if (commandParams.TryGetValue("positionLetter", out paramsList))
				{
					posLetter = paramsList.FirstOrDefault();
				}
				if (commandParams.TryGetValue("positionNumber", out paramsList))
				{
					posNumber = paramsList.FirstOrDefault();
				}
				if (commandParams.TryGetValue("positionUsedNato", out paramsList))
				{
					posUsedNato = paramsList.FirstOrDefault();
				}
				if (!String.IsNullOrWhiteSpace(posLetter) &&
					!String.IsNullOrWhiteSpace(posNumber) &&
					!String.IsNullOrWhiteSpace(posUsedNato))
				{
					Position = new Position(posLetter, posNumber);
					PositionUsedNatoAlphabet = posUsedNato.Equals("true", StringComparison.OrdinalIgnoreCase);
				}
			}
		}
	}
}
