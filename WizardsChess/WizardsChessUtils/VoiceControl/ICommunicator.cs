using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WizardsChess.VoiceControl
{
	public interface ICommunicator
	{
		Task SpeakAsync(string text);
	}
}
