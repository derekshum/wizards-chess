using System;
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
using WizardsChessApp.Movement.Drv;
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
			MotorDriver = new MotorDrv(16, 18);
            this.InitializeComponent();
		}

		private async void Button_Click(object sender, RoutedEventArgs e)
		{
			await GameManager.PerformCommandAsync();
		}

		public VisualGameManager GameManager { get; set; }

		private void MotorForward_Click(object sender, RoutedEventArgs e)
		{
			MotorDriver.ChangeState(MotorState.Forward);
		}

		private void MotorBackward_Click(object sender, RoutedEventArgs e)
		{
			MotorDriver.ChangeState(MotorState.Backward);
		}

		private void MotorStop_Click(object sender, RoutedEventArgs e)
		{
			MotorDriver.ChangeState(MotorState.Stopped);
		}

		private MotorDrv MotorDriver;
	}
}
