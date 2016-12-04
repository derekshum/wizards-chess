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

		public PhotoInterrupter(int pinNum, int gridPosition) : base(pinNum, GpioPinDriveMode.InputPullUp)
		{
			GridPosition = gridPosition;
		}
	}
}
