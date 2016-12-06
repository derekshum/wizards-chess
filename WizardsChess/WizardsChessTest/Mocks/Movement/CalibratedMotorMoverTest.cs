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
		}

		[TestCategory("Motor Calibration")]
		[TestMethod]
		public void TestMotorCalibration()
		{
			constructFreshCalibrator();
			calibratedMover.CalibrateAsync().Wait();
			Assert.AreEqual(50, (int)Math.Round(calibratedMover.StepsPerGridUnit), "Steps per grid unit were incorrect.");
			Assert.IsTrue(calibratedMover.StepPosition < -50, $"Final position {calibratedMover.StepPosition} should be less than -50.");
		}

		[TestCategory("Motor Calibration")]
		[TestMethod]
		public void TestOffsetMotorCalibration()
		{
			mockMotor.Position = 20;
			constructFreshCalibrator();
			calibratedMover.CalibrateAsync().Wait();
			Assert.AreEqual(50, (int)Math.Round(calibratedMover.StepsPerGridUnit), "Steps per grid unit were incorrect.");
			Assert.IsTrue(calibratedMover.StepPosition < -50, $"Final position {calibratedMover.StepPosition} should be less than -50.");
		}

		[TestCategory("Motor Calibration")]
		[TestMethod]
		public void TestMotorCalibrationFromAboveInterrupts()
		{
			mockMotor.Position = 100;
			constructFreshCalibrator();
			calibratedMover.CalibrateAsync().Wait();
			Assert.AreEqual(50, (int)Math.Round(calibratedMover.StepsPerGridUnit), "Steps per grid unit were incorrect.");
			Assert.IsTrue(calibratedMover.StepPosition < -50, $"Final position {calibratedMover.StepPosition} should be less than -50.");
		}

		[TestCategory("Motor Calibration")]
		[TestMethod]
		public void TestMotorCalibrationFromBelowInterrupts()
		{
			mockMotor.Position = -100;
			constructFreshCalibrator();
			calibratedMover.CalibrateAsync().Wait();
			Assert.AreEqual(50, (int)Math.Round(calibratedMover.StepsPerGridUnit), "Steps per grid unit were incorrect.");
			Assert.IsTrue(calibratedMover.StepPosition > 50, $"Final position {calibratedMover.StepPosition} should be greater than 50.");
		}

		private void constructFreshCalibrator()
		{
			calibratedMover = new CalibratedMotorMover(Axis.X, 3, -3, 20, mockMotor, stepCounter, topInterrupter, bottomInterrupter);
		}

		private IStepCounter stepCounter;
		private MockMotor mockMotor;
		private IPhotoInterrupter topInterrupter;
		private IPhotoInterrupter bottomInterrupter;

		private int stepsPerGridUnit = 50;

		private CalibratedMotorMover calibratedMover;
	}
}
