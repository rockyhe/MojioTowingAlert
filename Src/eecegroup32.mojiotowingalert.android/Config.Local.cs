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
			MojioAppId = "0c47d9b4-0712-49ce-961e-ca1a4cdc3a4e";
			MojioAppKey = "291a4b82-427a-42a4-aabe-b7b26066cf4a";

			// The API endpoint to use.  Default is Sandbox.  This MUST match where you got your app id/key.
			MojioApiEndpoint = MojioClient.Sandbox;
		}
	}
}

