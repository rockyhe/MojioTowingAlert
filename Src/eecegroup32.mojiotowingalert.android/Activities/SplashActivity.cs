using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using System.Threading;

namespace eecegroup32.mojiotowingalert.android
{
	[Activity (NoHistory = true)]
	public class SplashActivity: Activity
	{
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			ThreadPool.QueueUserWorkItem (o => {
				Wait (5000);
				Finish ();
			});

            
//			if (Client.IsLoggedIn())
//			{
//				GotoMainMenu();
//			}
//			else
//			{
//				GotoLogin();
//			}
		}

		private void GotoLogin ()
		{
			var login = new Intent (this, typeof(LoginActivity));
			login.AddFlags (ActivityFlags.ClearTask);
			StartActivity (login);
		}

		private void GotoMainMenu ()
		{
			var menu = new Intent (this, typeof(MainMenuActivity));
			StartActivity (menu);
		}
	}
}


