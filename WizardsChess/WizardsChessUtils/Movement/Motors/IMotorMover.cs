using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WizardsChess.Movement
{
	public interface IMotorMover
	{
		/// <summary>
		/// How many steps the motor tends to keep moving for after being told to stop.
		/// </summary>
		int EstimatedOvershoot { get; }

		/// <summary>
		/// The IMotorLocator for this motor.
		/// </summary>
		IMotorLocator Locator { get; }

		/// <summary>
		/// Drive the motor up to the given position, then stop. 
		/// Does not return until the motor is completely stopped.
		/// </summary>
		/// <param name="position">The target position to drive the motor to. Not necessarily where the motor stops.</param>
		/// <returns>The position when the motor stops.</returns>
		Task<int> GoToPositionAsync(int position);

		/// <summary>
		/// Cancel an in-progress move.
		/// </summary>
		void CancelMove();
	}
}
