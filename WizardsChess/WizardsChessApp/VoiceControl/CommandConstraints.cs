using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Media.SpeechRecognition;
using Windows.Storage;

namespace WizardsChessApp.VoiceControl
{
	enum GrammarConstraints
	{
		MoveCommands,
		YesNoCommands
	}

	static class CommandConstraints
	{
		public static async Task<IList<ISpeechRecognitionConstraint>> GetConstraintsAsync()
		{
			var grammarList = new List<ISpeechRecognitionConstraint>();

			var commandsFileTask = StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///VoiceControl/MoveCommands.grxml")).AsTask();
			var yesNoFileTask = StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///VoiceControl/YesNoCommands.grxml")).AsTask();
			await Task.WhenAll(commandsFileTask, yesNoFileTask);

			moveGrammar = new SpeechRecognitionGrammarFileConstraint(commandsFileTask.Result, k_moveCommands);
			yesNoGrammar = new SpeechRecognitionGrammarFileConstraint(yesNoFileTask.Result, k_yesNoCommands);

			grammarList.Add(moveGrammar);
			grammarList.Add(yesNoGrammar);

			return grammarList;
		}

		public static void EnableGrammar(GrammarConstraints grammar)
		{
			switch (grammar)
			{
				case GrammarConstraints.MoveCommands:
					moveGrammar.IsEnabled = true;
					break;
				case GrammarConstraints.YesNoCommands:
					yesNoGrammar.IsEnabled = true;
					break;
				default:
					break;
			}
		}

		public static void DisableGrammar(GrammarConstraints grammar)
		{
			switch (grammar)
			{
				case GrammarConstraints.MoveCommands:
					moveGrammar.IsEnabled = false;
					break;
				case GrammarConstraints.YesNoCommands:
					yesNoGrammar.IsEnabled = false;
					break;
				default:
					break;
			}
		}

		private static ISpeechRecognitionConstraint moveGrammar;
		private static ISpeechRecognitionConstraint yesNoGrammar;

		private static readonly string k_moveCommands = "moveCommands";
		private static readonly string k_yesNoCommands = "yesNoCommands";
	}
}
