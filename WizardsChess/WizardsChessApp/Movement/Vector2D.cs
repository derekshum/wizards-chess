using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WizardsChessApp.Movement
{
	// This is a value type, meaning it is passed by value, not by reference.
	// Because it is a small value, this is generally a good thing.
	// It is immutable to avoid confusion when updating values and having to propagate those changes elsewhere.
	struct Vector2D
	{
		public Vector2D(int x, int y)
		{
			X = x;
			Y = y;
		}

		public Vector2D(Point2D p)
		{
			X = p.X;
			Y = p.Y;
		}

		public int X { get; }
		public int Y { get; }

		public Vector2D FlipY() => new Vector2D(X, -Y);
		public Vector2D FlipX() => new Vector2D(-X, Y);

		public override bool Equals(object obj)
		{
			if (obj is Vector2D)
			{
				var v = (Vector2D)obj;
				return v == this;
			}
			return base.Equals(obj);
		}

		public override string ToString()
		{
			return $"[{X}, {Y}]";
		}

		public static Vector2D operator -(Vector2D vec) => new Vector2D(-vec.X, -vec.Y);
		public static Vector2D operator +(Vector2D vLhs, Vector2D vRhs) => new Vector2D(vLhs.X + vRhs.X, vLhs.Y + vRhs.Y);
		public static Vector2D operator -(Vector2D vLhs, Vector2D vRhs) => new Vector2D(vLhs.X - vRhs.X, vLhs.Y - vRhs.Y);
		public static bool operator ==(Vector2D vLhs, Vector2D vRhs) => vLhs.X == vRhs.X && vLhs.Y == vRhs.Y;
		public static bool operator !=(Vector2D vLhs, Vector2D vRhs) => !(vLhs == vRhs);
	}
}
