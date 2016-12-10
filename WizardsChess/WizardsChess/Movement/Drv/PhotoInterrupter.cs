using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Gpio;
using WizardsChess.Movement.Drv.Events;

namespace WizardsChess.Movement.Drv
{
	class PhotoInterrupter : GpioPinWrapper, IPhotoInterrupter
	{
		public int GridPosition { get; }

		public int StepPosition { get; set; }

		public PhotoInterrupter(int pinNum, int gridPosition, int estStepPosition) : base(pinNum, GpioPinDriveMode.InputPullUp)
		{
			GridPosition = gridPosition;
			StepPosition = estStepPosition;
		}
	}
}
