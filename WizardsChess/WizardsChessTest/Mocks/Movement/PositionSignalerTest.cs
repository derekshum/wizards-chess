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
	public class PositionSignalerTest
	{
		public PositionSignalerTest()
		{
			mockMotor = new MockMotor();
		}

		[TestCategory(nameof(PositionSignaler))]
		[TestMethod]
		public void TestPositionSignalerConstruction()
		{
			motorLocator = new MotorLocator(mockMotor, mockMotor);
			positionSignaler = new PositionSignaler(motorLocator, new MockGpio());

			positionSignaler.FinishedCounting += finishedCounting;
			positionSignaler.AdditionalStepsCounted += additionalStepsCounted;
			positionSignaler.MoveTimedOut += timeout;
		}

		[TestCategory(nameof(PositionSignaler))]
		[TestMethod]
		public void TestPositionSignalerBasic()
		{
			if (positionSignaler == null)
			{
				TestPositionSignalerConstruction();
			}

			resetTestVariables();

			isCounting = true;
			var expectedPosition = 20;
			positionSignaler.CountToPosition(expectedPosition, convertPositionToTimeout(expectedPosition));
			runMotor();
			while (isCounting)
			{
				Task.Delay(50).Wait();
			}
			Assert.AreEqual(expectedPosition, positionOnTargetReached, "Step count was off.");
			Assert.IsTrue(expectedPosition < positionOnExtraSteps, "Additional steps counted correctly.");
			checkForErrors();
		}

		[TestCategory(nameof(PositionSignaler))]
		[TestMethod]
		public void TestPositionSignalerStall()
		{
			if (positionSignaler == null)
			{
				TestPositionSignalerConstruction();
			}

			resetTestVariables();
			isTimeoutExpected = true;

			isCounting = true;
			positionSignaler.CountToPosition(80, convertPositionToTimeout(80));
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

		[TestCategory(nameof(PositionSignaler))]
		[TestMethod]
		public void TestPositionSignalerOverlappingCounts()
		{
			if (positionSignaler == null)
			{
				TestPositionSignalerConstruction();
			}

			resetTestVariables();

			isCounting = true;
			positionSignaler.CountToPosition(100, convertPositionToTimeout(100));
			runMotor();
			Task.Delay(20).Wait();
			var shorterTarget = motorLocator.Position + 10;
			positionSignaler.CountToPosition(shorterTarget, convertPositionToTimeout(shorterTarget));
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

		[TestCategory(nameof(PositionSignaler))]
		[TestMethod]
		public void TestPositionSignalerStopMidCount()
		{
			if (positionSignaler == null)
			{
				TestPositionSignalerConstruction();
			}

			resetTestVariables();

			isCounting = true;
			positionSignaler.CountToPosition(100, convertPositionToTimeout(100));
			runMotor();
			Task.Delay(80).Wait();
			var pos = motorLocator.Position;
			positionSignaler.CountToPosition(pos, convertPositionToTimeout(pos));
			while (isCounting)
			{
				Task.Delay(10).Wait();
			}
			Assert.AreEqual(pos, positionOnTargetReached, $"Overlapping count step did not reset target count properly. Expected {pos} and reached target {positionOnTargetReached}.");
			Assert.AreEqual(pos, positionOnExtraSteps, $"Extra steps position {positionOnExtraSteps} did not equal expected position of {pos}.");
			checkForErrors();
		}

		[TestCategory(nameof(PositionSignaler))]
		[TestMethod]
		public void TestPositionSignalerStepAfterStopping()
		{
			if (positionSignaler == null)
			{
				TestPositionSignalerConstruction();
			}

			resetTestVariables();

			isCounting = true;
			positionSignaler.CountToPosition(10, convertPositionToTimeout(10));
			runMotor();
			var start = DateTime.Now;
			while (isCounting)
			{
				Task.Delay(15).Wait();
			}
			var end = DateTime.Now;

			runMotor(MoveDirection.Backward);
			Task.Delay((end - start).Milliseconds + 30).Wait();
			stopMotor();

			checkForErrors();
		}

		private MockMotor mockMotor;
		private IMotorLocator motorLocator;
		private PositionSignaler positionSignaler;

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
			assert(isCounting, "Finished counting when not in counting state.");
			positionOnTargetReached = e.Position;
			stopMotor();
		}

		private void additionalStepsCounted(object sender, PositionChangedEventArgs e)
		{
			var pos = motorLocator.Position;
			assert(pos == e.Position, $"Expected {pos} after additional counting, received {e.Position}.");
			assert(isCounting, "Counted additional steps when not in counting state.");
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

		private void runMotor(MoveDirection direction = MoveDirection.Forward)
		{
			mockMotor.Direction = direction;
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
