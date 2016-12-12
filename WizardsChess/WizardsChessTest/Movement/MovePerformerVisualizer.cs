using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WizardsChess.Movement.Drv;
using WizardsChess.Movement.Drv.Events;

namespace WizardsChess.Movement
{
	public class MovePerformerVisualizer : IMovePerformer
	{

		public MovePerformerVisualizer()
		{
			BoardRep = new char[ySize, xSize];
		}

		public Task MovePieceAsync(IList<Point2D> steps)
		{
			Point2D convertedPoint;
			int i = 1;

			ResetBoardRep();

			foreach (var point in steps)
			{
				convertedPoint = ConvertPoint(point);
				BoardRep[convertedPoint.Y, convertedPoint.X] = i.ToString()[0];
				i++;
			}

			PrintBoardRep();

			return Task.FromResult(0);
		}

		public void ResetBoardRep()
		{
			for (int j = 0; j < ySize; j++)
			{
				for (int i = 0; i < xSize; i++)
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

		public void PrintBoardRep()
		{
			for (int j = 0; j < ySize; j++)
			{
				for (int i = 0; i < xSize; i++)
				{
					System.Diagnostics.Debug.Write(BoardRep[ySize - 1 - j, i]);
				}
				System.Diagnostics.Debug.WriteLine("");
			}
		}

		//converts input Points to Matrix indicies;
		public Point2D ConvertPoint(Point2D point)
		{
			return new Point2D(ConvertX(point.X), ConvertY(point.Y));
		}
		//converts (-11_11 to 0_23)
		public int ConvertX(int x)
		{
			return x + xOffset;
		}
		//convert (-8_8 to 0_16)
		public int ConvertY(int y)
		{
			return y + yOffset;
		}

		public Task GoHomeAsync()
		{
			return Task.FromResult(0);
		}

		public Task CalibrateAsync()
		{
			return Task.FromResult(0);
		}

		public void EnableMagnet(bool enable) {	}	//unneeded

		public Task MoveMotorAsync(Axis axis, int gridUnits)	{ return Task.FromResult(0); }	//unneeded

		public char[,] BoardRep;
		const int xSize = 23;
		const int ySize = 17;
		int emptyCol1 = 2;
		int emptyCol2 = 20;
		const int xOffset = (xSize - 1)/2;	//number added to input Point2D's to get boardMatrix index (-11_11 to 0_23)
		const int yOffset = (ySize - 1)/2;	//(-8_8 to 0_16)

	}
}
