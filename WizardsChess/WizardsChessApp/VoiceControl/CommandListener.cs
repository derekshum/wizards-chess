﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Media.SpeechRecognition;
using WizardsChess.VoiceControl.Commands;
using WizardsChess.VoiceControl.Events;

namespace WizardsChess.VoiceControl
{
	public class CommandListener : ICommandListener
	{
		#region Public Events
		public event CommandEventHandler ReceivedCommand;
		public event CommandHypothesisEventHandler ReceivedCommandHypothesis;

		private void onCommandRecognized(CommandEventArgs e)
		{
			ReceivedCommand?.Invoke(this, e);
		}

		private void onCommandHypothesized(CommandHypothesisEventArgs e)
		{
			ReceivedCommandHypothesis?.Invoke(this, e);
		}
		#endregion

		public CommandFamily CommandFamily { get { return commandFamily; } }

		public bool IsListening { get { return isListening; } }

		#region Construction
		private CommandListener()
		{
			isListening = false;
			recognizer = new SpeechRecognizer();
			continuousSession = recognizer.ContinuousRecognitionSession;
			continuousSession.AutoStopSilenceTimeout = TimeSpan.FromMinutes(5);
			continuousSession.ResultGenerated += respondToSpeechRecognition;
		}

		public static async Task<CommandListener> CreateAsync()
		{
			var cmdInterpreter = new CommandListener();
			var grammarCompilationResult = await cmdInterpreter.setupGrammarConstraintsAsync();
			if (grammarCompilationResult.Status != SpeechRecognitionResultStatus.Success)
			{
				throw new FormatException($"Could not compile grammar constraints. Received error {grammarCompilationResult.Status}");
			}
			cmdInterpreter.setupCommandFamilyGrammar(CommandFamily.Move);
			return cmdInterpreter;
		}
		#endregion

		public async Task ListenForAsync(CommandFamily family)
		{
			if (family == commandFamily)
				return;
			commandFamily = family;

			if (isListening)
			{
				await StopListeningAsync();
			}
			setupCommandFamilyGrammar(family);
			if (isListening) // we just stopped listening and need to restart it
			{
				await StartListeningAsync();
			}
		}

		public async Task StartListeningAsync()
		{
			if (!isListening)
			{
				await continuousSession.StartAsync().AsTask();
				isListening = true;
			}
		}

		public async Task StopListeningAsync()
		{
			if (isListening)
			{
				await continuousSession.StopAsync();
				isListening = false;
			}
		}

		#region Private Members
		private bool isListening;
		private CommandFamily commandFamily;
		private SpeechRecognizer recognizer;
		private SpeechContinuousRecognitionSession continuousSession;
		#endregion

		#region Private Methods
		private async Task<SpeechRecognitionCompilationResult> setupGrammarConstraintsAsync()
		{
			var grammarConstraints = await SpeechConstraints.GetConstraintsAsync();
			foreach (var constraint in grammarConstraints)
			{
				recognizer.Constraints.Add(constraint);
			}
			return await recognizer.CompileConstraintsAsync().AsTask();
		}

		private void setupCommandFamilyGrammar(CommandFamily family)
		{
			switch (family)
			{
				case CommandFamily.Move:
					SpeechConstraints.EnableGrammar(recognizer.Constraints, GrammarMode.MoveCommands, true);
					SpeechConstraints.EnableGrammar(recognizer.Constraints, GrammarMode.PieceConfirmation, false);
					SpeechConstraints.EnableGrammar(recognizer.Constraints, GrammarMode.YesNoCommands, false);
					SpeechConstraints.EnableGrammar(recognizer.Constraints, GrammarMode.CancelCommand, false);
					break;
				case CommandFamily.PieceConfirmation:
					SpeechConstraints.EnableGrammar(recognizer.Constraints, GrammarMode.PieceConfirmation, true);
					SpeechConstraints.EnableGrammar(recognizer.Constraints, GrammarMode.MoveCommands, false);
					SpeechConstraints.EnableGrammar(recognizer.Constraints, GrammarMode.YesNoCommands, false);
					SpeechConstraints.EnableGrammar(recognizer.Constraints, GrammarMode.CancelCommand, true);
					break;
				case CommandFamily.YesNo:
					SpeechConstraints.EnableGrammar(recognizer.Constraints, GrammarMode.YesNoCommands, true);
					SpeechConstraints.EnableGrammar(recognizer.Constraints, GrammarMode.MoveCommands, false);
					SpeechConstraints.EnableGrammar(recognizer.Constraints, GrammarMode.PieceConfirmation, false);
					SpeechConstraints.EnableGrammar(recognizer.Constraints, GrammarMode.CancelCommand, true);
					break;
				default:
					throw new ArgumentException("Received unkown CommandFamily in setupCommandFamilyGrammarAsync");
			}
			commandFamily = family;
		}

		private void respondToSpeechRecognition(
			SpeechContinuousRecognitionSession sender,
			SpeechContinuousRecognitionResultGeneratedEventArgs args)
		{
			if (args.Result.Status == SpeechRecognitionResultStatus.Success)
			{
				System.Diagnostics.Debug.WriteLine($"Recognized speech: {args.Result.Text}");
				System.Diagnostics.Debug.WriteLine($"Recognition confidence: {args.Result.Confidence}");
				if (args.Result.Confidence == SpeechRecognitionConfidence.Rejected)
				{
					return;
				}
				else if (args.Result.Confidence == SpeechRecognitionConfidence.Low)
				{
					onCommandHypothesized(new CommandHypothesisEventArgs(convertSpeechToCommand(args.Result), args.Result.Text));
				}
				else
				{
					onCommandRecognized(new CommandEventArgs(convertSpeechToCommand(args.Result)));
				}
			}
			else
			{
				System.Diagnostics.Debug.WriteLine($"Received continuous speech result of {args.Result.Status}");
			}

			continuousSession.Resume();
		}

		private ICommand convertSpeechToCommand(SpeechRecognitionResult speech)
		{
			var cmdType = CommandTypeMethods.Parse(speech.SemanticInterpretation.Properties);
			switch (cmdType)
			{
				case CommandType.Move:
					return new MoveCommand(speech.SemanticInterpretation.Properties);
				case CommandType.ConfirmPiece:
					return new ConfirmPieceCommand(speech.SemanticInterpretation.Properties);
				default:
					return new Command(speech.SemanticInterpretation.Properties);
			}
		}
		#endregion
	}
}
