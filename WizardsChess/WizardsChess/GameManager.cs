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

			var motorDriverX = new MotorDrv(23, 24);
			var motorDriverY = new MotorDrv(20, 21);
			var stepCounterX = new StepCounter(6, 19);
			var stepCounterY = new StepCounter(5, 13);
			var topInterrupterX = new PhotoInterrupter(9, 1);
			var bottomInterrupterX = new PhotoInterrupter(10, -1);
			var topInterrupterY = new PhotoInterrupter(11, 1);
			var bottomInterrupterY = new PhotoInterrupter(12, -1);

			var calXMover = new CalibratedMotorMover(Axis.X, 17, -17, motorDriverX, stepCounterX, topInterrupterX, bottomInterrupterX);
			var calYMover = new CalibratedMotorMover(Axis.Y, 23, -23, motorDriverY, stepCounterY, topInterrupterY, bottomInterrupterY);

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

		private ICommandInterpreter cmdInterpreter;
		private ChessLogic chessLogic;
		private IMoveManager moveManager;
		private GameState gameState;

		private void CommandReceived(Object sender, CommandEventArgs args)
		{
			System.Diagnostics.Debug.WriteLine($"Received command of type {args.Command.Type}.");
#if DEBUG
			if (args.Command.Type.GetFamily() == CommandFamily.Debug)
			{
				HandleDebugCommand(args.Command);
				return;
			}
#endif
		}

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
