using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WizardsChess.VoiceControl.Commands
{
	public interface ICommand
	{
		CommandType Type { get; }
	}
}
