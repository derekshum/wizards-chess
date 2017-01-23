using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WizardsChess.VoiceControl.Commands
{
	public class MagnetCommand : Command
	{
		public bool EnableMagnet { get; }

		private MagnetCommand() : base(CommandType.Magnet) { }

		public MagnetCommand(ICommand command) : this()
		{
			var mCmd = command as MagnetCommand;
			if (mCmd != null)
			{
				EnableMagnet = mCmd.EnableMagnet;
			}
			else
			{
				System.Diagnostics.Debug.WriteLine("Attempted to make a new MagnetCommand from a different ICommand.");
				throw new InvalidCastException("Attempted to make a new MagnetCommand from a different ICommand.");
			}
		}

		public MagnetCommand(IReadOnlyDictionary<string, IReadOnlyList<string>> commandParams) : this()
		{
			IReadOnlyList<string> paramsList;

			if (commandParams.TryGetValue("enable", out paramsList))
			{
				EnableMagnet = Boolean.Parse(paramsList.FirstOrDefault());
			}
			else
			{
				System.Diagnostics.Debug.WriteLine("MagnetCommand did not receive an 'enable' value.");
			}
		}
	}
}