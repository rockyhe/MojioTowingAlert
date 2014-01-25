using System;
using System.Text;

namespace eecegroup32.mojiotowingalert.android
{
	public class MyLogger: ILogger
	{
		public enum Level {
			Error,
			Warning,
			Information,
			Debug
		}

		public void Write(string tag, string message, Level level = Level.Information)
		{
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

