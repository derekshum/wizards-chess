using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Media.SpeechSynthesis;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using WizardsChess.AppDebugging;

namespace WizardsChess.VoiceControl
{
	public class Communicator : ICommunicator
	{
		public Communicator()
		{
			speechSynth = new SpeechSynthesizer();
			audioOut = new MediaElement();
			audioOut.MediaEnded += audioEventReceived;
			audioOut.MediaFailed += audioEventReceived;
			audioOut.MediaOpened += audioEventReceived;
		}

		public async Task Speak(string text)
		{
			await Threading.MarshallToUiThread(async () =>
			{
				if (audioOut.CurrentState == Windows.UI.Xaml.Media.MediaElementState.Playing)
				{
					audioOut.Stop();
				}
				var voiceStream = await speechSynth.SynthesizeTextToStreamAsync(text);
				if (voiceStream == null)
				{
					System.Diagnostics.Debug.WriteLine($"Could not synthesize voice stream from text: {text}.");
				}
				audioOut.SetSource(voiceStream, voiceStream.ContentType);
				audioOut.Play();
			});
		}

		private void audioEventReceived(object sender, RoutedEventArgs e)
		{
			System.Diagnostics.Debug.WriteLine($"Received audio event: {e.ToString()}");
		}

		private SpeechSynthesizer speechSynth;
		private MediaElement audioOut;
	}
}
