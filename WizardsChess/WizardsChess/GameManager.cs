using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WizardsChess.VoiceControl;

namespace WizardsChess
{
	class GameManager
	{
		private GameManager()
		{
		}

		public static async Task<GameManager> CreateAsync()
		{
			GameManager manager = new GameManager();
			await Task.FromResult(0);
			return manager;
		}
	}
}
