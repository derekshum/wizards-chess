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
			CompileConstraintsTask_ = SpeechRecognizer_.CompileConstraintsAsync().AsTask();
		}

		public static async Task<CommandRecognizer> CreateAsync()
		{
			var recognizer = new CommandRecognizer();
			await recognizer.CompileConstraintsTask_;
			return recognizer;
		}

		public async Task<SpeechRecognitionResult> RecognizeSpeechAsync()
		{
			return await SpeechRecognizer_.RecognizeWithUIAsync().AsTask();
		}

		private SpeechRecognizer SpeechRecognizer_;
		private Task<SpeechRecognitionCompilationResult> CompileConstraintsTask_;
	}
}
