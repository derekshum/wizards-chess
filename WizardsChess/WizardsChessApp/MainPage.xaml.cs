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
using WizardsChess.AppDebugging;
using WizardsChess.VoiceControl;
using WizardsChess.Movement;
using WizardsChess.Movement.Drv;
using Windows.Devices.Gpio;
using System.Threading.Tasks;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace WizardsChess
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
			var uiDispatcher = Windows.UI.Core.CoreWindow.GetForCurrentThread().Dispatcher;
			GameManager = new VisualGameManager(uiDispatcher);
			//var motorDriverX = new MotorDrv(23, 24);
			//var motorDriverY = new MotorDrv(20, 21);
			//var magnetDriver = new MagnetDrv(26);
			//var stepCounterX = new StepCounter(6, 19);
			//var stepCounterY = new StepCounter(5, 13);
			//movePerformer = new MovePerformer(motorDriverX, motorDriverY, magnetDriver, stepCounterX, stepCounterY);
			this.InitializeComponent();
		}

		private async void Button_Click(object sender, RoutedEventArgs e)
		{
			await GameManager.SetupCommandInterpreter();
			await GameManager.StartGameAsync();
		}

		public VisualGameManager GameManager { get; set; }

		private void MotorMoveX_Click(object sender, RoutedEventArgs e)
		{
			movePerformer.MoveMotorAsync(Axis.X, 1000);
		}

		private void MotorMoveY_Click(object sender, RoutedEventArgs e)
		{
			movePerformer.MoveMotorAsync(Axis.Y, 1000);
		}

		private void MotorStop_Click(object sender, RoutedEventArgs e)
		{
			movePerformer.MoveMotorAsync(Axis.X, 0);
			movePerformer.MoveMotorAsync(Axis.Y, 0);
		}

		private void ElectroMagToggle_Click(object sender, RoutedEventArgs e)
		{
			if (isElectroMagOn)
			{
				System.Diagnostics.Debug.WriteLine("Turning OFF the electromagnet");
				isElectroMagOn = false;
				movePerformer.EnableMagnet(isElectroMagOn);
			}
			else
			{
				System.Diagnostics.Debug.WriteLine("Turning ON the electromagnet");
				isElectroMagOn = true;
				movePerformer.EnableMagnet(isElectroMagOn);
			}
		}

		private MovePerformer movePerformer;
		private bool isElectroMagOn = false;
	}
}
