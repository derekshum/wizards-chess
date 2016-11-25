using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WizardsChess.Chess;
using WizardsChess.Movement;

namespace WizardsChess.VoiceControl.Commands
{
	public class MotorMoveCommand : ICommand
	{
		public CommandType Type { get; }
		public Axis Axis { get; }
		public int Steps { get; }

		private MotorMoveCommand()
		{
			Type = CommandType.MotorMove;
		}

		public MotorMoveCommand(ICommand command) : this()
		{
			var mvCmd = command as MotorMoveCommand;
			if (mvCmd != null)
			{
				Axis = mvCmd.Axis;
			}
		}

		public MotorMoveCommand(IReadOnlyDictionary<string, IReadOnlyList<string>> commandParams) : this()
		{
			IReadOnlyList<string> paramsList;

			if (Type == CommandType.Move)
			{
				if (commandParams.TryGetValue("axis", out paramsList))
				{
					Axis = (Axis)Enum.Parse(typeof(Axis), paramsList.FirstOrDefault(), true);
				}
				var steps = commandParams["steps"].FirstOrDefault();
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
