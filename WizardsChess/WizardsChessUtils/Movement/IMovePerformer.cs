using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WizardsChess.Movement
{
	public interface IMovePerformer
	{
		Task MovePieceAsync(IList<Point2D> steps);
		Task MoveMotorAsync(Axis axis, int gridUnits);
		Task GoHomeAsync();
		Task CalibrateAsync();
		void EnableMagnet(bool enable);
	}
}
