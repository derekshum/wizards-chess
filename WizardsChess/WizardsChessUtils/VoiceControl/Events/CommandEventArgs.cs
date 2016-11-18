using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WizardsChess.VoiceControl.Commands;

namespace WizardsChess.VoiceControl.Events
{
	public class CommandEventArgs : EventArgs
	{
		public ICommand Command { get; }

		public CommandEventArgs(ICommand command) : base()
		{
			Command = command;
		}
	}
}
