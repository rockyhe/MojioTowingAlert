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

using Mojio.Client;

namespace eecegroup32.mojiotowingalert.android
{
	[Application]
	public class MainApp : Application
	{
		public static Config ConfigSettings;
		public static MojioClient Client;

		public MainApp (IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer) { }

		public override void OnCreate()
		{
			base.OnCreate();

			//Load config settings
			ConfigSettings = new Config();

			if (ConfigSettings.MojioAppId == null || ConfigSettings.MojioAppKey == null)
				throw new Exception("You must fill in the App ID and Key MojioTowingAlert.cs");

			// Setup Mojio Client
			Client = new MojioClient(this, new Guid(ConfigSettings.MojioAppId), new Guid(ConfigSettings.MojioAppKey), ConfigSettings.MojioApiEndpoint);
		}
	}
}

