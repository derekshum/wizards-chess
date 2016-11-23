using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WizardsChess.Movement
{
	public interface IMovePerformer
	{
		Task MovePiece(IList<Point2D> steps);
	}
}
