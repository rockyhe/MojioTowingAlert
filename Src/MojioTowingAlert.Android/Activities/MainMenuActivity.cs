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

namespace MojioTowingAlert.Android
{
	[Activity (Label = "MainMenuActivity")]			
	public class MainMenuActivity : Activity
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

		public void OnNotificationsClicked(object sender, EventArgs e)
		{
			var login = new Intent(this, typeof(NotificationsActivity));
			StartActivity(login);
		}

		public void OnMapsClicked(object sender, EventArgs e)
		{
			var login = new Intent(this, typeof(MapsActivity));
			StartActivity(login);
		}

		public void OnSettingsClicked(object sender, EventArgs e)
		{
			var login = new Intent(this, typeof(SettingsActivity));
			StartActivity(login);
		}

		public void OnLogOutClicked(object sender, EventArgs e)
		{
			var login = new Intent(this, typeof(LoginActivity));
			StartActivity(login);
		}

	}
}

