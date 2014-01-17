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
using Mojio.Events;

namespace eecegroup32.mojiotowingalert.android
{
	[Activity (Label = "MainMenuActivity")]			
	public class MainMenuActivity : EventBaseActivity
	{
		Button notifcationButton;
		Button mapsButton;
		Button settingsButton;
		Button logOutButton;
		TextView welcome;

		protected static Context CurContext;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			SetContentView (Resource.Layout.MainMenu);

			welcome = FindViewById<TextView> (Resource.Id.welcomeText);

			string username = string.Empty;
			if (Client != null && Client.CurrentUser != null)
			{
				username = Client.CurrentUser.UserName;
			}


			welcome.Text = "Welcome " + username;
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

		protected override void OnStart()
		{
			base.OnStart();
			CurContext = this;
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

		protected override void OnDestroy()
		{
			base.OnDestroy();
		}

	}
}

