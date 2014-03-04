using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Mojio.Events;
using eecegroup32.mojiotowingalert.core;
using System.Threading.Tasks;

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
			MyLogger.Debug (this.LocalClassName, "Lifecycle Entered: OnCreate");
			base.OnCreate (bundle);
			SetContentView (Resource.Layout.MainMenu);
			InitializeVariables ();
			InitializeEventHandlers ();
			InitializeWelcomeScreen ();
			Task.Factory.StartNew (() => LoadLastEvents (EventsToSubscribe)).Wait (2000);
			
			MyLogger.Debug (this.LocalClassName, "Lifecycle Exited: OnCreate");
		}

		protected override void OnResume ()
		{
			MyLogger.Debug (this.LocalClassName, "Lifecycle Entered: OnResume");
			base.OnResume ();
			MainApp.SetCurrentActivity (this);
			
			UpdateNumberOfNewEvents ();
			MyLogger.Debug (this.LocalClassName, "Lifecycle Exited: OnResume");
		}

		private void InitializeVariables ()
		{
			this.ActionBar.SetBackgroundDrawable (Resources.GetDrawable (Resource.Drawable.Black));
			this.ActionBar.SetTitle (Resource.String.mainMenu);
			welcome = FindViewById<TextView> (Resource.Id.welcomeText);
			notifcationButton = FindViewById<Button> (Resource.Id.notificationsButton);
			mapsButton = FindViewById<Button> (Resource.Id.mapsButton);
			settingsButton = FindViewById<Button> (Resource.Id.settingsButton);
			logOutButton = FindViewById<Button> (Resource.Id.logOutButton);
		}

		private void InitializeEventHandlers ()
		{
			notifcationButton.Click += new EventHandler (OnNotificationsClicked);
			mapsButton.Click += new EventHandler (OnMapsClicked);
			settingsButton.Click += new EventHandler (OnSettingsClicked);
			logOutButton.Click += new EventHandler (OnLogOutClicked);
		}

		private void InitializeWelcomeScreen ()
		{
			string username = string.Empty;
			if (Client != null && Client.CurrentUser != null) {
				username = Client.CurrentUser.UserName;
			}
			welcome.Text = "Welcome " + username;
		}

		protected override void OnMojioEventReceived (Event eve)
		{
			base.OnMojioEventReceived (eve);
			UpdateNumberOfNewEvents ();
		}

		private void UpdateNumberOfNewEvents ()
		{
			if (!(MainApp.GetCurrentActivity () is MainMenuActivity)) {
				MyLogger.Information (this.LocalClassName, string.Format ("Notification Button not updated because MainMenuActivity is not visible."));
			} else {
				int numberOfNewEvents = TowManager.GetNewEventNumber ();
				MyLogger.Information (this.LocalClassName, string.Format ("{0} New Events found.", numberOfNewEvents));
				if (numberOfNewEvents == 0) {
					notifcationButton.Text = Resources.GetString (Resource.String.notifications);
				} else {
					var msg = string.Format ("{0} ({1})", Resources.GetString (Resource.String.notifications), numberOfNewEvents);
					notifcationButton.Text = msg;
				}
			}
		}

		private void OnNotificationsClicked (object sender, EventArgs e)
		{
			StartActivity (new Intent (this, typeof(NotificationsActivity)));
		}

		private void OnMapsClicked (object sender, EventArgs e)
		{
			StartActivity (new Intent (this, typeof(MapsActivity)));
		}

		private void OnSettingsClicked (object sender, EventArgs e)
		{
			StartActivity (new Intent (this, typeof(SettingsActivity)));
		}

		private void OnLogOutClicked (object sender, EventArgs e)
		{
			Client.ClearUser ();
			GotoLogin ();
		}

		private void GotoLogin ()
		{
			var login = new Intent (this, typeof(LoginActivity));
			login.AddFlags (ActivityFlags.ClearTop);
			StartActivity (login);
		}
	}
}

