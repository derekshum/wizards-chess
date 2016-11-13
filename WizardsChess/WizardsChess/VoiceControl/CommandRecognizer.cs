using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Media.Playback;
using Windows.Media.SpeechRecognition;
using Windows.Media.SpeechSynthesis;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using WizardsChess.Chess;
using WizardsChess.CommandConversion;

namespace WizardsChess.VoiceControl
{
	internal class CommandRecognizer
	{
		private CommandRecognizer()
		{
			speechRecognizer = new SpeechRecognizer();
			speechRecognizer.UIOptions.IsReadBackEnabled = false;
			speechRecognizer.UIOptions.ShowConfirmation = false;
			continousSpeechSession = speechRecognizer.ContinuousRecognitionSession;
			continousSpeechSession.ResultGenerated += respondToContinuousSpeechRecognition;
			continousSpeechSession.Completed += respondToContinuousSpeechCompleted;

			audioOut.MediaFailed += ReleaseAudio;
			audioOut.MediaEnded += ReleaseAudio;
			audioOut.CurrentStateChanged += ReleaseAudio;
		}

		public static async Task<CommandRecognizer> CreateAsync()
		{
			var recognizer = new CommandRecognizer();
			var compilationResult = await recognizer.setupConstraintsAsync();
			if (compilationResult.Status != SpeechRecognitionResultStatus.Success)
			{
				throw new FormatException($"Could not compile grammar constraints. Received error {compilationResult.Status}");
			}
			await recognizer.StartContinuousSessionAsync();
			return recognizer;
		}

		public async Task StartContinuousSessionAsync()
		{
			await continousSpeechSession.StartAsync(SpeechContinuousRecognitionMode.PauseOnRecognition);
		}

		public async Task StopContinuousSessionAsync()
		{
			await continousSpeechSession.StopAsync();
		}

		public async Task PauseContinuousSessionAsync()
		{
			await continousSpeechSession.PauseAsync();
		}

		public void ResumeContinousSessionAsync()
		{
			continousSpeechSession.Resume();
		}

		private void respondToContinuousSpeechRecognition(
			SpeechContinuousRecognitionSession sender,
			SpeechContinuousRecognitionResultGeneratedEventArgs args)
		{

			if (args.Result.Status == SpeechRecognitionResultStatus.Success)
			{
				System.Diagnostics.Debug.WriteLine($"Recognized speech: {args.Result.Text}");
			}
			else
			{
				System.Diagnostics.Debug.WriteLine($"Received continuous speech result of {args.Result.Status}");
			}
			continousSpeechSession.Resume();
		}

		private void respondToContinuousSpeechCompleted(
			SpeechContinuousRecognitionSession sender,
			SpeechContinuousRecognitionCompletedEventArgs args)
		{
			System.Diagnostics.Debug.WriteLine($"Received continuous speech completion result of {args.Status}");
		}

		public async Task<Command> ConfirmPieceSelectionAsync(PieceType pieceType, IReadOnlyList<Position> possiblePositions)
		{
			StringBuilder strBuilder = new StringBuilder();
			strBuilder.Append("Did you mean the ").Append(pieceType.ToString()).Append(" at position ");
			for (int i = 0; i < possiblePositions.Count; i++)
			{
				if (possiblePositions.Count > 1 && i == possiblePositions.Count - 1)
				{
					strBuilder.Append(", or ");
				}
				else if (i > 0)
				{
					strBuilder.Append(", ");
				}
				strBuilder.Append(possiblePositions[i].ToString());
			}
			strBuilder.Append("?");

			await outputVoice(strBuilder.ToString());

			SpeechConstraints.EnableGrammar(speechRecognizer.Constraints, GrammarMode.YesNoCommands, false);
			SpeechConstraints.EnableGrammar(speechRecognizer.Constraints, GrammarMode.MoveCommands, false);
			SpeechConstraints.EnableGrammar(speechRecognizer.Constraints, GrammarMode.PieceConfirmation, true);

			var recognizedSpeech = await speechRecognizer.RecognizeAsync().AsTask();

			if (recognizedSpeech.Confidence == SpeechRecognitionConfidence.Rejected)
			{
				await outputVoice("I could not understand what you said. Please repeat yourself.");
				return await ConfirmPieceSelectionAsync(pieceType, possiblePositions);
			}
			if (recognizedSpeech.Confidence == SpeechRecognitionConfidence.Low)
			{
				var isPhraseCorrect = await VerifyPhraseAsync(recognizedSpeech);
				if (!isPhraseCorrect)
				{
					await outputVoice("Please re-state your command now.");
					return await ConfirmPieceSelectionAsync(pieceType, possiblePositions);
				}
			}

			if (recognizedSpeech.Status != Windows.Media.SpeechRecognition.SpeechRecognitionResultStatus.Success)
			{
				throw new Exception($"Voice recognition failed with status {recognizedSpeech.Status.ToString()}");
			}

			var cmd = new Command(recognizedSpeech.SemanticInterpretation.Properties);

			if (cmd.Action != WizardsChess.CommandConversion.Action.ConfirmPiece)
			{
				return cmd;
			}

			var confirmPieceCmd = new ConfirmPieceCommand(recognizedSpeech.SemanticInterpretation.Properties);
			return confirmPieceCmd;
		}

		public async Task<Command> RecognizeMoveAsync()
		{
			SpeechConstraints.EnableGrammar(speechRecognizer.Constraints, GrammarMode.YesNoCommands, false);
			SpeechConstraints.EnableGrammar(speechRecognizer.Constraints, GrammarMode.MoveCommands, true);
			SpeechConstraints.EnableGrammar(speechRecognizer.Constraints, GrammarMode.PieceConfirmation, false);

			var result = await speechRecognizer.RecognizeAsync().AsTask();

			if (result.Confidence == SpeechRecognitionConfidence.Rejected)
			{
				await outputVoice("I could not understand what you said. Please repeat yourself.");
				return await RecognizeMoveAsync();
			}
			if (result.Confidence == SpeechRecognitionConfidence.Low)
			{
				var isPhraseCorrect = await VerifyPhraseAsync(result);
				if (!isPhraseCorrect)
				{
					await outputVoice("Please re-state your command now.");
					return await RecognizeMoveAsync();
				}
			}

			if (result.Status != Windows.Media.SpeechRecognition.SpeechRecognitionResultStatus.Success)
			{
				throw new Exception($"Voice recognition failed with status {result.Status.ToString()}");
			}

			var cmd = new Command(result.SemanticInterpretation.Properties);

			if (cmd.Action != WizardsChess.CommandConversion.Action.Move)
			{
				return cmd;
			}

			var moveCmd = new MoveCommand(result.SemanticInterpretation.Properties);
			return moveCmd;
		}

		public async Task<bool> VerifyPhraseAsync(SpeechRecognitionResult phrase)
		{
			await outputVoice($"Did you mean: {phrase.Text}?");

			SpeechConstraints.EnableGrammar(speechRecognizer.Constraints, GrammarMode.YesNoCommands, true);
			SpeechConstraints.EnableGrammar(speechRecognizer.Constraints, GrammarMode.MoveCommands, false);
			SpeechConstraints.EnableGrammar(speechRecognizer.Constraints, GrammarMode.PieceConfirmation, false);

			var result = await speechRecognizer.RecognizeAsync().AsTask();

			if (result.Status != SpeechRecognitionResultStatus.Success)
			{
				throw new Exception($"Could not recognize speech input");
			}

			var cmd = new Command(result.SemanticInterpretation.Properties);

			return cmd.Action == WizardsChess.CommandConversion.Action.Yes;
		}

		private async Task outputVoice(string phrase)
		{
			var voiceStream = await speechSynth.SynthesizeTextToStreamAsync(phrase);
			audioOut.SetSource(voiceStream, voiceStream.ContentType);
			audioOut.Play();
			await Task.Delay(3000);
			audioOut.Stop();
		}

		private void ReleaseAudio(object sender, RoutedEventArgs args)
		{
			System.Diagnostics.Debug.WriteLine($"ReleaseAudio called with args {args}");
		}

		private async Task<SpeechRecognitionCompilationResult> setupConstraintsAsync()
		{
			var grammarConstraints = await SpeechConstraints.GetConstraintsAsync();
			foreach (var constraint in grammarConstraints)
			{
				speechRecognizer.Constraints.Add(constraint);
			}
			return await speechRecognizer.CompileConstraintsAsync().AsTask();
		}

		private Semaphore audioLock = new Semaphore(1, 1);
		private SpeechRecognizer speechRecognizer;
		private SpeechContinuousRecognitionSession continousSpeechSession;
		private SpeechSynthesizer speechSynth = new SpeechSynthesizer();
		private MediaElement audioOut = new MediaElement();
	}
}
