using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Media.SpeechSynthesis;
using Windows.UI.Xaml.Controls;

namespace WizardsChess.VoiceControl
{
	public class Communicator : ICommunicator
	{
		public Communicator()
		{
			speechSynth = new SpeechSynthesizer();
			audioOut = new MediaElement();
		}

		public async Task Speak(string text)
		{
			if (audioOut.CurrentState == Windows.UI.Xaml.Media.MediaElementState.Playing)
			{
				audioOut.Stop();
			}
			var voiceStream = await speechSynth.SynthesizeTextToStreamAsync(text);
			audioOut.SetSource(voiceStream, voiceStream.ContentType);
			audioOut.Play();
		}

		private SpeechSynthesizer speechSynth;
		private MediaElement audioOut;
	}
}
