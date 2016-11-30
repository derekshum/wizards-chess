using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WizardsChess.VoiceControl.Commands;
using System.Collections.Generic;

namespace WizardsChessTest.VoiceControl.Commands
{
	[TestClass]
	public class CommandTest
	{
		public CommandTest()
		{
			var cmdParams = new Dictionary<string, IReadOnlyList<string>>();
			cmdParams.Add("action", new List<string>{ "move" });
			commandParams = cmdParams;
			commandType = CommandType.Move;			
		}

		private IReadOnlyDictionary<string, IReadOnlyList<string>> commandParams;
		private CommandType commandType;

		[TestMethod]
		public void TestTypeConstruction()
		{
			var cmd = new Command(CommandType.Yes);
			Assert.AreEqual(CommandType.Yes, cmd.Type);
		}

		[TestMethod]
		public void TestStringConstruction()
		{
			var cmd2 = new Command(commandParams);
			Assert.AreEqual(commandType, cmd2.Type);
		}
	}
}
