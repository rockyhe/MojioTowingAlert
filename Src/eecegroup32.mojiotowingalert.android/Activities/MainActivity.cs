using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;

//TODO why don't we just remove this splash screen? 
namespace eecegroup32.mojiotowingalert.android
{
	[Activity (Label = "@string/applicationName", MainLauncher = true)]
	public class MainActivity : BaseActivity
	{
		void ShowSplashScreen (long duration)
		{
			SetContentView (Resource.Layout.SplashScreen);
			SystemClock.Sleep (duration);
		}	
		protected override void OnCreate(Bundle bundle)
		{
			base.OnCreate(bundle);
			ShowSplashScreen (3000);

			if (Client.IsLoggedIn())
			{
				GotoMainMenu();
			}
			else
			{
				GotoLogin();
			}
		}

		private void GotoLogin()
		{
			var login = new Intent(this, typeof(LoginActivity));
			login.AddFlags(ActivityFlags.ClearTask);
			StartActivity(login);
		}

		private void GotoMainMenu()
		{
			var menu = new Intent(this, typeof(MainMenuActivity));
			StartActivity(menu);
		}
	}
}


