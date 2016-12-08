﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Media.Playback;
using Windows.Media.SpeechSynthesis;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace WizardsChess.VoiceControl
{
	class Communicator : ICommunicator
	{
		public Communicator()
		{
			speechSynth = new SpeechSynthesizer();
			try
			{
				audioOut = Windows.Media.Playback.BackgroundMediaPlayer.Current;
			}
			catch (Exception e)
			{
				System.Diagnostics.Debug.WriteLine(e);
			}
			audioOut.MediaEnded += audioEventReceived;
			audioOut.MediaFailed += audioEventReceived;
			audioOut.MediaOpened += audioEventReceived;
		}

		public async Task Speak(string text)
		{
			await Threading.MarshallToUiThread(async () =>
			{
				if (audioOut.PlaybackSession.PlaybackState == MediaPlaybackState.Playing)
				{
					audioOut.Pause();
				}
				var voiceStream = await speechSynth.SynthesizeTextToStreamAsync(text);
				if (voiceStream == null)
				{
					System.Diagnostics.Debug.WriteLine($"Could not synthesize voice stream from text: {text}.");
				}
				
				audioOut.Source = Windows.Media.Core.MediaSource.CreateFromStream(voiceStream, voiceStream.ContentType);
				audioOut.Play();
			});
		}

		private void audioEventReceived(MediaPlayer sender, object e)
		{
			System.Diagnostics.Debug.WriteLine($"Received audio event: {e.ToString()}");
		}

		private SpeechSynthesizer speechSynth;
		private MediaPlayer audioOut;
	}
}
