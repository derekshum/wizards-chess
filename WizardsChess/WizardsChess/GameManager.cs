using System;
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

namespace WizardsChess
{
	enum GameState
	{
		Ready,
		Playing,
		AwaitingReset,
		Complete
	}

	class GameManager
	{
		private GameManager(ICommandInterpreter commandInterpreter, ChessLogic logic, IMoveManager movementManager)
		{
			commandInterpreter.CommandReceived += CommandReceived;

			cmdInterpreter = commandInterpreter;
			chessLogic = logic;
			moveManager = movementManager;
			gameState = GameState.Ready;
		}

		public static async Task<GameManager> CreateAsync()
		{
			var commandInterpreterConstructor = CommandInterpreter.CreateAsync();

			ChessLogic logic = new ChessLogic();

			var stepCountPinX = new GpioPinWrapper(5, Windows.Devices.Gpio.GpioPinDriveMode.InputPullUp);
			var stepClearPinX = new GpioPinWrapper(13, Windows.Devices.Gpio.GpioPinDriveMode.Output, Windows.Devices.Gpio.GpioPinValue.Low);
			var motorInformationX = new MotorInformation(Axis.X, stepCountPinX);
			var motorDriverX = new MotorDrv(20, 21, motorInformationX);
			var motorLocatorX = new MotorLocator(stepClearPinX, motorDriverX.Information);
			var positionSignalerX = new PositionSignaler(motorLocatorX);
			var motorMoverX = new MotorMover(50, positionSignalerX, motorLocatorX, motorDriverX);

			var stepCountPinY = new GpioPinWrapper(6, Windows.Devices.Gpio.GpioPinDriveMode.InputPullUp);
			var stepClearPinY = new GpioPinWrapper(19, Windows.Devices.Gpio.GpioPinDriveMode.Output, Windows.Devices.Gpio.GpioPinValue.Low);
			var motorInformationY = new MotorInformation(Axis.Y, stepCountPinY);
			var motorDriverY = new MotorDrv(24, 23, motorInformationY);
			var motorLocatorY = new MotorLocator(stepClearPinY, motorDriverY.Information);
			var positionSignalerY = new PositionSignaler(motorLocatorY);
			var motorMoverY = new MotorMover(50, positionSignalerY, motorLocatorY, motorDriverY);

			var topInterrupterX = new PhotoInterrupter(17, 1, 150);
			var bottomInterrupterX = new PhotoInterrupter(27, -1, -150);
			var motorCalibratorX = new MotorCalibrator(-23, 23, motorMoverX, motorInformationX, topInterrupterX, bottomInterrupterX);

			var topInterrupterY = new PhotoInterrupter(25, 1, 150);
			var bottomInterrupterY = new PhotoInterrupter(22, -1, -150);
			var motorCalibratorY = new MotorCalibrator(-17, 17, motorMoverY, motorInformationY, topInterrupterY, bottomInterrupterY);

			var preciseMoverX = new PreciseMotorMover(motorMoverX);
			var gridMoverX = new GridMotorMover(preciseMoverX, motorCalibratorX);

			var preciseMoverY = new PreciseMotorMover(motorMoverY);
			var gridMoverY = new GridMotorMover(preciseMoverY, motorCalibratorY);

			var magnetDriver = new MagnetDrv(26);

			var movePerformer = new MovePerformer(gridMoverX, gridMoverY, magnetDriver);
			var motorCalibrationTask = movePerformer.CalibrateAsync();
			var movePlanner = new MovePlanner(logic.Board);
			var moveManager = new MoveManager(movePlanner, movePerformer);

			GameManager manager = new GameManager(await commandInterpreterConstructor, logic, moveManager);

			await motorCalibrationTask;
#if DEBUG
				manager.DebugMovePerformer = movePerformer;
#endif
			return manager;
		}

		public async Task<GameState> PlayGameAsync()
		{
			gameState = GameState.Playing;
			await cmdInterpreter.StartAsync();

			while (gameState == GameState.Playing)
			{
				await Task.Delay(500);
			}

			return gameState;
		}

		public Task ResetAsync()
		{
			throw new NotImplementedException("Not yet implemented");
		}

		public Task CongratulateWinnerAsync()
		{
			throw new NotImplementedException("Congratulate winner not yet implemented");
		}

		private async void CommandReceived(Object sender, CommandEventArgs args)
		{
			System.Diagnostics.Debug.WriteLine($"Received command of type {args.Command.Type}.");
			switch (args.Command.Type)
			{
				case CommandType.Move:
					System.Diagnostics.Debug.WriteLine($"Attempting Move.");
					var moveCmd = args.Command as MoveCommand;
					currentMoveCommand = moveCmd;
					System.Diagnostics.Debug.WriteLine($"About to call preformMoveIfValidAsync.");
					await performMoveIfValidAsync(moveCmd);
					break;
				case CommandType.Castle:
					await performCastleIfValid(args);
					break;
				case CommandType.ConfirmPiece:
					var pieceConfirmation = args.Command as ConfirmPieceCommand;
					if (currentMoveCommand == null)
					{
						System.Diagnostics.Debug.WriteLine($"Received piece confirmation command when currentMoveCommand was null");
					}
					currentMoveCommand.Position = pieceConfirmation.Position;
					await performMoveIfValidAsync(currentMoveCommand);
					break;
				case CommandType.Undo:
					//no output when undo not possible, basically ignore it
					await performUndoIfPossible();
					break;
                // TODO: case CommandType.Reset:
                default:
                    System.Diagnostics.Debug.WriteLine($"GameManager ignored command of type {args.Command.Type}.");
					break;
			}
			

#if DEBUG
			if (args.Command.Type.GetFamily() == CommandFamily.Debug)
			{
				HandleDebugCommand(args.Command);
				return;
			}
#endif
		}

		private async Task performMoveIfValidAsync(MoveCommand moveCmd)
		{
			System.Diagnostics.Debug.WriteLine($"preformMoveIfValidAsync called");
			if (!moveCmd.Position.HasValue)	//checks command format
			{
				System.Diagnostics.Debug.WriteLine($"moveCmd.Position.HasValue == false");
				var possibleStartPositions = chessLogic.FindPotentialPiecesForMove(moveCmd.Piece.Value, moveCmd.Destination);
				if (possibleStartPositions.Count == 0)
				{
					System.Diagnostics.Debug.WriteLine($"Could not find a possible starting piece of type {moveCmd.Piece.Value} going to {moveCmd.Destination}");
					//TODO: output saying no valid moves fit that description
					return;
				}
				else if (possibleStartPositions.Count == 1)
				{
					System.Diagnostics.Debug.WriteLine($"1 possible start position");
					moveCmd.Position = possibleStartPositions.First();
					System.Diagnostics.Debug.WriteLine(chessLogic.Board.ToString());
				}
				else
				{
					System.Diagnostics.Debug.WriteLine($"multiple possible start positions");
					await cmdInterpreter.ConfirmPieceSelectionAsync(moveCmd.Piece.Value, possibleStartPositions.ToList());
					return;
				}
			}
			else
			{
				System.Diagnostics.Debug.WriteLine($"moveCmd.Position.HasValue == true");
				if (!chessLogic.IsMoveValid((Position)moveCmd.Position, moveCmd.Destination))
				{
					System.Diagnostics.Debug.WriteLine($"Specified move not valid.");
					//TODO: output saying no valid moves fit that description
					return;
				}
			}
			if (chessLogic.DoesMoveCapture((Position)moveCmd.Position, moveCmd.Destination))	//piece captured
			{
				await moveManager.MoveAsync(new Point2D((Position)moveCmd.Position), new Point2D(moveCmd.Destination), new Point2D(chessLogic.CaptureLocation((Position)moveCmd.Position, moveCmd.Destination)));
			}
			else
			{
				await moveManager.MoveAsync(new Point2D((Position)moveCmd.Position), new Point2D(moveCmd.Destination));
			}
			chessLogic.MovePiece((Position)moveCmd.Position, moveCmd.Destination);
		}

		private async Task performCastleIfValid(CommandEventArgs args)
		{
			var castleCmd = args.Command as CastleCommand;
			if (castleCmd.Direction == CastleDirection.Short)
			{
				await performShortCastleIfValidAsync();
			}
			else //long castle
			{
				await performLongCastleIfValidAsync();
			}
			/*var validRookLocations = chessLogic.validRookLocationsForCastling();
			if (validRookLocations.Count == 0)
			{
				System.Diagnostics.Debug.WriteLine($"No valid castles");
				//TODO: output saying no valid moves fit that description
				return;
			}
			else if (validRookLocations.Count == 1)
			{
				await moveManager.CastleAsync(validRookLocations[0], chessLogic.Board.GetKingCol());
				chessLogic.Castle(validRookLocations[0]);
			}
			else	// validRookLocations.Count == 2
			{
				//await cmdInterpreter.ConfirmPieceSelectionAsync(moveCmd.Piece.Value, validRookLocations);
				//TODO: figure out how to make this similar
				return;

			}*/
		}

		private async Task performShortCastleIfValidAsync()
		{
			if (chessLogic.shortCastleLegal())
			{
				Point2D rookPos = chessLogic.shortCastleRookPos();
				await moveManager.CastleAsync(rookPos, chessLogic.Board.GetKingCol());
				chessLogic.Castle(rookPos);
				System.Diagnostics.Debug.WriteLine(chessLogic.Board.ToString());
			}
			else
			{
				System.Diagnostics.Debug.WriteLine("short castle not legal");
			}
		}

		private async Task performLongCastleIfValidAsync()
		{
			if (chessLogic.longCastleLegal())
			{
				Point2D rookPos = chessLogic.longCastleRookPos();
				await moveManager.CastleAsync(rookPos, chessLogic.Board.GetKingCol());
				chessLogic.Castle(rookPos);
				System.Diagnostics.Debug.WriteLine(chessLogic.Board.ToString());
			}
			else
			{
				System.Diagnostics.Debug.WriteLine("long castle not legal");
			}
		}

		private async Task performUndoIfPossible()
		{
			await moveManager.UndoMoveAsync();
			chessLogic.UndoMove();
			System.Diagnostics.Debug.WriteLine(chessLogic.Board.ToString());
		}

		private ICommandInterpreter cmdInterpreter;
		private ChessLogic chessLogic;
		private IMoveManager moveManager;
		private GameState gameState;
		private MoveCommand currentMoveCommand;

#if DEBUG
		public IMovePerformer DebugMovePerformer;

		private void HandleDebugCommand(ICommand command)
		{
			switch (command.Type)
			{
				case CommandType.MotorMove:
					var mtrMvCmd = command as MotorMoveCommand;
					DebugMovePerformer.MoveMotorAsync(mtrMvCmd.Axis, mtrMvCmd.Steps);
					break;
				case CommandType.Magnet:
					var magnetCmd = command as MagnetCommand;
					DebugMovePerformer.EnableMagnet(magnetCmd.EnableMagnet);
					break;
				default:
					break;
			}
		}
#endif
	}
}
