using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Media.SpeechRecognition;

namespace WizardsChessApp.VoiceControl
{
	class CommandRecognizer
	{
		private CommandRecognizer()
		{
			SpeechRecognizer_ = new SpeechRecognizer();
		}

		public static async Task<CommandRecognizer> CreateAsync()
		{
			var recognizer = new CommandRecognizer();
			var compilationResult = await recognizer.SetupConstraintsAsync();
			if (compilationResult.Status != SpeechRecognitionResultStatus.Success)
			{
				throw new FormatException($"Could not compile grammar constraints. Received error {compilationResult.Status}");
			}
			return recognizer;
		}

		public async Task<SpeechRecognitionResult> RecognizeSpeechAsync()
		{
			return await SpeechRecognizer_.RecognizeWithUIAsync().AsTask();
		}

		protected async Task<SpeechRecognitionCompilationResult> SetupConstraintsAsync()
		{
			var grammarConstraints = await SpeechConstraints.GetConstraintsAsync();
			foreach (var constraint in grammarConstraints)
			{
				SpeechRecognizer_.Constraints.Add(constraint);
			}
			return await SpeechRecognizer_.CompileConstraintsAsync().AsTask();
		}

		private SpeechRecognizer SpeechRecognizer_;
	}
}
