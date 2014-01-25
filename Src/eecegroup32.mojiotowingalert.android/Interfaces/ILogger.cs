using System;
using System.Text;

namespace eecegroup32.mojiotowingalert.android
{
	public interface ILogger
	{
		void Error(string tag, string message);

		void Warning(string tag, string message);

		void Information(string tag, string message);

		void Debug(string tag, string message);
	}
}

