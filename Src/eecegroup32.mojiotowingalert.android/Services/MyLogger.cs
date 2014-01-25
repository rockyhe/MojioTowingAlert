using System;
using System.Text;

namespace eecegroup32.mojiotowingalert.android
{
	public class MyLogger: ILogger
	{
		public enum Level {
			Debug,
			Information,
			Warning,
			Error
		}

		private Level VerbosityLevel = Level.Debug;

		public void Write(string tag, string message, Level level = Level.Information)
		{
			if (VerbosityLevel > level)
				return;

			var str = new StringBuilder ();
			str.Append ("(");
			str.Append (level.ToString ());
			str.Append (") ");
			str.Append ("[");
			str.Append (tag);
			str.Append ("] ");
			str.Append(message);
			Console.WriteLine (str.ToString());
		}

		public void SetVerbosityLevel (int level)
		{
			try 
			{
				VerbosityLevel = (Level) level;
			}
			catch (IndexOutOfRangeException) 
			{
				VerbosityLevel = Level.Error;
				Error ("Logger", "Verbosity level out of range. Set to the highest instead.");
			}
		}

		public void Error(string tag, string message)
		{
			Write (tag, message, Level.Error);
		}

		public void Warning(string tag, string message)
		{
			Write (tag, message, Level.Warning);
		}

		public void Information(string tag, string message)
		{
			Write (tag, message, Level.Information);
		}

		public void Debug(string tag, string message)
		{
			Write (tag, message, Level.Debug);
		}
	}
}

