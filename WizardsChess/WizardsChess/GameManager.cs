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

			var motorDriverX = new MotorDrv(20, 21);
			var motorDriverY = new MotorDrv(24, 23);
			var stepCountPinX = new GpioPinWrapper(5, Windows.Devices.Gpio.GpioPinDriveMode.InputPullUp);
			var motorLocatorX = new MotorLocator(stepCountPinX, motorDriverX);
			var stepCountPinY = new GpioPinWrapper(6, Windows.Devices.Gpio.GpioPinDriveMode.InputPullUp);
			var motorLocatorY = new MotorLocator(stepCountPinY, motorDriverY);
			var stepClearPinX = new GpioPinWrapper(13, Windows.Devices.Gpio.GpioPinDriveMode.Output, Windows.Devices.Gpio.GpioPinValue.Low);
			var stepClearPinY = new GpioPinWrapper(19, Windows.Devices.Gpio.GpioPinDriveMode.Output, Windows.Devices.Gpio.GpioPinValue.Low);
			var stepCounterX = new StepCounter(motorLocatorX, stepClearPinX);
			var stepCounterY = new StepCounter(motorLocatorY, stepClearPinY);
			var topInterrupterX = new PhotoInterrupter(17, 1);
			var bottomInterrupterX = new PhotoInterrupter(27, -1);
			var topInterrupterY = new PhotoInterrupter(25, 1);
			var bottomInterrupterY = new PhotoInterrupter(22, -1);

			var calXMover = new CalibratedMotorMoverOld(Axis.X, 23, -23, 3, motorDriverX, stepCounterX, topInterrupterX, bottomInterrupterX);
			var calYMover = new CalibratedMotorMoverOld(Axis.Y, 17, -17, 3, motorDriverY, stepCounterY, topInterrupterY, bottomInterrupterY);

			var magnetDriver = new MagnetDrv(26);

			var movePerformer = new MovePerformer(calXMover, calYMover, magnetDriver);
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
			//TODO: fill in comments below
			switch (args.Command.Type)
			{
				case CommandType.Move:
					var moveCmd = args.Command as MoveCommand;
					currentMoveCommand = moveCmd;
					await performMoveIfValidAsync(moveCmd);
				break;
				/*case CommandType.Castle	//need to add this
				*	await performCastleIfValid();
				*/
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
				//case yes
				//case no
				//case cancel?
				//case MotorMove?
				//case Magnet?
				default:
					//debug writing done above
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
			if (!moveCmd.Position.HasValue)
			{
				var possibleStartPositions = chessLogic.FindPotentialPiecesForMove(moveCmd.Piece.Value, moveCmd.Destination);
				if (possibleStartPositions.Count == 0)
				{
					System.Diagnostics.Debug.WriteLine($"Could not find a possible starting piece of type {moveCmd.Piece.Value} going to {moveCmd.Destination}");
					//TODO: output saying no valid moves fit that description
					return;
				}
				else if (possibleStartPositions.Count == 1)
				{
					moveCmd.Position = possibleStartPositions.First();
				}
				else
				{
					await cmdInterpreter.ConfirmPieceSelectionAsync(moveCmd.Piece.Value, possibleStartPositions.ToList());
					return;
				}
			}
			else
			{
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

		private async Task performCastleIfValid()
		{
			var validRookLocations = chessLogic.validRookLocationsForCastling();
			if (validRookLocations.Count == 0)
			{
				System.Diagnostics.Debug.WriteLine($"No valid castles");
				//TODO: output saying no valid moves fit that description
				return;
			}
			else if (validRookLocations.Count == 1)
			{
				chessLogic.Castle(validRookLocations[0]);
			}
			else	// validRookLocations.Count == 2
			{
				//await cmdInterpreter.ConfirmPieceSelectionAsync(moveCmd.Piece.Value, numValidCastles.ToList());
				//TODO: figure out how to make this similar
				return;

			}
		}

		private async Task performUndoIfPossible()
		{
			await moveManager.UndoMoveAsync();
			chessLogic.UndoMove();
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
