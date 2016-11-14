using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WizardsChess.Chess;

namespace WizardsChess.VoiceControl
{
	interface ICommandInterpreter
	{
		event CommandEventHandler CommandReceived;

		Task ConfirmPieceSelectionAsync(PieceType pieceType, IReadOnlyList<Position> possiblePositions);
	}
}
