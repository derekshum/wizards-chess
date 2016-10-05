using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WizardsChessApp.Chess;

namespace WizardsChessApp.Movement
{
	// This is a value type, meaning it is passed by value, not by reference.
	// Because it is a small value, this is generally a good thing.
	// It is immutable to avoid confusion when updating values and having to propagate those changes elsewhere.
	public struct Point2D
	{
		public Point2D(int x, int y)
		{
			X = x;
			Y = y;
		}

		public Point2D(Vector2D vec)
		{
			X = vec.X;
			Y = vec.Y;
		}

		public Point2D(Vector2D vec, Point2D origin)
		{
			X = vec.X + origin.X;
			Y = vec.Y + origin.Y;
		}

		public int X { get; }
		public int Y { get; }

		public override bool Equals(object obj)
		{
			if (obj is Point2D)
			{
				var p = (Point2D)obj;
				return p == this;
			}
			return base.Equals(obj);
		}

		public override string ToString() => $"[{X}, {Y}]";

		public override int GetHashCode() => 37 * Y + X;

		public static Point2D operator -(Point2D p) => new Point2D(-p.X, -p.Y);
		public static Point2D operator +(Point2D origin, Vector2D v) => new Point2D(origin.X + v.X, origin.Y + v.Y);
		public static Point2D operator +(Vector2D v, Point2D origin) => origin + v;
		public static Vector2D operator -(Point2D pA, Point2D pB) => new Vector2D(pA.X - pB.X, pA.Y - pB.Y);
		public static bool operator ==(Point2D pLhs, Point2D pRhs) => pLhs.X == pRhs.X && pLhs.Y == pRhs.Y;
		public static bool operator !=(Point2D pLhs, Point2D pRhs) => !(pLhs == pRhs);
	}
}
