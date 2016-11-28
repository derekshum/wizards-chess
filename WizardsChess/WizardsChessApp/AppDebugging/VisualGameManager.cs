using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using WizardsChess.Chess;
using WizardsChess.VoiceControl.Commands;
using WizardsChess.VoiceControl;
using WizardsChess.VoiceControl.Events;
using WizardsChess.Movement;

namespace WizardsChess.AppDebugging
{
	public class VisualGameManager : INotifyPropertyChanged
	{
		public VisualGameManager(Windows.UI.Core.CoreDispatcher uiDispatcher)
		{
			this.uiDispatcher = uiDispatcher;
			State = "Ok";
			IsError = false;

			chessBoard = new ObservableChessBoard(Logic.Board, uiDispatcher);
		}

		public async Task SetupCommandInterpreter()
		{
			if (commandInterpreter != null)
				return;
			commandInterpreter = await CommandInterpreter.CreateAsync();
			commandInterpreter.CommandReceived += commandReceived;
		}

		public async Task StartGameAsync()
		{
			await commandInterpreter.StartAsync();
		}

		private async void commandReceived(Object sender, CommandEventArgs e)
		{
			switch (e.Command.Type)
			{
				case CommandType.Move:
					var moveCmd = e.Command as MoveCommand;
					currentMoveCommand = moveCmd;
					await performMoveOnUiIfValidAsync(moveCmd);
					break;
				case CommandType.ConfirmPiece:
					var pieceConfirmation = e.Command as ConfirmPieceCommand;
					if (currentMoveCommand == null)
					{
						throw new Exception("Received piece confirmation command when currentMoveCommand was null");
					}
					currentMoveCommand.Position = pieceConfirmation.Position;
					await performMoveOnUiIfValidAsync(currentMoveCommand);
					break;
				default:
					System.Diagnostics.Debug.WriteLine($"VisualGameManager received command type: {e.Command.Type}");
					break;
			}
		}

		private async Task performMoveOnUiIfValidAsync(MoveCommand moveCmd)
		{
			if (uiDispatcher == null)
			{
				uiDispatcher = Windows.UI.Core.CoreWindow.GetForCurrentThread().Dispatcher;
			}
			await uiDispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () => { await performMoveIfValidAsync(moveCmd); });
		}

		private async Task performMoveIfValidAsync(MoveCommand moveCmd)
		{
			if (!moveCmd.Position.HasValue)
			{
				var possibleStartPositions = Logic.FindPotentialPiecesForMove(moveCmd.Piece.Value, moveCmd.Destination);
				if (possibleStartPositions.Count == 0)
				{
					System.Diagnostics.Debug.WriteLine($"Could not find a possible starting piece of type {moveCmd.Piece.Value} going to {moveCmd.Destination}");
					return;
				}
				else if (possibleStartPositions.Count == 1)
				{
					moveCmd.Position = possibleStartPositions.First();
				}
				else
				{
					await commandInterpreter.ConfirmPieceSelectionAsync(moveCmd.Piece.Value, possibleStartPositions.ToList());
					return;
				}
			}

			try
			{
				Logic.MovePiece(moveCmd.Position.Value, moveCmd.Destination);
			}
			catch (Exception e)
			{
				System.Diagnostics.Debug.WriteLine(e.Message);
			}
			chessBoard.UpdatePieceLocations();
		}

		private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		public event PropertyChangedEventHandler PropertyChanged;

		public string State {
			get { return this.state; }
			private set
			{
				if (value != this.state)
				{
					this.state = value;
					NotifyPropertyChanged();
				}
			}
		}

		public bool IsError
		{
			get { return this.isError; }
			private set
			{
				if (value != this.isError)
				{
					this.isError = value;
					NotifyPropertyChanged();
				}
			}
		}

		public ObservableChessBoard Pieces { get { return chessBoard; } }

		private string state;
		private bool isError;
		private ObservableChessBoard chessBoard;
		private ChessLogic Logic = new ChessLogic(); //TODO: figure this error out
		private ICommandInterpreter commandInterpreter;
		private MoveCommand currentMoveCommand;
		private Windows.UI.Core.CoreDispatcher uiDispatcher;
	}
}
