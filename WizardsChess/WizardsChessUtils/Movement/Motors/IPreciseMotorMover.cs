using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WizardsChess.Movement
{
	public interface IPreciseMotorMover
	{
		int Position { get; }
		float StepsPerGridUnit { get; }
		Task GoToPositionAsync(int position);
		Task CalibrateAsync();
	}
}
