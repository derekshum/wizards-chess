using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WizardsChess.AppDebugging
{
	public static class Threading
	{
		public static Windows.UI.Core.CoreDispatcher uiDispatcher;

		public static void SetUiDispatcher(Windows.UI.Core.CoreDispatcher dispatcher)
		{
			uiDispatcher = dispatcher;
		}

		public static async Task MarshallToUiThread(Windows.UI.Core.DispatchedHandler fn)
		{
			if (uiDispatcher == null)
			{
				uiDispatcher = Windows.UI.Core.CoreWindow.GetForCurrentThread()?.Dispatcher;
				if (uiDispatcher == null)
				{
					System.Diagnostics.Debug.WriteLine("Could not get ui dispatcher.");
					return;
				}
			}

			await uiDispatcher
				.RunAsync(
					Windows.UI.Core.CoreDispatcherPriority.Normal,
					fn);
		}
	}
}
