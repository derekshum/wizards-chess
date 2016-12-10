using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WizardsChess.Movement.Drv.Events;

namespace WizardsChess.Movement.Drv
{
	public interface IMotorInformation
	{
		IGpioPin SteppingPin { get; }
		MoveDirection Direction { get; }
		Axis Axis { get; }
		event MotorDirectionChangedEventHandler DirectionChanged;
	}

	public delegate void MotorDirectionChangedEventHandler(object sender, MotorDirectionChangedEventArgs e);
}
