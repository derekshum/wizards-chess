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

	static class CommandConstraintsGenerator
	{
		public static async Task<IList<ISpeechRecognitionConstraint>> GetConstraintsAsync()
		{
			var grammarList = new List<ISpeechRecognitionConstraint>();

			var commandsFileTask = StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///VoiceControl/MoveCommands.grxml")).AsTask();
			var yesNoFileTask = StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///VoiceControl/YesNoCommands.grxml")).AsTask();
			await Task.WhenAll(commandsFileTask, yesNoFileTask);

			var moveGrammar = new SpeechRecognitionGrammarFileConstraint(commandsFileTask.Result, k_moveCommands);
			var yesNoGrammar = new SpeechRecognitionGrammarFileConstraint(yesNoFileTask.Result, k_yesNoCommands);

			grammarList.Add(moveGrammar);
			grammarList.Add(yesNoGrammar);

			return grammarList;
		}

		public static void EnableGrammar(IList<ISpeechRecognitionConstraint> constraints, GrammarConstraints constraintTag, bool enable)
		{
			string tag;
			switch (constraintTag)
			{
				case GrammarConstraints.MoveCommands:
					tag = k_moveCommands;
					break;
				case GrammarConstraints.YesNoCommands:
					tag = k_yesNoCommands;
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
	}
}
