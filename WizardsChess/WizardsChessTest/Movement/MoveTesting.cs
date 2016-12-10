using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WizardsChess.Chess;
using WizardsChess.Movement;
using WizardsChess.Movement.Drv;
using WizardsChess.VoiceControl;
using WizardsChess.VoiceControl.Commands;
using WizardsChess.VoiceControl.Events;
using WizardsChessTest.Mocks.Movement.Drv;

namespace WizardsChessTest.Movement
{
	[TestClass]
	public class MoveTesting
	{
		[TestMethod]
		public void TestMoveComponentCompilation()
		{
			ChessLogic logic = new ChessLogic();
			movePlanner = new MovePlanner(logic.Board);
			constructAnAxis(ref xMover, ref xCalibrator, ref xPreciseMover, ref xGridMover);
			constructAnAxis(ref yMover, ref yCalibrator, ref yPreciseMover, ref yGridMover);
			var magnet = new MockMagnet();
			movePerformer = new MovePerformer(xGridMover, yGridMover, magnet);
			moveManager = new MoveManager(movePlanner, movePerformer);
		}

		[TestMethod]
		public void TestMovePerformerMove()
		{
			TestMoveComponentCompilation();

			int targetX = 2;
			int targetY = -2;
			movePerformer.CalibrateAsync().Wait();
			var list = new List<Point2D>() { new Point2D(-1, -1), new Point2D(targetX, targetY) };
			movePerformer.MovePieceAsync(list).Wait();
			Assert.AreEqual(targetX, xGridMover.GridPosition,"The xMotor didn't end where expected.");
			Assert.AreEqual(targetY, yGridMover.GridPosition, "The yMotor didn't end where expected.");
			Assert.AreEqual(targetX, (int)Math.Round((float)xPreciseMover.Position / xPreciseMover.StepsPerGridUnit), "xPreciseMover was not where expected.");
			Assert.AreEqual(targetY, (int)Math.Round((float)yPreciseMover.Position / yPreciseMover.StepsPerGridUnit), "yPreciseMover was not where expected.");
		}

		private IMotorMover xMover;
		private IMotorMover yMover;
		private IMotorCalibrator xCalibrator;
		private IMotorCalibrator yCalibrator;
		private IPreciseMotorMover xPreciseMover;
		private IPreciseMotorMover yPreciseMover;
		private IGridMotorMover xGridMover;
		private IGridMotorMover yGridMover;
		private IMovePerformer movePerformer;
		private IMovePlanner movePlanner;
		private IMoveManager moveManager;

		private void constructAnAxis(ref IMotorMover mover, ref IMotorCalibrator calibrator, ref IPreciseMotorMover preciseMover, ref IGridMotorMover gridMover)
		{
			var motor = MockMotor.Create();
			var locator = new MotorLocator(new MockGpio(), motor.Information);
			var signaler = new PositionSignaler(locator);
			mover = new MotorMover(3, signaler, locator, motor);
			var topInterrupter = new MockPhotoInterrupter(1, 4, 6, locator, motor);
			var bottomInterrupter = new MockPhotoInterrupter(-1, -6, -4, locator, motor);
			calibrator = new MotorCalibrator(-5, 5, mover, motor.Information, topInterrupter, bottomInterrupter);
			preciseMover = new PreciseMotorMover(mover, calibrator);
			gridMover = new GridMotorMover(preciseMover);
		}
	}
}
