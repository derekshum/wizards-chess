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
		PieceConfirmation
	}

	static class SpeechConstraints
	{
		public static async Task<IList<ISpeechRecognitionConstraint>> GetConstraintsAsync()
		{
			var grammarList = new List<ISpeechRecognitionConstraint>();

			var commandsFileTask = StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///VoiceControl/MoveCommands.grxml")).AsTask();
			var yesNoFileTask = StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///VoiceControl/YesNoCommands.grxml")).AsTask();
			var pieceConfirmationTask = StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///VoiceControl/PieceConfirmation.grxml")).AsTask();
			await Task.WhenAll(commandsFileTask, yesNoFileTask, pieceConfirmationTask);

			var moveGrammar = new SpeechRecognitionGrammarFileConstraint(commandsFileTask.Result, k_moveCommands);
			var yesNoGrammar = new SpeechRecognitionGrammarFileConstraint(yesNoFileTask.Result, k_yesNoCommands);
			var pieceConfirmationGrammar = new SpeechRecognitionGrammarFileConstraint(pieceConfirmationTask.Result, k_pieceConfirmationCommands);

			grammarList.Add(moveGrammar);
			grammarList.Add(yesNoGrammar);
			grammarList.Add(pieceConfirmationGrammar);

			return grammarList;
		}

		public static void EnableGrammar(IList<ISpeechRecognitionConstraint> constraints, GrammarMode constraintTag, bool enable)
		{
			string tag;
			switch (constraintTag)
			{
				case GrammarMode.MoveCommands:
					tag = k_moveCommands;
					break;
				case GrammarMode.YesNoCommands:
					tag = k_yesNoCommands;
					break;
				case GrammarMode.PieceConfirmation:
					tag = k_pieceConfirmationCommands;
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

		private static readonly string k_moveCommands = "moveCommands";
		private static readonly string k_yesNoCommands = "yesNoCommands";
		private static readonly string k_pieceConfirmationCommands = "pieceConfirmation";
	}
}
