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

		public int PinNum { get; }

		public MockGpio() { PinNum = 1; }

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

		protected GpioValue value
		{
			get { return _value; }
			set
			{
				if (value != _value)
				{
					_value = value;
					if (_value == GpioValue.High)
					{
						edge = GpioEdge.RisingEdge;
					}
					else
					{
						edge = GpioEdge.FallingEdge;
					}
				}
			}
		}

		public GpioValue Read()
		{
			return value;
		}

		public void Write(GpioValue val)
		{
			value = val;
		}

		private GpioEdge _edge;
		private GpioValue _value;
	}
}
