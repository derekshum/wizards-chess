using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WizardsChess.CommandConversion
{
	public class Command : ICommand
	{
		public CommandType Type { get; }
		
		public Command(CommandType type)
		{
			Type = type;
		}

		public Command(IReadOnlyDictionary<string, IReadOnlyList<string>> commandParams)
		{
			Type = CommandTypeMethods.Parse(commandParams);
		}
	}
}
