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
		Move,
		Hypothesis,
		PieceConfirmation
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
			isStarted = false;
			commandHypothesis = null;
			listeningState = ListeningState.Move;

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
			possiblePieceType = pieceType;
			possiblePiecePositions = possiblePositions;
			await changeStateAsync(ListeningState.PieceConfirmation);
		}

		public async Task StartAsync()
		{
			if (isStarted)
			{
				return;
			}

			await changeStateAsync(ListeningState.Move);
			await listener.StartListeningAsync();
			isStarted = true;
		}

		#region Private Members
		private bool isStarted;
		private ListeningState listeningState;
		private ListeningState previousListeningState;
		private ICommandListener listener;
		private CommandEventArgs commandHypothesis;
		private PieceType possiblePieceType;
		private IReadOnlyList<Position> possiblePiecePositions;
		#endregion

		#region Private Methods
		private async Task changeStateAsync(ListeningState state)
		{
			if (listeningState != state)
			{
				System.Diagnostics.Debug.WriteLine($"Changing CommandInterpreter listening state to {state}");
				previousListeningState = listeningState;

				listeningState = state;

				switch (state)
				{
					case ListeningState.Move:
						await listener.ListenForAsync(CommandFamily.Move);
						break;
					case ListeningState.PieceConfirmation:
						await listener.ListenForAsync(CommandFamily.PieceConfirmation);
						break;
					case ListeningState.Hypothesis:
						await listener.ListenForAsync(CommandFamily.YesNo);
						break;
					default:
						throw new Exception("Tried to change CommandInterpreter state to an unknown listening state");
				}
			}
		}

		private async void receivedCommand(Object sender, CommandEventArgs e)
		{
			if (isCommandFamilyValid(e.Command.Type.GetFamily()))
			{
				await handleCommandAsync(e.Command);
			}
			else
			{
				System.Diagnostics.Debug.WriteLine($"CommandInterpreter received invalid command of type {e.Command.Type}");
				// TODO: This demonstrates a major internal error. Handle it.
			}
		}

		private async void receivedCommandHypothesis(Object sender, CommandHypothesisEventArgs e)
		{
			await listener.StopListeningAsync();
			// TODO: Use an ICommandSpeaker to ask if this was the right command
			commandHypothesis = e;
			await changeStateAsync(ListeningState.Hypothesis);
			await listener.StartListeningAsync();
		}

		private bool isCommandFamilyValid(CommandFamily family)
		{
			switch (listeningState)
			{
				case ListeningState.Hypothesis:
					return family == CommandFamily.YesNo || family == CommandFamily.Other;
				case ListeningState.PieceConfirmation:
					return family == CommandFamily.PieceConfirmation || family == CommandFamily.Other;
				case ListeningState.Move:
				default:
					return family == CommandFamily.Move;
			}
		}

		private async Task handleCommandAsync(ICommand command)
		{
			switch (listeningState)
			{
				case ListeningState.Hypothesis:
					await changeStateAsync(previousListeningState);
					if (isHypothesisValid(command))
					{
						onCommandReceived(commandHypothesis);
					}
					break;
				case ListeningState.PieceConfirmation:
					await handleConfirmPieceCommandAsync(command);
					break;
				case ListeningState.Move:
				default:
					onCommandReceived(new CommandEventArgs(command));
					break;
			}
		}

		private async Task handleHypothesisConfirmationResponseAsync(ICommand command)
		{
			await changeStateAsync(previousListeningState);
			if (isHypothesisValid(command))
			{
				receivedCommand(this, commandHypothesis);
			}
			else if (listeningState == ListeningState.PieceConfirmation)
			{
				await ConfirmPieceSelectionAsync(possiblePieceType, possiblePiecePositions);
			}
			else
			{
				// TODO: Ask for their move again
			}
		}

		private bool isHypothesisValid(ICommand command)
		{
			var family = command.Type.GetFamily();
			if (family == CommandFamily.YesNo)
			{
				return command.Type == CommandType.Yes;
			}
			return false;
		}

		private async Task handleConfirmPieceCommandAsync(ICommand command)
		{
			if (isValidPieceCommand(command as ConfirmPieceCommand))
			{
				onCommandReceived(new CommandEventArgs(command));
			}
			else
			{
				// TODO: Speak to user about issue
			}
			await changeStateAsync(ListeningState.Move);
		}

		private bool isValidPieceCommand(ConfirmPieceCommand command)
		{
			// Check the start position is actually one we asked for.
			foreach (var position in possiblePiecePositions)
			{
				if (position == command.Position)
				{
					return true;
				}
			}

			return false;
		}
		#endregion
	}
}
