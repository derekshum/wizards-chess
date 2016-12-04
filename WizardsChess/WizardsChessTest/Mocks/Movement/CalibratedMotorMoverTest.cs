using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WizardsChess.Movement.Drv;
using WizardsChessTest.Mocks.Movement.Drv;
using WizardsChess.Movement;

namespace WizardsChessTest.Mocks.Movement
{
	[TestClass]
	public class CalibratedMotorMoverTest
	{
		public CalibratedMotorMoverTest()
		{
			mockMotor = new MockMotor();
			stepCounter = new StepCounter(mockMotor, new MockGpio());
			topInterrupter = new MockPhotoInterrupter(1, stepsPerGridUnit - 5, stepsPerGridUnit + 5, mockMotor);
			bottomInterrupter = new MockPhotoInterrupter(-1, -stepsPerGridUnit - 5, -stepsPerGridUnit + 5, mockMotor);

			calibratedMover = new CalibratedMotorMover(Axis.X, 10, -10, 15, mockMotor, stepCounter, topInterrupter, bottomInterrupter);
		}

		[TestMethod]
		public void TestMotorCalibration()
		{
			calibratedMover.CalibrateAsync().Wait();
			Assert.AreEqual(50, (int)Math.Round(calibratedMover.StepsPerGridUnit), "Steps per grid unit were incorrect.");
		}

		[TestMethod]
		public void TestOffsetMotorCalibration()
		{
			mockMotor.Position = 20;
			calibratedMover.CalibrateAsync().Wait();
			Assert.AreEqual(50, (int)Math.Round(calibratedMover.StepsPerGridUnit), "Steps per grid unit were incorrect.");
		}

		private IStepCounter stepCounter;
		private MockMotor mockMotor;
		private IPhotoInterrupter topInterrupter;
		private IPhotoInterrupter bottomInterrupter;

		private int stepsPerGridUnit = 50;

		private CalibratedMotorMover calibratedMover;
	}
}
