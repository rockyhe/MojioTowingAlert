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

namespace MojioTowingAlert.Android
{
	public interface ILogger
	{
		void Verbose (string message);

		void Debug (string message);

		void Information (string message);

		void Notice (string message);

		void Warning (string message);

		void Error (string message);

		void Critical (string message);

		void Alert (string message);

		void Emergency (string message);
	}
}

