using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WizardsChess.Chess;

namespace WizardsChess.CommandConversion
{
	public class ConfirmPieceCommand : ICommand
	{
		public CommandType Type { get; }
		public PieceType? Piece { get; }
		public Position Position { get; }

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
			else
			{
				throw new ArgumentException($"Cannot create valid {nameof(ConfirmPieceCommand)} without a valid {nameof(Position)}");
			}
		}
	}
}
