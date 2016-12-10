using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WizardsChess.Movement.Drv.Events;

namespace WizardsChess.Movement.Drv
{
	public interface IPhotoInterrupter : IGpioPin
	{
		/// <summary>
		/// The grid position this interrupter is located, as defined by the physical setup.
		/// </summary>
		int GridPosition { get; }

		/// <summary>
		/// The step position we should expect to find the interrupter, as defined by calibration.
		/// </summary>
		int StepPosition { get; set; }
	}
}
