using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Mojio;
using Mojio.Client;

namespace MojioTowingAlert.Android
{
	[Activity (Label = "@string/applicationName", MainLauncher = true)]
	public class MainActivity : Activity
	{
	

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			// Set our view from the "main" layout resource
			SetContentView (Resource.Layout.SplashScreen);

			SystemClock.Sleep (3000);
			GotoLogin ();

		}

		private void GotoLogin()
		{
			var login = new Intent(this, typeof(LoginActivity));
			StartActivity(login);
		}

	}
}


