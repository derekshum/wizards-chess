using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WizardsChessTest.Mocks.Movement.Drv;
using WizardsChess.Movement.Drv;
using WizardsChess.Movement.Drv.Events;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text;

namespace WizardsChessTest.Mocks.Movement
{
	[TestClass]
	public class StepCounterTest
	{
		public StepCounterTest()
		{
			mockMotor = new MockMotor();
		}

		[TestMethod]
		public void TestStepCounterConstruction()
		{
			stepCounter = new StepCounter(mockMotor, new MockGpio());

			stepCounter.FinishedCounting += finishedCounting;
			stepCounter.AdditionalStepsCounted += additionalStepsCounted;
			stepCounter.MoveTimedOut += timeout;
		}

		[TestMethod]
		public void TestStepCounterBasic()
		{
			if (stepCounter == null)
			{
				TestStepCounterConstruction();
			}

			resetTestVariables();

			isCounting = true;
			var expectedSteps = 20;
			stepCounter.CountSteps(expectedSteps, convertStepsToTimeOut(expectedSteps));
			runMotor();
			while (isCounting)
			{
				Task.Delay(50).Wait();
			}
			Assert.AreEqual(expectedSteps, stepsReceived, "Step count was off.");
			checkForErrors();
		}

		[TestMethod]
		public void TestStepCounterStall()
		{
			if (stepCounter == null)
			{
				TestStepCounterConstruction();
			}

			resetTestVariables();
			isTimeoutExpected = true;

			isCounting = true;
			stepCounter.CountSteps(80, convertStepsToTimeOut(80));
			runMotor();
			Task.Delay(20).Wait();
			stopMotor();
			while (isCounting)
			{
				Task.Delay(10).Wait();
			}
			Assert.IsTrue(timeoutOccurred, "Timeout did not occur during stall test.");
			checkForErrors();
		}

		[TestMethod]
		public void TestStepCounterOverlappingCounts()
		{
			if (stepCounter == null)
			{
				TestStepCounterConstruction();
			}

			resetTestVariables();

			isCounting = true;
			stepCounter.CountSteps(100, convertStepsToTimeOut(100));
			runMotor();
			Task.Delay(20).Wait();
			stepCounter.CountSteps(5, convertStepsToTimeOut(5));
			while(isCounting)
			{
				Task.Delay(10).Wait();
			}
			var firstStepsReceived = stepsReceived;
			Assert.IsTrue(stepsReceived < 100, $"Overlapping count step did not reset target count properly. Counted {stepsReceived} steps.");
			isCounting = true;
			Task.Delay(convertStepsToTimeOut(100)).Wait();
			Assert.AreEqual(firstStepsReceived, stepsReceived, "Counted more steps after delay!");
			checkForErrors();
		}

		[TestMethod]
		public void TestStepCounterStopMidCount()
		{
			if (stepCounter == null)
			{
				TestStepCounterConstruction();
			}

			resetTestVariables();

			isCounting = true;
			stepCounter.CountSteps(100, convertStepsToTimeOut(100));
			runMotor();
			Task.Delay(80).Wait();
			stepCounter.CountSteps(0, convertStepsToTimeOut(0));
			while (isCounting)
			{
				Task.Delay(10).Wait();
			}
			var firstStepsReceived = stepsReceived;
			Assert.IsTrue(stepsReceived < 100 && stepsReceived > 0, $"Overlapping count step did not reset target count properly. Counted {stepsReceived} steps which was not between 0 and 100.");
			checkForErrors();
		}

		private MockMotor mockMotor;
		private StepCounter stepCounter;

		private int stepsReceived;
		private int extraStepsReceived;
		private bool isTimeoutExpected;
		private bool timeoutOccurred;

		private bool isCounting;
		private bool isPassing;

		private Queue<string> errorMessages = new Queue<string>();

		private void finishedCounting(object sender, StepEventArgs e)
		{
			var pos = mockMotor.Position;
			assert(pos == e.NumSteps, $"Expected {pos} at finished counting, received {e.NumSteps}.");
			stepsReceived = e.NumSteps;
			stopMotor();
		}

		private void additionalStepsCounted(object sender, StepEventArgs e)
		{
			var pos = mockMotor.Position;
			extraStepsReceived = e.NumSteps;
			assert(pos == (stepsReceived + extraStepsReceived), $"Expected {pos} steps after additional counting, counted {stepsReceived + extraStepsReceived}.");
			isCounting = false;
		}

		private void timeout(object sender, StepEventArgs e)
		{
			assert(isTimeoutExpected, "Timeout occurred when not expected.");
			timeoutOccurred = true;
			isCounting = false;
		}

		private TimeSpan convertStepsToTimeOut(int steps)
		{
			return TimeSpan.FromMilliseconds(steps * 100);
		}

		private void stopMotor()
		{
			mockMotor.SetState(MotorState.Stopped);
		}

		private void runMotor()
		{
			mockMotor.SetState(MotorState.Forward);
		}

		private void resetTestVariables()
		{
			stepsReceived = 0;
			extraStepsReceived = 0;
			isTimeoutExpected = false;
			mockMotor.Position = 0;
			isPassing = true;
			timeoutOccurred = false;
		}

		private void assert(bool pass, string message)
		{
			if (!pass)
			{
				isPassing = false;
				errorMessages.Enqueue(message);
			}
		}

		private void checkForErrors()
		{
			if (isPassing)
				return;
			StringBuilder errorStrings = new StringBuilder();
			foreach(var msg in errorMessages)
			{
				errorStrings.Append(msg);
			}
			Assert.IsTrue(isPassing, errorStrings.ToString());
		}
	}
}
