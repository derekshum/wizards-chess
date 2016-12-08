using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WizardsChess.VoiceControl.Commands;

namespace WizardsChess.VoiceControl.Events
{
	public class CommandHypothesisEventArgs : CommandEventArgs
	{
		public string CommandText { get; }

		public CommandHypothesisEventArgs(ICommand command, string commandText) : base(command)
		{
			CommandText = commandText;
		}
	}
}
