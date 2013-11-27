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
using Android.Net;

using Mojio.Client;
using Mojio;
using PushSharp.Client;

namespace eecegroup32.mojiotowingalert.android
{
	public class BaseActivity : Activity
	{
		public static bool ConnectedToNetwork;

		public static string SharedPreferencesName = "MojioClientTestPreferences";
		public static string DevicePrefs = "MOJIO_DEVICE";
		public static string NotificationPref = "NOTIFICATION_SETTING";
		public static Device Dev;
		public static NotificationSetting Notif = new NotificationSetting();

		public static MyNotificationManager myNotificationManager = new MyNotificationManager();
		private static bool ActivityVisible;

		public MojioClient Client
		{
			get { return MainApp.Client; }
		}

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			ConnectedToNetwork = CheckNetworkConnection();

			//If not connected to network, display an alert with an Exit button
			//If Exit button is clicked, close the app
			if (!ConnectedToNetwork)
			{
				AlertDialog.Builder alert = new AlertDialog.Builder(this);
				alert.SetTitle("Error");
				alert.SetMessage(Resource.String.noConnectivity);
				alert.SetPositiveButton("Exit", delegate { Finish(); });
				alert.Show();
				return;
			}

			// Lets make sure we have registered for GCM messages.
			if (!PushClient.IsRegistered(this.ApplicationContext))
			{
				//Check to ensure everything's setup right
				PushClient.CheckDevice(this.ApplicationContext);
				PushClient.CheckManifest(this.ApplicationContext);

				//Call to register
				PushClient.Register(this.ApplicationContext, PushReceiver.SENDER_IDS);
				Boolean x = PushClient.IsRegistered (this.ApplicationContext);
			}
		}

		public static void setupDevice()
		{		
			var res = MainApp.Client.UserMojios (MainApp.Client.CurrentUser.Id);
			foreach (Device moj in res.Data) {
				Dev = moj;
			}
		}


		protected bool CheckNetworkConnection()
		{
			ConnectivityManager connectivityManager = (ConnectivityManager)GetSystemService(ConnectivityService); 
			NetworkInfo activeConnection = connectivityManager.ActiveNetworkInfo;
			if ((activeConnection != null) && activeConnection.IsConnected)
			{
				return true;
			}
			return false;
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();
		}

		// bookkeeping status of the app: in foreground or not
		protected override void OnResume()
		{
			base.OnResume();
			ActivityResumed();
		}

		protected override void OnPause()
		{
			base.OnPause();
			ActivityPaused();
		}

		public static bool IsActivityVisible()
		{
			return ActivityVisible;
		}

		public static void ActivityResumed()
		{
			ActivityVisible = true;
		}

		public static void ActivityPaused()
		{
			ActivityVisible = false;
		}

	}
}

