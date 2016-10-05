﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using WizardsChessApp.AppDebugging;
using WizardsChessApp.VoiceControl;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace WizardsChessApp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
			GameManager = new VisualGameManager();
            this.InitializeComponent();
		}

		private async void Button_Click(object sender, RoutedEventArgs e)
		{
			System.Diagnostics.Debug.WriteLine("Starting task\n");

			var cmdRecognizer = await CommandRecognizer.CreateAsync();
			var result = await cmdRecognizer.RecognizeSpeechAsync();

			if (result.Status == Windows.Media.SpeechRecognition.SpeechRecognitionResultStatus.Success)
			{
				foreach (var property in result.SemanticInterpretation.Properties)
				{
					System.Diagnostics.Debug.WriteLine($"Retrieved Key: {property.Key}");

					foreach (var innerProp in property.Value)
					{
						System.Diagnostics.Debug.WriteLine($"\tValue: {innerProp}");
					}
				}

				System.Diagnostics.Debug.WriteLine(result.Text + "\nConfidence: " + result.Confidence);
			}
		}

		public VisualGameManager GameManager { get; set; }
	}
}
