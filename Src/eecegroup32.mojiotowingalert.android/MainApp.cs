using System;
using System.Collections.Generic;
using Android.App;
using Android.Runtime;
using Mojio;
using Mojio.Client;
using eecegroup32.mojiotowingalert.core;

namespace eecegroup32.mojiotowingalert.android
{
	[Application]
	public class MainApp : Application
	{
		public static Config ConfigSettings { get; set; }

		public static MojioClient Client { get; set; }

		private static Activity _CurrentActivity;

		public MainApp (IntPtr javaReference, JniHandleOwnership transfer) : base (javaReference, transfer)
		{
		}

		private void SetupMojioClient ()
		{
			Client = new MojioClient (this, new Guid (ConfigSettings.MojioAppId), new Guid (ConfigSettings.MojioAppKey), ConfigSettings.MojioApiEndpoint);

			if (Client.Token == null) {
				Client.Begin (new Guid (ConfigSettings.MojioAppId), new Guid (ConfigSettings.MojioAppKey));
			}
		}

		static void SetupConfigSettings ()
		{
			ConfigSettings = new Config ();

			if (ConfigSettings.MojioAppId == null || ConfigSettings.MojioAppKey == null)
				throw new Exception ("You must fill in the App ID and Key MojioTowingAlert.cs");
		}

		public override void OnCreate ()
		{
			base.OnCreate ();
			SetupConfigSettings ();
			SetupMojioClient ();
		}

		public static Activity GetCurrentActivity ()
		{
			return _CurrentActivity;
		}

		public static void SetCurrentActivity (Activity activity)
		{
			_CurrentActivity = activity;
		}
	}
}

