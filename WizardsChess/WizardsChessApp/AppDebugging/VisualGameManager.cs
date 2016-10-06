using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using WizardsChessApp.Chess;
using WizardsChessApp.CommandConversion;
using WizardsChessApp.VoiceControl;

namespace WizardsChessApp.AppDebugging
{
	public class VisualGameManager : INotifyPropertyChanged
	{
		public VisualGameManager()
		{
			State = "Ok";
			IsError = false;

			chessBoard = new ObservableChessBoard(board);
		}

		public async Task ListenForCommandAsync()
		{
			IsError = false;
			State = "Ok";
			if (cmdRecognizer == null)
			{
				cmdRecognizer = await CommandRecognizer.CreateAsync();
			}

			var voiceCommand = await cmdRecognizer.RecognizeSpeechAsync();

			if (voiceCommand.Status != Windows.Media.SpeechRecognition.SpeechRecognitionResultStatus.Success)
			{
				IsError = true;
				State = voiceCommand.Status.ToString();
				return;
			}

			var cmd = new Command(voiceCommand.SemanticInterpretation.Properties);

			if (cmd.Action != CommandConversion.Action.Move)
			{
				IsError = true;
				State = $"Received command with action {cmd.Action} instead of Move";
				return;
			}

			State = voiceCommand.Text;
			var moveCmd = new MoveCommand(voiceCommand.SemanticInterpretation.Properties);
			if (moveCmd.Piece.HasValue)
			{
				IsError = true;
				State = $"Cannot determine the selected piece at this time.";
				return;
			}

			if (!board.IsMoveValid(moveCmd.Position, moveCmd.Destination))
			{
				IsError = true;
				State = $"Invalid move";
				return;
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
