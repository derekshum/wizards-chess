using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WizardsChess.Movement.Drv
{
	public interface IMagnetDrv
	{
		void TurnOn();

		void TurnOff();

		bool IsOn { get; }
	}
}
