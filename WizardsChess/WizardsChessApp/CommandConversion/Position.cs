using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WizardsChessApp.CommandConversion
{
	enum ColumnLetter
	{
		A = 0,
		B = 1,
		C = 2,
		D = 3,
		E = 4,
		F = 5,
		G = 6,
		H = 7
	}

	class Position
	{
		public Position(string posLetter, string posNumber)
		{
			Column = getColumnLetter(posLetter);
			Row = Int32.Parse(posNumber);
		}

		public Position(Position pos)
		{
			Row = pos.Row;
			Column = pos.Column;
		}

		public int Row { get; set; }
		public ColumnLetter Column { get; set; }

		private ColumnLetter getColumnLetter(string posLetter)
		{
			switch (posLetter)
			{
				case "A":
					return ColumnLetter.A;
				case "B":
					return ColumnLetter.B;
				case "C":
					return ColumnLetter.C;
				case "D":
					return ColumnLetter.D;
				case "E":
					return ColumnLetter.E;
				case "F":
					return ColumnLetter.F;
				case "G":
					return ColumnLetter.G;
				case "H":
					return ColumnLetter.H;
				default:
					throw new ArgumentException($"String \"{posLetter}\" could not be converted to a ColumnLetter.");
			}
		}
	}
}
