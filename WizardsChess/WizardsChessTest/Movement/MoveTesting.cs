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
			xGridMover = constructMover();
			yGridMover = constructMover();
			var magnet = new MockMagnet();
			movePerformer = new MovePerformer(xGridMover, yGridMover, magnet);
			moveManager = new MoveManager(movePlanner, movePerformer);
		}

		[TestMethod]
		public void TestMoveManagerMove()
		{
			TestMoveComponentCompilation();

			moveManager.MoveAsync(new Point2D(-1, -1), new Point2D(2, 1));
			Assert.AreEqual(xGridMover.GridPosition, 0, "The xMotor didn't return to position zero after a move.");
			Assert.AreEqual(yGridMover.GridPosition, 0, "The yMotor didn't return to position zero after a move.");
		}

		private IPreciseMotorMover xMover;
		private IPreciseMotorMover yMover;
		private IGridMotorMover xGridMover;
		private IGridMotorMover yGridMover;
		private IMovePerformer movePerformer;
		private IMovePlanner movePlanner;
		private IMoveManager moveManager;

		private IGridMotorMover constructMover()
		{
			var motor = MockMotor.Create();
			var locator = new MotorLocator(new MockGpio(), motor.Information);
			var signaler = new PositionSignaler(locator);
			var basicMover = new MotorMover(3, signaler, locator, motor);
			var topInterrupter = new MockPhotoInterrupter(1, 4, 6, locator, motor);
			var bottomInterrupter = new MockPhotoInterrupter(-1, -6, -4, locator, motor);
			var calibrator = new MotorCalibrator(-5, 5, basicMover, motor.Information, topInterrupter, bottomInterrupter);
			var preciseMover = new PreciseMotorMover(basicMover, calibrator);
			var gridMover = new GridMotorMover(preciseMover);
			return gridMover;
		}
	}
}
