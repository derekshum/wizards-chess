using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WizardsChess.Movement
{
	public enum CalibrationState
	{
		Ready,
		NeedsCalibrating,
		PreparingToCalibrate,
		Calibrating
	}

	public interface IMotorCalibrator
	{
		float StepsPerGridUnit { get; }
		CalibrationState State { get; }

		Task CalibrateAsync();
	}
}
