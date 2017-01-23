using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WizardsChess.VoiceControl.Commands
{
    public enum CastleDirection
    {
        Left,
        Right
    }

    public class CastleCommand : Command
    {
        public CastleDirection Direction;

        public CastleCommand() : base (CommandType.Castle) {}

        public CastleCommand(ICommand command) : this()
		{
            var cCmd = command as CastleCommand;
            if (cCmd != null)
            {
                Direction = cCmd.Direction;
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("Attempted to make a new CastleCommand from a different ICommand.");
                throw new InvalidCastException("Attempted to make a new CastleCommand from a different ICommand.");
            }
        }

        public CastleCommand(IReadOnlyDictionary<string, IReadOnlyList<string>> commandParams) : this()
		{
            IReadOnlyList<string> paramsList;

            if (commandParams.TryGetValue("direction", out paramsList))
            {
                Direction = (CastleDirection)Enum.Parse(typeof(CastleDirection), paramsList.FirstOrDefault(), true);
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("CastleCommand did not receive a 'direction' value.");
            }
        }
    }
}
