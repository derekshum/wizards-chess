using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WizardsChessApp.Chess;

namespace WizardsChessApp.CommandConversion
{
	public class ConfirmPieceCommand : Command
	{
		public ConfirmPieceCommand(IReadOnlyDictionary<string, IReadOnlyList<string>> commandParams) : base(commandParams)
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

		public PieceType? Piece { get; set; }
		public Position Position { get; set; }
	}
}
