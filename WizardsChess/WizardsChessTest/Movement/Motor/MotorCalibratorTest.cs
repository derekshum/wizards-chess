using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WizardsChessTest.Mocks.Movement.Drv;
using WizardsChess.Movement;
using WizardsChess.Movement.Drv;

namespace WizardsChessTest.Movement
{
	[TestClass]
	public class MotorCalibratorTest
	{
		[TestMethod]
		public void TestMotorCalibrationBasic()
		{
			setup();

			calibrator.CalibrateAsync().Wait();
			checkResults();
		}

		[TestMethod]
		public void TestMotorCalibrationAboveInterrupts()
		{
			setup();

			locator.ShiftPosition(10);
			calibrator.CalibrateAsync().Wait();
			checkResults();
		}

		[TestMethod]
		public void TestMotorCalibrationBelowInterrupts()
		{
			setup();

			locator.ShiftPosition(-10);
			calibrator.CalibrateAsync().Wait();
			checkResults();
		}

		[TestMethod]
		public void TestMotorCalibratorDetectingDecalibration()
		{
			setup();

			calibrator.CalibrateAsync().Wait();

			topInterrupt.UpperStepPosition = 20;
			topInterrupt.LowerStepPosition = 18;
			bottomInterrupt.UpperStepPosition = 12;
			bottomInterrupt.LowerStepPosition = 10;

			mover.GoToPositionAsync(0).Wait();
			mover.GoToPositionAsync(13).Wait();

			Assert.AreEqual(CalibrationState.NeedsCalibrating, calibrator.State, "Calibrator didn't register the need for recalibrating.");
			calibrator.CalibrateAsync().Wait();

			var oldTopIntPos = topIntPos;
			topIntPos = 4;
			var oldBottomIntPos = bottomIntPos;
			bottomIntPos = -4;
			var oldStepsPerGrid = stepsPerGrid;
			stepsPerGrid = 4;
			checkResults();
			topIntPos = oldTopIntPos;
			bottomIntPos = oldBottomIntPos;
			stepsPerGrid = oldStepsPerGrid;
		}

		private MockMotor mockMotor;
		private IMotorLocator locator;
		private IPositionSignaler signaler;
		private IMotorMover mover;
		private MockPhotoInterrupter topInterrupt;
		private MockPhotoInterrupter bottomInterrupt;
		private IMotorCalibrator calibrator;

		private int upperTopIntPos = 8;
		private int lowerTopIntPos = 6;
		private int topIntPos = 7;
		private int upperBottomIntPos = -6;
		private int lowerBottomIntPos = -8;
		private int bottomIntPos = -7;
		private int stepsPerGrid = 7;

		private void setup()
		{
			mockMotor = MockMotor.Create();
			locator = new MotorLocator(new MockGpio(), mockMotor.Information);
			signaler = new PositionSignaler(locator);
			mover = new MotorMover(3, signaler, locator, mockMotor);
			topInterrupt = new MockPhotoInterrupter(1, lowerTopIntPos, upperTopIntPos, locator, mockMotor);
			bottomInterrupt = new MockPhotoInterrupter(-1, lowerBottomIntPos, upperBottomIntPos, locator, mockMotor);
			calibrator = new MotorCalibrator(-5, 5, mover, mockMotor.Information, topInterrupt, bottomInterrupt);
		}

		private void checkResults()
		{
			Assert.AreEqual(CalibrationState.Ready, calibrator.State, "Calibrator isn't in the Ready state after callibration.");
			Assert.AreEqual(topIntPos, topInterrupt.StepPosition, "Top interrupt was calibrated to the wrong place");
			Assert.AreEqual(bottomIntPos, bottomInterrupt.StepPosition, "Bottom interrupt was calibrated to the wrong place.");
			Assert.AreEqual(stepsPerGrid, calibrator.StepsPerGridUnit, "Steps per grid unit was calibrated wrong.");
		}
	}
}
