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
		[TestCategory(nameof(PositionSignaler))]
		[TestMethod]
		public void TestPositionSignalerConstruction()
		{
			mockMotor = MockMotor.Create();
			motorLocator = new MotorLocator(mockMotor, new MockGpio(), mockMotor);
			positionSignaler = new PositionSignaler(motorLocator);

			positionSignaler.ReachedPosition += finishedCounting;
		}

		[TestCategory(nameof(PositionSignaler))]
		[TestMethod]
		public void TestPositionSignalerBasic()
		{
			TestPositionSignalerConstruction();

			resetTestVariables();

			isCounting = true;
			var expectedPosition = 10;
			positionSignaler.SignalOnPosition(expectedPosition);
			runMotor();
			while (isCounting)
			{
				Task.Delay(60).Wait();
			}
			Assert.AreEqual(expectedPosition, positionOnTargetReached, "Step count was off.");
			checkForErrors();
		}

		[TestCategory(nameof(PositionSignaler))]
		[TestMethod]
		public void TestPositionSignalerOverlappingCounts()
		{
			TestPositionSignalerConstruction();
			resetTestVariables();

			isCounting = true;
			positionSignaler.SignalOnPosition(50);
			runMotor();
			Task.Delay(30).Wait();
			var shorterTarget = motorLocator.Position + 5;
			positionSignaler.SignalOnPosition(shorterTarget);
			isCounting = true;
			while (isCounting)
			{
				Task.Delay(30).Wait();
			}
			var firstTargetPositionReached = positionOnTargetReached;
			Assert.AreEqual(shorterTarget, firstTargetPositionReached, $"Overlapping count step did not reset target count properly. Counted to position {firstTargetPositionReached}.");
			Task.Delay(convertPositionToTimeout(50)).Wait();
			Assert.AreEqual(firstTargetPositionReached, positionOnTargetReached, "Counted more steps after delay!");
			checkForErrors();
		}

		[TestCategory(nameof(PositionSignaler))]
		[TestMethod]
		public void TestPositionSignalerStopMidCount()
		{
			TestPositionSignalerConstruction();
			resetTestVariables();

			isCounting = true;
			positionSignaler.SignalOnPosition(50);
			runMotor();
			Task.Delay(90).Wait();
			var pos = motorLocator.Position;
			positionSignaler.SignalOnPosition(pos);
			while(mockMotor.Information.Direction != MoveDirection.Stopped)
			{
				Task.Delay(30).Wait();
			}
			Assert.AreEqual(pos, positionOnTargetReached, $"Overlapping count step did not reset target count properly. Expected {pos} and reached target {positionOnTargetReached}.");
			checkForErrors();
		}

		[TestCategory(nameof(PositionSignaler))]
		[TestMethod]
		public void TestPositionSignalerStepAfterStopping()
		{
			TestPositionSignalerConstruction();
			resetTestVariables();

			isCounting = true;
			positionSignaler.SignalOnPosition(10);
			runMotor();
			var start = DateTime.Now;
			while (mockMotor.Information.Direction != MoveDirection.Stopped)
			{
				Task.Delay(30).Wait();
			}
			var end = DateTime.Now;

			runMotor(MoveDirection.Backward);
			Task.Delay((end - start).Milliseconds + 60).Wait();
			stopMotor();

			checkForErrors();
		}

		private MockMotor mockMotor;
		private IMotorLocator motorLocator;
		private PositionSignaler positionSignaler;

		private int positionOnTargetReached;

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
			isCounting = false;
		}

		private TimeSpan convertPositionToTimeout(int position)
		{
			return TimeSpan.FromMilliseconds((position - motorLocator.Position) * 30);
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
			isPassing = true;
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
