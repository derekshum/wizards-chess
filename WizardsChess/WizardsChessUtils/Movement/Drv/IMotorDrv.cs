using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WizardsChess.Movement.Drv
{
	public enum MotorState
	{
		Stopped,
		Forward,
		Backward
	}

	public interface IMotorDrv
	{
		void SetState(MotorState state);

		MotorState State { get; }
	}

}
