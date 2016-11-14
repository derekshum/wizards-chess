using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WizardsChess.VoiceControl.Commands;
using WizardsChess.VoiceControl.Events;

namespace WizardsChess.VoiceControl
{
	

	public interface ICommandListener
	{
		Task StartListeningAsync();
		Task StopListeningAsync();

		Task ListenForAsync(CommandFamily command);

		event CommandEventHandler ReceivedCommand;
		event CommandHypothesisEventHandler ReceivedCommandHypothesis;
	}

	public delegate void CommandEventHandler(Object sender, CommandEventArgs e);
	public delegate void CommandHypothesisEventHandler(Object sender, CommandHypothesisEventArgs e);
}
