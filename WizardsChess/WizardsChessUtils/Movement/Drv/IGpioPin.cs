using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WizardsChess.Movement.Drv.Events;

namespace WizardsChess.Movement.Drv
{
	public enum GpioValue
	{
		Low,
		High
	}

	public interface IGpioPin
	{
		event GpioValueChangedEventHandler ValueChanged;

		int PinNum { get; }

		GpioValue Read();

		void Write(GpioValue val);
	}

	public delegate void GpioValueChangedEventHandler(object sender, GpioValueChangedEventArgs e);
}
