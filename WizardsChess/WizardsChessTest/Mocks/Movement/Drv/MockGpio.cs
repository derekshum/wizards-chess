using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WizardsChess.Movement.Drv;
using WizardsChess.Movement.Drv.Events;

namespace WizardsChessTest.Mocks.Movement.Drv
{
	class MockGpio : IGpioPin
	{
		public event GpioValueChangedEventHandler ValueChanged;

		protected void onValueChanged()
		{
			ValueChanged?.Invoke(this, new GpioValueChangedEventArgs(_edge));
		}

		protected GpioEdge edge
		{
			get { return _edge; }
			set
			{
				if (value != _edge)
				{
					_edge = value;
					onValueChanged();
				}
			}
		}

		protected GpioValue value;

		public GpioValue Read()
		{
			return value;
		}

		public void Write(GpioValue val)
		{
			value = val;
		}

		private GpioEdge _edge;
	}
}
