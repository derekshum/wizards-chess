using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WizardsChess.Movement.Drv;
using WizardsChess.Movement.Drv.Events;

namespace WizardsChess.Movement
{
	public class CalibratedMotorMover
	{
		public CalibratedMotorMover(Axis axis, IMotorDrv mtrDrv, IStepCounter stepCntr, int gridMax, int gridMin)
		{
			gridPosition = 0;
			estimatedExtraSteps = 100;
			stepsPerGridUnit = 225;

			this.axis = axis;

			motorDrv = mtrDrv;
			stepCounter = stepCntr;

			this.gridMax = gridMax;
			this.gridMin = gridMin;

			stepCounter.FinishedCounting += finishedCounting;
			stepCounter.AdditionalStepsCounted += additionalStepsCounted;
		}

		public void PerformHoming()
		{
			// TODO: Write this
		}

		public void Move(int gridUnits)
		{
			if (gridPosition + gridUnits > gridMax)
			{
				gridUnits = gridMax - gridPosition;
			}
			else if (gridPosition + gridUnits < gridMin)
			{
				gridUnits = gridMin - gridPosition;
			}

			var state = gridUnits > 0 ? MotorState.Forward : MotorState.Backward;
			if (gridUnits == 0)
			{
				state = MotorState.Stopped;
			}
			else
			{
				var gridPositionInSteps = convertGridUnitsToSteps(gridPosition);
				var offset = stepPosition - gridPositionInSteps;
				var distanceToMove = convertGridUnitsToSteps(gridUnits) - offset;

				var steps = Math.Abs(distanceToMove) - estimatedExtraSteps;
				stepCounter.CountSteps(steps);
			}

			motorDrv.SetState(state);
		}

		private int convertGridUnitsToSteps(int gridUnits)
		{
			return gridUnits * stepsPerGridUnit;
		}

		private int convertStepsToGridUnits(int steps)
		{
			return (int)Math.Round((float)steps / stepsPerGridUnit);
		}

		private void finishedCounting(object sender, StepEventArgs stepEventArgs)
		{
			motorDrv.SetState(MotorState.Stopped);
		}

		private void additionalStepsCounted(object sender, StepEventArgs stepEventArgs)
		{
			System.Diagnostics.Debug.WriteLine($"Counted {stepEventArgs.NumSteps} additional steps in the {axis} axis");
			estimatedExtraSteps = stepEventArgs.NumSteps;
		}
		
		private int stepPosition;
		private int gridPosition;
		private int gridMax;
		private int gridMin;
		private int stepsPerGridUnit;
		private int estimatedExtraSteps;

		private Axis axis;

		private IMotorDrv motorDrv;
		private IStepCounter stepCounter;
	}
}
