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
		private Button notifcationButton;
		private Button mapsButton;
		private Button settingsButton;
		private Button logOutButton;
		private TextView welcome;

		protected override void OnCreate (Bundle bundle)
		{
			logger.Debug (this.LocalClassName, "Lifecycle Entered: OnCreate");

			base.OnCreate (bundle);
			SetContentView (Resource.Layout.MainMenu);
			InitializeComponents ();
			InitializeEventHandlers ();
			InitializeWelcomeScreen ();

			logger.Debug (this.LocalClassName, "Lifecycle Exited: OnCreate");
		}

		protected override void OnStart()
		{
			logger.Debug (this.LocalClassName, "Lifecycle Entered: OnStart");
			base.OnStart();
			logger.Debug (this.LocalClassName, "Lifecycle Exited: OnStart");
		}

		protected override void OnStop()
		{
			logger.Debug (this.LocalClassName, "Lifecycle Entered: OnStop");
			base.OnStop();		
			logger.Debug (this.LocalClassName, "Lifecycle Exited: OnStop");
		}

		protected override void OnDestroy()
		{
			logger.Debug (this.LocalClassName, "Lifecycle Entered: OnDestroy");
			base.OnDestroy();		
			logger.Debug (this.LocalClassName, "Lifecycle Exited: OnDestroy");
		}

		protected override void OnResume()
		{
			logger.Debug (this.LocalClassName, "Lifecycle Entered: OnResume");
			base.OnResume();
			MainApp.SetCurrentActivity (this);
			logger.Debug (this.LocalClassName, "Lifecycle Exited: OnResume");
		}

		protected override void OnPause()
		{
			logger.Debug (this.LocalClassName, "Lifecycle Entered: OnPause");
			base.OnPause();
			logger.Debug (this.LocalClassName, "Lifecycle Exited: OnPause");
		}

		private void InitializeComponents ()
		{
			welcome = FindViewById<TextView> (Resource.Id.welcomeText);
			notifcationButton = FindViewById<Button>(Resource.Id.notificationsButton);
			mapsButton = FindViewById<Button>(Resource.Id.mapsButton);
			settingsButton = FindViewById<Button>(Resource.Id.settingsButton);
			logOutButton = FindViewById<Button>(Resource.Id.logOutButton);
		}

		private void InitializeEventHandlers()
		{
			notifcationButton.Click += new EventHandler(OnNotificationsClicked);
			mapsButton.Click += new EventHandler(OnMapsClicked);
			settingsButton.Click += new EventHandler(OnSettingsClicked);
			logOutButton.Click += new EventHandler(OnLogOutClicked);
		}

		private void InitializeWelcomeScreen ()
		{
			string username = string.Empty;
			if (Client != null && Client.CurrentUser != null) {
				username = Client.CurrentUser.UserName;
			}
			welcome.Text = "Welcome " + username;
		}

		private void OnNotificationsClicked(object sender, EventArgs e)
		{
			StartActivity(new Intent(this, typeof(NotificationsActivity)));
		}

		private void OnMapsClicked(object sender, EventArgs e)
		{
			StartActivity(new Intent(this, typeof(MapsActivity)));
		}

		private void OnSettingsClicked(object sender, EventArgs e)
		{
			StartActivity(new Intent(this, typeof(SettingsActivity)));
		}

		private void OnLogOutClicked(object sender, EventArgs e)
		{
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

