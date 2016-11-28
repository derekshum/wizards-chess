using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WizardsChess.Chess;
using WizardsChess.Movement;

namespace WizardsChess.VoiceControl.Commands
{
	public class MotorMoveCommand : Command
	{
		public Axis Axis { get; }
		public int Steps { get; }

		private MotorMoveCommand() : base(CommandType.MotorMove) { }

		public MotorMoveCommand(ICommand command) : this()
		{
			var mvCmd = command as MotorMoveCommand;
			if (mvCmd != null)
			{
				Axis = mvCmd.Axis;
			}
			else
			{
				throw new InvalidCastException("Attempted to make a new MotorMoveCommand from a different ICommand.");
			}
		}

		public MotorMoveCommand(IReadOnlyDictionary<string, IReadOnlyList<string>> commandParams) : this()
		{
			IReadOnlyList<string> paramsList;

			if (Type == CommandType.MotorMove)
			{
				if (commandParams.TryGetValue("axis", out paramsList))
				{
					Axis = (Axis)Enum.Parse(typeof(Axis), paramsList.FirstOrDefault(), true);
				}
				else
				{
					System.Diagnostics.Debug.WriteLine("MotorMoveCommand did not receive an 'axis' value.");
				}
				if (commandParams.TryGetValue("steps", out paramsList))
				{
					int steps;
					if (!Int32.TryParse(paramsList.FirstOrDefault(), out steps))
					{
						System.Diagnostics.Debug.WriteLine("Could not parse steps for a motor move.");
					}
					else
					{
						Steps = steps;
					}
				}
				else
				{
					System.Diagnostics.Debug.WriteLine("MotorMoveCommand did not receive a 'steps' value.");
				}
				if (commandParams.TryGetValue("direction", out paramsList))
				{
					if (paramsList.FirstOrDefault().Equals("backward", StringComparison.OrdinalIgnoreCase))
					{
						Steps *= -1;
					}
				}
				else
				{
					System.Diagnostics.Debug.WriteLine("MotorMoveCommand did not receive a 'direction' value.");
				}
			}
			else
			{
				throw new ArgumentException("Attempted to make a MotorMoveCommand from a different command type.");
			}
		}
	}
}
