using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using WizardsChess.Chess;
using WizardsChess.CommandConversion;
using WizardsChess.VoiceControl;

namespace WizardsChess.AppDebugging
{
	public class VisualGameManager : INotifyPropertyChanged
	{
		public VisualGameManager()
		{
			State = "Ok";
			IsError = false;

			chessBoard = new ObservableChessBoard(board);
		}

		public async Task PerformCommandAsync()
		{
			try
			{
				cmdRecognizer = await CommandRecognizer.CreateAsync();
				await cmdRecognizer.StartContinuousSessionAsync();

				//State = "Listening";
				//var cmd = await listenForCommandAsync();
				//State = "Processing";

				//if (cmd.Action == WizardsChess.CommandConversion.Action.Move)
				//{
				//	await performMoveIfValidAsync(cmd as MoveCommand);
				//}

				//IsError = false;
				//State = "Ok";
			}
			catch (Exception e)
			{
				IsError = true;
				State = e.Message;
			}
		}

		private async Task<ICommand> listenForCommandAsync()
		{
			if (cmdRecognizer == null)
			{
				cmdRecognizer = await CommandRecognizer.CreateAsync();
			}

			return await cmdRecognizer.RecognizeMoveAsync();
		}

		private async Task performMoveIfValidAsync(MoveCommand moveCmd)
		{
			if (moveCmd.Piece.HasValue)
			{
				var possibleStartPositions = board.FindPotentialPiecesForMove(moveCmd.Piece.Value, moveCmd.Destination);
				if (possibleStartPositions.Count == 0)
				{
					throw new InvalidOperationException($"Could not find a possible starting piece of type {moveCmd.Piece.Value} going to {moveCmd.Destination}");
				}
				else if (possibleStartPositions.Count == 1)
				{
					moveCmd.Position = possibleStartPositions.First();
				}
				else
				{
					var cmd = await cmdRecognizer.ConfirmPieceSelectionAsync(moveCmd.Piece.Value, possibleStartPositions.ToList());
					var confirmationCmd = cmd as ConfirmPieceCommand;
					if (confirmationCmd == null)
					{
						throw new Exception("Could not confirm which piece was meant to move.");
					}

					moveCmd.Position = confirmationCmd.Position;
				}
			}

			board.MovePiece(moveCmd.Position, moveCmd.Destination);
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
		private ChessBoard board = new ChessBoard();
		private CommandRecognizer cmdRecognizer;
	}
}
