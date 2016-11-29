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
			cmdInterpreter.CommandReceived += CommandReceived;

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
			var magnetDriver = new MagnetDrv(26);
			var stepCounterX = new StepCounter(6, 19);
			var stepCounterY = new StepCounter(5, 13);
			var movePerformer = new MovePerformer(motorDriverX, motorDriverY, magnetDriver, stepCounterX, stepCounterY);

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
			if (command is MotorMoveCommand)
			{
				var mtrMvCmd = command as MotorMoveCommand;
				DebugMovePerformer.MoveMotor(mtrMvCmd.Axis, mtrMvCmd.Steps);
			}
		}
#endif
	}
}
