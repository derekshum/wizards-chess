using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WizardsChess.Movement
{
	public interface IGridMotorMover
	{
		int GridPosition { get; }
		Task GoToPositionAsync(int gridPos);
		Task CalibrateAsync();
	}
}
