using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WizardsChess.Chess;
using WizardsChess.VoiceControl.Commands;
using WizardsChess.VoiceControl.Events;

namespace WizardsChess.VoiceControl
{
	enum ListeningState
	{
		General,
		Hypothesis
	}

	public class CommandInterpreter : ICommandInterpreter
	{
		#region Events
		public event CommandEventHandler CommandReceived;

		private void onCommandReceived(CommandEventArgs e)
		{
			CommandReceived?.Invoke(this, e);
		}
		#endregion

		#region Construction
		private CommandInterpreter(CommandListener listener)
		{
			commandHypothesis = null;
			listeningState = ListeningState.General;

			listener.ReceivedCommand += receivedCommand;
			listener.ReceivedCommandHypothesis += receivedCommandHypothesis;

			this.listener = listener;
		}

		public static async Task<CommandInterpreter> CreateAsync()
		{
			var listener = await CommandListener.CreateAsync();

			var interpreter = new CommandInterpreter(listener);

			return interpreter;
		}
		#endregion

		public async Task ConfirmPieceSelectionAsync(PieceType pieceType, IReadOnlyList<Position> possiblePositions)
		{
			// TODO: Announce the piece options using the ICommandSpeaker
			await SetCommandFamilyAsync(CommandFamily.PieceConfirmation);
		}

		public async Task SetCommandFamilyAsync(CommandFamily family)
		{
			if (commandFamily != family)
			{
				commandFamily = family;
				System.Diagnostics.Debug.WriteLine($"Set CommandInterpreter command family to {family}");
				await listener.ListenForAsync(family);
			}
			if (!listener.IsListening)
			{
				await listener.StartListeningAsync();
			}
		}

		#region Private Members
		private CommandFamily commandFamily;
		private ListeningState listeningState;
		private ICommandListener listener;
		private CommandEventArgs commandHypothesis;
		#endregion

		#region Private Methods
		private async void receivedCommand(Object sender, CommandEventArgs e)
		{
			var family = CommandTypeMethods.GetFamily(e.Command.Type);
			switch (listeningState)
			{
				case ListeningState.Hypothesis:
					if (family == CommandFamily.YesNo)
					{
						if (e.Command.Type == CommandType.Yes)
						{
							onCommandReceived(commandHypothesis);
						}
						else
						{
							// Command hypothesis was rejected
							commandHypothesis = null;
							await SetCommandFamilyAsync(commandFamily);
						}
						listeningState = ListeningState.General;
					}
					else if (e.Command.Type == CommandType.Cancel)
					{
						await SetCommandFamilyAsync(commandFamily);
						listeningState = ListeningState.General;
					}
					else
					{
						System.Diagnostics.Debug.WriteLine($"CommandInterpreter was expecting a hypothesis response, but received a command of type {e.Command.Type}");
					}
					break;
				case ListeningState.General:
				default:
					if (isCommandValid(family))
					{
						onCommandReceived(e);
					}
					else
					{
						System.Diagnostics.Debug.WriteLine($"CommandInterpreter received an invalid General state command of type {e.Command.Type}");
					}
					break;
			}
		}

		private async void receivedCommandHypothesis(Object sender, CommandHypothesisEventArgs e)
		{
			await listener.StopListeningAsync();
			// TODO: Use an ICommandSpeaker to ask if this was the right command
			commandHypothesis = e;
			listeningState = ListeningState.Hypothesis;
			await listener.ListenForAsync(CommandFamily.YesNo);
			await listener.StartListeningAsync();
		}

		private bool isCommandValid(CommandFamily family)
		{
			return family == commandFamily || family == CommandFamily.Other;
		}
		#endregion
	}
}
