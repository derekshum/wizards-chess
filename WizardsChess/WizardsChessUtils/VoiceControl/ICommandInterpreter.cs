using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WizardsChess.Chess;
using WizardsChess.VoiceControl.Commands;

namespace WizardsChess.VoiceControl
{
	public interface ICommandInterpreter
	{
		event CommandEventHandler CommandReceived;

		Task StartAsync();
		Task StopAsync();
		Task ConfirmPieceSelectionAsync(PieceType pieceType, IReadOnlyList<Position> possiblePositions);
	}
}
