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
			uiDispatcher = Windows.UI.Core.CoreWindow.GetForCurrentThread().Dispatcher;
			GameManager = new VisualGameManager(uiDispatcher);
			//motorDriver = new MotorDrv(20, 21);
			//MotorTracker = new MotorTracker(23, motorDriver);
			//var gpios = GpioController.GetDefault();
			//electroMagPin = gpios.OpenPin(26);
			//electroMagPin.Write(GpioPinValue.Low);
			//electroMagPin.SetDriveMode(GpioPinDriveMode.Output);
			this.InitializeComponent();
		}

		private async void Button_Click(object sender, RoutedEventArgs e)
		{
			await GameManager.SetupCommandInterpreter();
			await GameManager.StartGameAsync();
		}

		public VisualGameManager GameManager { get; set; }

		private void MotorForward_Click(object sender, RoutedEventArgs e)
		{
			motorDriver.ChangeState(MotorState.Forward);
		}

		private void MotorBackward_Click(object sender, RoutedEventArgs e)
		{
			motorDriver.ChangeState(MotorState.Backward);
		}

		private void MotorStop_Click(object sender, RoutedEventArgs e)
		{
			motorDriver.ChangeState(MotorState.Stopped);
		}

		private void ElectroMagToggle_Click(object sender, RoutedEventArgs e)
		{
			if (electroMagPin.Read() == GpioPinValue.Low)
			{
				System.Diagnostics.Debug.WriteLine("Turning ON the electromagnet");
				electroMagPin.Write(GpioPinValue.High);
			}
			else
			{
				System.Diagnostics.Debug.WriteLine("Turning OFF the electromagnet");
				electroMagPin.Write(GpioPinValue.Low);
			}
		}

		private MotorDrv motorDriver;
		private GpioPin electroMagPin;
		public MotorTracker MotorTracker;
		private Windows.UI.Core.CoreDispatcher uiDispatcher;
	}
}
