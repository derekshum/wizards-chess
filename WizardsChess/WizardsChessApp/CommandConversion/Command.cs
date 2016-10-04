using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WizardsChessApp.CommandConversion
{
	class Command
	{
		public Command(Command command)
		{
			Action = command.Action;
		}

		public Command(IReadOnlyDictionary<string, IReadOnlyList<string>> commandParams)
		{
			IReadOnlyList<string> paramsList;
			if (commandParams.TryGetValue("action", out paramsList))
			{
				Action = ActionMethods.Parse(paramsList.FirstOrDefault());
			}
			else
			{
				throw new ArgumentException("Cannot create a Command without an action parameter");
			}
		}

		public Action Action { get; set; }
	}
}
