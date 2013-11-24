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
	public partial class Config
	{
		public Config()
		{
			//App ID and SecretKey from http://sandbox.developer.moj.io/account
			MojioAppId = "1e9dac04-5acb-477a-9a0f-f3ce8600498b";
			MojioAppKey = "c22d37ca-4997-4d35-a159-d6ac993af8f0";

			// The API endpoint to use.  Default is Sandbox.  This MUST match where you got your app id/key.
			MojioApiEndpoint = MojioClient.Sandbox;
		}
	}
}

