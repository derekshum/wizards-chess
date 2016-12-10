using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WizardsChess.Movement.Drv;

namespace WizardsChessTest.Mocks.Movement.Drv
{
	class MockMagnet : IMagnetDrv
	{
		public bool IsOn
		{
			get;
			private set;
		}

		public void TurnOff()
		{
			IsOn = false;
		}

		public void TurnOn()
		{
			IsOn = true;
		}
	}
}
