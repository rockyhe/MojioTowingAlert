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
		//App ID and SecretKey from http://sandbox.developer.moj.io/account
		public string MojioAppId = null;
		public string MojioAppKey = null;

		//The API endpoint to use.  Default is Sandbox.  This MUST match where you got your app id/key.
		public string MojioApiEndpoint = MojioClient.Sandbox;
	}
}

