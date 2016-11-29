using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WizardsChess.Movement
{
	public interface IMovePerformer
	{
		Task MovePiece(List<Point2D> steps);
		void GoHome();
	}
}
