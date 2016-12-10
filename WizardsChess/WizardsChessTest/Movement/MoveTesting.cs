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
			var movePlanner = new MovePlanner(logic.Board);
			var xMover = constructMover();
			var yMover = constructMover();
			var magnet = new MockMagnet();
			var moveManager = new MoveManager(movePlanner, new MovePerformer(xMover, yMover, magnet));
		}

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
