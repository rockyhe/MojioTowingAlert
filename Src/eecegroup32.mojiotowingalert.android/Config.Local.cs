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
			MojioAppId = "59f309b1-a318-4711-bf5c-aa7f861704d6";
			MojioAppKey = "2158aa6b-efd4-46b3-a6cf-b6554303a2a7";

			// The API endpoint to use.  Default is Sandbox.  This MUST match where you got your app id/key.
			MojioApiEndpoint = MojioClient.Sandbox;
		}
	}
}

