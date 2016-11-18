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
		private CommandInterpreter(ICommandListener listener, ICommunicator communicator)
		{
			isStarted = false;
			commandHypothesis = null;
			listeningState = ListeningState.Move;

			listener.ReceivedCommand += receivedCommand;
			listener.ReceivedCommandHypothesis += receivedCommandHypothesis;

			this.listener = listener;
			this.communicator = communicator;
		}

		public static async Task<CommandInterpreter> CreateAsync()
		{
			var listener = await CommandListener.CreateAsync();

			var interpreter = new CommandInterpreter(listener, new Communicator());

			return interpreter;
		}
		#endregion

		public async Task ConfirmPieceSelectionAsync(PieceType pieceType, IReadOnlyList<Position> possiblePositions)
		{
			StringBuilder confirmPieceSpeech = new StringBuilder();
			confirmPieceSpeech.Append($"Do you mean the {pieceType} at position ");

			int i = 0;
			for (i = 0; i < (possiblePositions.Count - 1); i++)
			{
				confirmPieceSpeech.Append($"{possiblePositions[i]}, ");
			}
			confirmPieceSpeech.Append($"or {possiblePositions[i]}?");

			await communicator.Speak(confirmPieceSpeech.ToString());

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
		private ListeningState preHypothesisListeningState;
		private ICommandListener listener;
		private ICommunicator communicator;
		private CommandHypothesisEventArgs commandHypothesis;
		private PieceType possiblePieceType;
		private IReadOnlyList<Position> possiblePiecePositions;
		#endregion

		#region Private Methods
		private async Task changeStateAsync(ListeningState state)
		{
			if (listeningState != state)
			{
				System.Diagnostics.Debug.WriteLine($"Changing CommandInterpreter listening state to {state}");

				if (state == ListeningState.Hypothesis)
				{
					preHypothesisListeningState = listeningState;
				}

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
			if (listeningState == ListeningState.Hypothesis)
			{
				await changeStateAsync(preHypothesisListeningState);
				await handleRejectedHypothesisAsync();
				return;
			}

			//await listener.StopListeningAsync();
			commandHypothesis = e;
			await changeStateAsync(ListeningState.Hypothesis);
			await communicator.Speak($"Did you say: {e.CommandText}");
			//await listener.StartListeningAsync();
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
					await handleHypothesisConfirmationResponseAsync(command);
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
			await changeStateAsync(preHypothesisListeningState);
			if (isHypothesisValid(command))
			{
				// The last command was understood correctly, handle it as a regular command
				receivedCommand(this, commandHypothesis);
			}
			else
			{
				await handleRejectedHypothesisAsync();
			}
		}

		private async Task handleRejectedHypothesisAsync()
		{
			switch (listeningState)
			{
				case ListeningState.Hypothesis:
					System.Diagnostics.Debug.WriteLine("CommandInterpreter got stuck in the Hypothesis state afer rejecting a hypothesis");
					break;
				case ListeningState.PieceConfirmation:
					await ConfirmPieceSelectionAsync(possiblePieceType, possiblePiecePositions);
					break;
				case ListeningState.Move:
				default:
					await communicator.Speak("Ok. Please repeat your previous command.");
					break;
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
			else if (command.Type != CommandType.Cancel)
			{
				// Try piece confirmation again if their answer just didn't make sense
				await communicator.Speak("That position was not valid.");
				await ConfirmPieceSelectionAsync(possiblePieceType, possiblePiecePositions);
				return;
			}
			else
			{
				// If they cancel, return to move command state
				await communicator.Speak("Ok. Please repeat your move command.");
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
