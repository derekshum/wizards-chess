using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WizardsChess.Movement.Drv;
using WizardsChess.Movement.Drv.Events;
using WizardsChess.Movement.Exceptions;

namespace WizardsChess.Movement
{
	public class MovePerformer : IMovePerformer
	{
		public MovePerformer(IGridMotorMover xMtrMover, IGridMotorMover yMtrMover, IMagnetDrv magnetDrv)
		{
			xMover = xMtrMover;
			yMover = yMtrMover;
			magnet = magnetDrv;
		}

		public async Task MovePieceAsync(IList<Point2D> steps)
		{
			if (steps.Count <= 1)
			{
				System.Diagnostics.Debug.WriteLine($"MovePerformer received a piece move list with only {steps.Count} move(s).");
				return;
			}

			var start = steps[0];
			steps.RemoveAt(0);

			System.Diagnostics.Debug.WriteLine($"MovePerformer sending move {start}");
			await tryToMoveAsync(xMover, start.X);
			await tryToMoveAsync(yMover, start.Y);

			magnet.TurnOn();

			foreach(var point in steps)
			{
				System.Diagnostics.Debug.WriteLine($"MovePerformer sending move {point}");
				await tryToMoveAsync(xMover, point.X);
				await tryToMoveAsync(yMover, point.Y);
			}

			magnet.TurnOff();
		}

		public Task MoveMotorAsync(Axis axis, int gridUnits)
		{
			System.Diagnostics.Debug.WriteLine("MovePerformer can't perform direct grid moves.");
			return Task.FromResult(0);
		}

		public async Task GoHomeAsync()
		{
			await tryToMoveAsync(xMover, 0);
			await tryToMoveAsync(yMover, 0);
		}

		public async Task CalibrateAsync()
		{
			await xMover.CalibrateAsync();
			await yMover.CalibrateAsync();
			await GoHomeAsync();
		}

		public void EnableMagnet(bool enable)
		{
			if (enable)
			{
				magnet.TurnOn();
			}
			else
			{
				magnet.TurnOff();
			}
		}

		private IGridMotorMover xMover;
		private IGridMotorMover yMover;
		private IMagnetDrv magnet;

		private async Task tryToMoveAsync(IGridMotorMover mover, int desiredGridPosition)
		{
			var previousPosition = mover.GridPosition;
			try
			{
				await mover.GoToPositionAsync(desiredGridPosition);
			}
			catch (CalibrationException)
			{
				await calibrateAndRetry(mover, previousPosition, desiredGridPosition);
			}
		}

		private async Task calibrateAndRetry(IGridMotorMover mover, int previousPos, int desiredGridPos)
		{
			if (magnet.IsOn)
			{
				magnet.TurnOff();
			}
			await mover.CalibrateAsync();
			try
			{
				await mover.GoToPositionAsync(previousPos);
			}
			catch (CalibrationException)
			{
				System.Diagnostics.Debug.WriteLine("Threw another CalibrationException on move despite recalibrating.");
			}

			if (magnet.IsOn)
			{
				magnet.TurnOn();
			}
			try
			{
				await mover.GoToPositionAsync(desiredGridPos);
			}
			catch(CalibrationException)
			{
				System.Diagnostics.Debug.WriteLine("Threw another CalibrationException on move despite recalibrating.");
			}
		}
	}
}
