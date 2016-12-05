using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WizardsChess.Movement.Exceptions
{
	public class CalibrationException : Exception
	{
		public CalibrationException() : base() { }
		public CalibrationException(string message) : base(message) { }
	}
}
