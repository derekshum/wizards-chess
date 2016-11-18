using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WizardsChess.Chess;

namespace WizardsChess.VoiceControl.Commands
{
	public class ConfirmPieceCommand : ICommand
	{
		public CommandType Type { get; }
		public PieceType? Piece { get; }
		public Position Position { get; }
		public bool PositionUsedNatoAlphabet { get; }

		public ConfirmPieceCommand()
		{
			Type = CommandType.ConfirmPiece;
		}

		public ConfirmPieceCommand(IReadOnlyDictionary<string, IReadOnlyList<string>> commandParams) : this()
		{
			IReadOnlyList<string> paramsList;
			if (commandParams.TryGetValue("piece", out paramsList))
			{
				Piece = PieceTypeMethods.Parse(paramsList.FirstOrDefault());
			}

			string posLetter = null;
			string posNumber = null;
			string posUsedNato = null;
			if (commandParams.TryGetValue("pieceLetter", out paramsList))
			{
				posLetter = paramsList.FirstOrDefault();
			}
			if (commandParams.TryGetValue("pieceNumber", out paramsList))
			{
				posNumber = paramsList.FirstOrDefault();
			}
			if (commandParams.TryGetValue("usedNato", out paramsList))
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
			else
			{
				throw new ArgumentException($"Cannot create valid {nameof(ConfirmPieceCommand)} without a valid {nameof(Position)}");
			}
		}
	}
}
