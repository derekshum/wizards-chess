using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WizardsChess.Movement.Drv;
using WizardsChess.Movement.Drv.Events;

namespace WizardsChess.Movement
{
	class MovePerformerVisualizer : IMovePerformer
	{

		public MovePerformerVisualizer()
		{
			int i;
			int j;

			for (j = 0; j < ySize; j++)
			{
				for (i = 0; i < xSize; i++)
				{
					if ((j > 0 && j < ySize - 1) && (i == 0 || i == xSize - 1))
						BoardRep[j, i] = 'o';
					else if ((j > 0 && j < ySize - 1) && (i == emptyCol1 || i == emptyCol2))
						BoardRep[j, i] = 'x';
					else if (i % 2 == 0 && j % 2 == 1 && i != 0 && i != xSize - 1)
						BoardRep[j, i] = ' ';
					else if (i % 2 == 1 && j % 2 == 0)
						BoardRep[j, i] = '+';
					else if (i % 2 == 1)
						BoardRep[j, i] = '|';
					else
						BoardRep[j, i] = '-';
				}
			}
		}

		public async Task MovePieceAsync(IList<Point2D> steps)
		{
			var start = steps[0];
			steps.RemoveAt(0);
			//Console.Writeline()
		}

		public async Task GoHomeAsync()
		{

		}

		public void EnableMagnet(bool enable) {	}	//unneeded

		public async Task MoveMotorAsync(Axis axis, int gridUnits)	{ }	//unneeded

		public char[,] BoardRep;
		const int xSize = 23;
		const int ySize = 17;
		int emptyCol1 = 2;
		int emptyCol2 = 20;

	}
}
