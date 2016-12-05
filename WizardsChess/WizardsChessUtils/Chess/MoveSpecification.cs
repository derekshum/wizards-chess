using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WizardsChess.Chess
{
	public class MoveSpecification
	{
		public MoveSpecification (Position s, Position e, Position? cap = null, bool hasMovedChange = false, bool cast = false)
		{
			Start = s;
			End = e;
			Capture = cap;
			HasMovedChange = hasMovedChange;
			Castle = cast;
		}
		/*
		public MoveSpecification Get()
		{
			return new MoveSpecification(start, end, capture);
		}
		public Position GetStart()
		{
			return start;
		}
		public Position GetEnd()
		{
			return end;
		}
		public Position? GetCapture()
		{
			return capture;
		}
		*/
		public Position Start;	//end position of the move
		public Position End;	//end position of the move
		public Position? Capture;   //true if this move involved capturing a piece
		public bool HasMovedChange;	//whether or not this move was that piece moving for the first time. Not updated on castling, but accounted for in castling undo
		public bool Castle;		//whether or not the move was a castle
	}
}
