using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WizardsChess.VoiceControl.Events
{
	public class CommandHypothesisEventArgs : EventArgs
	{
		public string SpeechHypothesis { get; }

		public CommandHypothesisEventArgs(string speechHypothesis)
		{
			SpeechHypothesis = speechHypothesis;
		}
	}
}
