﻿using System;
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

			var motorDriverX = new MotorDrv(23, 24);
			var motorDriverY = new MotorDrv(20, 21);
			var stepCounterX = new StepCounter(6, 19);
			var stepCounterY = new StepCounter(5, 13);

			var calXMover = new CalibratedMotorMover(Axis.X, motorDriverX, stepCounterX, 14, -14);
			var calYMover = new CalibratedMotorMover(Axis.Y, motorDriverY, stepCounterY, 14, -14);

			var magnetDriver = new MagnetDrv(26);

			var movePerformer = new MovePerformer(calXMover, calYMover, magnetDriver);
			var movePlanner = new MovePlanner(logic.Board);

			var moveManager = new MoveManager(movePlanner, movePerformer);

			GameManager manager = new GameManager(await commandInterpreterConstructor, logic, moveManager);
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

		private void CommandReceived(Object sender, CommandEventArgs args)
		{
			System.Diagnostics.Debug.WriteLine($"Received command of type {args.Command.Type}.");
			//TODO: fill in comments below
			switch (args.Command.Type)
			{
				case CommandType.Move:
					var moveCmd = args.Command as MoveCommand;
					currentMoveCommand = moveCmd;
					//R: await performMoveOnUiIfValidAsync(moveCmd);

					//numPossible = number of moves fitting that description possible
					//switch numPossible
					//	case 0
					//		give user feedback
					//	case 1
					//		preform move
					//	case 2 //2+, but I don't think any more than 2 possibilities can be relevant
					//		give user feedback asking which one
				break;
				case CommandType.ConfirmPiece:
					var pieceConfirmation = args.Command as ConfirmPieceCommand;
					if (currentMoveCommand == null)
					{
						//R: throw new Exception("Received piece confirmation command when currentMoveCommand was null");
						//Output that a piece isn't what was being looked for, or do nothing
					}
					currentMoveCommand.Position = pieceConfirmation.Position;
					//R:await performMoveOnUiIfValidAsync(currentMoveCommand);
				break;
				//case yes
				//case no
				//case undo?
				//case cancel?
				//case MotorMove?
				//case Magnet?
				//case castle?
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
					DebugMovePerformer.MoveMotor(mtrMvCmd.Axis, mtrMvCmd.Steps);
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
