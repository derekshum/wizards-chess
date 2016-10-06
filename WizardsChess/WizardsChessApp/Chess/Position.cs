using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WizardsChessApp.Movement;

namespace WizardsChessApp.Chess
{
	public enum ColumnLetter
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

	public struct Position
	{
		public Position(string posLetter, string posNumber)
		{
			ColumnLetter = (Enum.Parse(typeof(ColumnLetter), posLetter, true) as ColumnLetter?).Value;
			// Convert from 1-based indexing to zero-based indexing
			Row = Int32.Parse(posNumber) - 1;
		}

		public Position(Position pos)
		{
			Row = pos.Row;
			ColumnLetter = pos.ColumnLetter;
		}

		public ColumnLetter ColumnLetter { get; /*set;*/ }
		public int Row { get; /*set;*/ }
		public int Column
		{
			get { return (int)ColumnLetter; }
			//set { ColumnLetter = (ColumnLetter)value; }
		}

		public override string ToString() => $"{ColumnLetter}{Row}";

		public static implicit operator Point2D(Position p) => new Point2D(p.Column, p.Row);
	}
}
