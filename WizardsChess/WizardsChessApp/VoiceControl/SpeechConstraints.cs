using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Media.SpeechRecognition;
using Windows.Storage;

namespace WizardsChess.VoiceControl
{
	enum GrammarMode
	{
		MoveCommands,
		YesNoCommands,
		PieceConfirmation,
		CancelCommand
	}

	static class SpeechConstraints
	{
		public static async Task<IList<ISpeechRecognitionConstraint>> GetConstraintsAsync()
		{
			var grammarList = new List<ISpeechRecognitionConstraint>();

			var commandsFileTask = StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///VoiceControl/MoveCommands.grxml")).AsTask();
			var yesNoFileTask = StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///VoiceControl/YesNoCommands.grxml")).AsTask();
			var pieceConfirmationTask = StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///VoiceControl/PieceConfirmation.grxml")).AsTask();
			var cancelFileTask = StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///VoiceControl/CancelCommand.grxml")).AsTask();

			await Task.WhenAll(commandsFileTask, yesNoFileTask, pieceConfirmationTask, cancelFileTask);

			var moveGrammar = new SpeechRecognitionGrammarFileConstraint(commandsFileTask.Result, moveCommandsGrammarTag);
			var yesNoGrammar = new SpeechRecognitionGrammarFileConstraint(yesNoFileTask.Result, yesNoCommandsGrammarTag);
			var pieceConfirmationGrammar = new SpeechRecognitionGrammarFileConstraint(pieceConfirmationTask.Result, pieceConfirmationGrammarTag);
			var cancelGrammar = new SpeechRecognitionGrammarFileConstraint(cancelFileTask.Result, cancelCommandGrammarTag);

			grammarList.Add(moveGrammar);
			grammarList.Add(yesNoGrammar);
			grammarList.Add(pieceConfirmationGrammar);
			grammarList.Add(cancelGrammar);

#if DEBUG
			var debugFile = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///VoiceControl/DebugCommands.grxml")).AsTask();
			var debugGrammar = new SpeechRecognitionGrammarFileConstraint(debugFile, debugGrammarTag);
			grammarList.Add(debugGrammar);
#endif

			return grammarList;
		}

		public static void EnableGrammar(IList<ISpeechRecognitionConstraint> constraints, GrammarMode constraintTag, bool enable)
		{
			string tag;
			switch (constraintTag)
			{
				case GrammarMode.MoveCommands:
					tag = moveCommandsGrammarTag;
					break;
				case GrammarMode.YesNoCommands:
					tag = yesNoCommandsGrammarTag;
					break;
				case GrammarMode.PieceConfirmation:
					tag = pieceConfirmationGrammarTag;
					break;
				case GrammarMode.CancelCommand:
					tag = cancelCommandGrammarTag;
					break;
				default:
					throw new ArgumentException($"No such grammar constraint: {constraintTag}");
			}

			foreach (var constraint in constraints)
			{
				if (constraint.Tag == tag)
				{
					constraint.IsEnabled = enable;
					return;
				}
			}
		}

		private static readonly string moveCommandsGrammarTag = "moveCommands";
		private static readonly string yesNoCommandsGrammarTag = "yesNoCommands";
		private static readonly string pieceConfirmationGrammarTag = "pieceConfirmation";
		private static readonly string cancelCommandGrammarTag = "cancelCommand";
		private static readonly string debugGrammarTag = "debugCommand";
	}
}
