using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace eecegroup32.mojiotowingalert.android
{
	public class MojioLog : ILogger
	{
		public enum Level {
			Emergency,
			Alert,
			Critical,
			Error,
			Warning,
			Notice,
			Information,
			Debug
		}

		public void Write(string message, Level level = Level.Information)
		{
			var str = "[" + level.ToString () + "] " + DateTime.UtcNow.ToString () + " -- " + message;

			#if (!DEBUG)
			// Perhaps deside on level of loggin depending on environment?
			if (!String.IsNullOrEmpty(Config.TestFlightApi) && TestFlight.SendsLogs () && level <= Level.Notice)
				TestFlight.Log (str);
			#endif

			#if (DEBUG)
			Console.WriteLine (str);
			#endif
		}

		public void Verbose(string message)
		{
			Write (message, Level.Debug);
		}

		public void Debug(string message)
		{
			Write (message, Level.Debug);
		}

		public void Information(string message)
		{
			Write (message, Level.Information);
		}

		public void Notice(string message)
		{
			Write (message, Level.Notice);
		}

		public void Warning(string message)
		{
			Write (message, Level.Warning);
		}

		public void Error(string message)
		{
			Write (message, Level.Error);
		}

		public void Critical(string message)
		{
			Write (message, Level.Critical);
		}

		public void Alert(string message)
		{
			Write (message, Level.Alert);
		}

		public void Emergency(string message)
		{
			Write (message, Level.Emergency);
		}
	}
}

