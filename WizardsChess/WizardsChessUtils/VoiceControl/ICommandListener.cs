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
		bool IsListening { get; }

		Task StartListeningAsync();
		Task StopListeningAsync();

		event CommandEventHandler ReceivedCommand;
		event CommandHypothesisEventHandler ReceivedCommandHypothesis;
	}

	public delegate void CommandEventHandler(Object sender, CommandEventArgs e);
	public delegate void CommandHypothesisEventHandler(Object sender, CommandHypothesisEventArgs e);
}
