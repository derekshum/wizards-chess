using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WizardsChessTest.Mocks.Movement.Drv;
using WizardsChess.Movement.Drv;
using WizardsChess.Movement.Drv.Events;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text;
using WizardsChess.Movement;
using WizardsChess.Movement.Events;

namespace WizardsChessTest.Mocks.Movement
{
	[TestClass]
	public class StepCounterTest
	{
		public StepCounterTest()
		{
			mockMotor = new MockMotor();
		}

		[TestCategory("Step Counter")]
		[TestMethod]
		public void TestStepCounterConstruction()
		{
			motorLocator = new MotorLocator(mockMotor, mockMotor);
			stepCounter = new StepCounter(motorLocator, new MockGpio());

			stepCounter.FinishedCounting += finishedCounting;
			stepCounter.AdditionalStepsCounted += additionalStepsCounted;
			stepCounter.MoveTimedOut += timeout;
		}

		[TestCategory("Step Counter")]
		[TestMethod]
		public void TestStepCounterBasic()
		{
			if (stepCounter == null)
			{
				TestStepCounterConstruction();
			}

			resetTestVariables();

			isCounting = true;
			var expectedPosition = 20;
			stepCounter.CountToPosition(expectedPosition, convertPositionToTimeout(expectedPosition));
			runMotor();
			while (isCounting)
			{
				Task.Delay(50).Wait();
			}
			Assert.AreEqual(expectedPosition, positionOnTargetReached, "Step count was off.");
			Assert.IsTrue(expectedPosition < positionOnExtraSteps, "Additional steps counted correctly.");
			checkForErrors();
		}

		[TestCategory("Step Counter")]
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
			stepCounter.CountToPosition(80, convertPositionToTimeout(80));
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

		[TestCategory("Step Counter")]
		[TestMethod]
		public void TestStepCounterOverlappingCounts()
		{
			if (stepCounter == null)
			{
				TestStepCounterConstruction();
			}

			resetTestVariables();

			isCounting = true;
			stepCounter.CountToPosition(100, convertPositionToTimeout(100));
			runMotor();
			Task.Delay(20).Wait();
			var shorterTarget = motorLocator.Position + 10;
			stepCounter.CountToPosition(shorterTarget, convertPositionToTimeout(shorterTarget));
			while(isCounting)
			{
				Task.Delay(10).Wait();
			}
			var firstTargetPositionReached = positionOnTargetReached;
			Assert.IsTrue(firstTargetPositionReached == shorterTarget, $"Overlapping count step did not reset target count properly. Counted to position {firstTargetPositionReached}.");
			isCounting = true;
			Task.Delay(convertPositionToTimeout(100)).Wait();
			Assert.AreEqual(firstTargetPositionReached, positionOnTargetReached, "Counted more steps after delay!");
			checkForErrors();
		}

		[TestCategory("Step Counter")]
		[TestMethod]
		public void TestStepCounterStopMidCount()
		{
			if (stepCounter == null)
			{
				TestStepCounterConstruction();
			}

			resetTestVariables();

			isCounting = true;
			stepCounter.CountToPosition(100, convertPositionToTimeout(100));
			runMotor();
			Task.Delay(80).Wait();
			var pos = motorLocator.Position;
			stepCounter.CountToPosition(pos, convertPositionToTimeout(pos));
			while (isCounting)
			{
				Task.Delay(10).Wait();
			}
			Assert.AreEqual(pos, positionOnTargetReached, $"Overlapping count step did not reset target count properly. Expected {pos} and reached target {positionOnTargetReached}.");
			Assert.AreEqual(pos, positionOnExtraSteps, $"Extra steps position {positionOnExtraSteps} did not equal expected position of {pos}.");
			checkForErrors();
		}

		private MockMotor mockMotor;
		private IMotorLocator motorLocator;
		private StepCounter stepCounter;

		private int positionOnTargetReached;
		private int positionOnExtraSteps;
		private bool isTimeoutExpected;
		private bool timeoutOccurred;

		private bool isCounting;
		private bool isPassing;

		private Queue<string> errorMessages = new Queue<string>();

		private void finishedCounting(object sender, PositionChangedEventArgs e)
		{
			var pos = motorLocator.Position;
			assert(pos == e.Position, $"Expected {pos} at finished counting, received {e.Position}.");
			positionOnTargetReached = e.Position;
			stopMotor();
		}

		private void additionalStepsCounted(object sender, PositionChangedEventArgs e)
		{
			var pos = motorLocator.Position;
			assert(pos == e.Position, $"Expected {pos} after additional counting, received {e.Position}.");
			positionOnExtraSteps = e.Position;
			isCounting = false;
		}

		private void timeout(object sender, PositionChangedEventArgs e)
		{
			assert(isTimeoutExpected, "Timeout occurred when not expected.");
			timeoutOccurred = true;
			isCounting = false;
		}

		private TimeSpan convertPositionToTimeout(int position)
		{
			return TimeSpan.FromMilliseconds((position - motorLocator.Position) * 100);
		}

		private void stopMotor()
		{
			mockMotor.Direction = MoveDirection.Stopped;
		}

		private void runMotor()
		{
			mockMotor.Direction = MoveDirection.Forward;
		}

		private void resetTestVariables()
		{
			isTimeoutExpected = false;
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
