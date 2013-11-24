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

namespace eecegroup32.mojiotowingalert.android
{
	[Activity (Label = "MainMenuActivity")]			
	public class MainMenuActivity : BaseActivity
	{
		Button notifcationButton;
		Button mapsButton;
		Button settingsButton;
		Button logOutButton;
		TextView welcome;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			SetContentView (Resource.Layout.MainMenu);

			welcome = FindViewById<TextView> (Resource.Id.welcomeText);
			welcome.Text = "Welcome " + Intent.GetStringExtra ("UsernameData");
			// Get button from the layout resource and attach an event to it
			notifcationButton = FindViewById<Button>(Resource.Id.notificationsButton);
			notifcationButton.Click += new EventHandler(OnNotificationsClicked);

			mapsButton = FindViewById<Button>(Resource.Id.mapsButton);
			mapsButton.Click += new EventHandler(OnMapsClicked);

			settingsButton = FindViewById<Button>(Resource.Id.settingsButton);
			settingsButton.Click += new EventHandler(OnSettingsClicked);

			logOutButton = FindViewById<Button>(Resource.Id.logOutButton);
			logOutButton.Click += new EventHandler(OnLogOutClicked);
		}

		private void OnNotificationsClicked(object sender, EventArgs e)
		{
			var notif = new Intent(this, typeof(NotificationsActivity));
			StartActivity(notif);
		}

		private void OnMapsClicked(object sender, EventArgs e)
		{
			var maps = new Intent(this, typeof(MapsActivity));
			StartActivity(maps);
		}

		private void OnSettingsClicked(object sender, EventArgs e)
		{
			var settings = new Intent(this, typeof(SettingsActivity));
			StartActivity(settings);
		}

		private void OnLogOutClicked(object sender, EventArgs e)
		{
			//Clear the user session and go to login
			Client.ClearUser();
			GotoLogin();
		}

		private void GotoLogin()
		{
			var login = new Intent(this, typeof(LoginActivity));
			login.AddFlags(ActivityFlags.ClearTop);
			StartActivity(login);
		}

	}
}

