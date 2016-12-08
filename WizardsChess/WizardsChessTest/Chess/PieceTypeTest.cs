using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WizardsChess.Chess;


namespace WizardsChessTest
{
	[TestClass]
	public class PieceTypeTest
	{
		[TestMethod]
		public void TestStringConversion()
		{
			Assert.AreEqual(PieceType.King, PieceTypeMethods.Parse("king"));
			Assert.AreEqual(PieceType.King, PieceTypeMethods.Parse("KING"));
			Assert.AreEqual(PieceType.Queen, PieceTypeMethods.Parse("queen"));
			Assert.AreEqual(PieceType.Queen, PieceTypeMethods.Parse("QUEEN"));
			Assert.AreEqual(PieceType.Bishop, PieceTypeMethods.Parse("bishop"));
			Assert.AreEqual(PieceType.Bishop, PieceTypeMethods.Parse("BISHOP"));
			Assert.AreEqual(PieceType.Knight, PieceTypeMethods.Parse("knight"));
			Assert.AreEqual(PieceType.Knight, PieceTypeMethods.Parse("KNIGHT"));
			Assert.AreEqual(PieceType.Rook, PieceTypeMethods.Parse("rook"));
			Assert.AreEqual(PieceType.Rook, PieceTypeMethods.Parse("ROOK"));
			Assert.AreEqual(PieceType.Rook, PieceTypeMethods.Parse("castle"));
			Assert.AreEqual(PieceType.Rook, PieceTypeMethods.Parse("CASTLE"));
			Assert.AreEqual(PieceType.Pawn, PieceTypeMethods.Parse("pawn"));
			Assert.AreEqual(PieceType.Pawn, PieceTypeMethods.Parse("PAWN"));
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentException))]
		public void TestFailedStringConversion()
		{
			PieceTypeMethods.Parse("");
			PieceTypeMethods.Parse("test");
		}
	}
}
