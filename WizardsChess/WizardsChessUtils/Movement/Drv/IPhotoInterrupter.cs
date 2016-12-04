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
		int GridPosition { get; }
	}
}
