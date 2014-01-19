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
		private static string logTag = "BaseActivity";

		public static bool ConnectedToNetwork;
		public static string SharedPreferencesName = "MojioClientTestPreferences";
		public static string NotificationTogglePref = "NOTIFICATION_TOGGLE_PREFERENCE";
		public static string NotificationSoundPref = "NOTIFICATION_SOUND_PREFERENCE";
		public static string NotificationVibrationPref = "NOTIFICATION_VIBRATION_PREFERENCE";
		public static Device MojioDevice;

		public static MyNotificationManager myNotificationManager = new MyNotificationManager();
		private static bool ActivityVisible;

		//Reference to single instance of Mojio client
		public MojioClient Client
		{
			get { return MainApp.Client; }
		}

		void DisplayNetworkAlert ()
		{
			AlertDialog.Builder alert = new AlertDialog.Builder (this);
			alert.SetTitle ("Error");
			alert.SetMessage (Resource.String.noConnectivity);
			alert.SetPositiveButton ("Exit", delegate {
				Finish ();
			});
			alert.Show ();
		}

		void RegisterForGcmMsgs ()
		{
			PushClient.CheckDevice (this.ApplicationContext);
			PushClient.CheckManifest (this.ApplicationContext);
			Android.Util.Log.Info (logTag, "Registering For GCM Msgs");
			PushClient.Register (this.ApplicationContext, PushReceiver.SENDER_IDS);
		}

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			ConnectedToNetwork = CheckNetworkConnection();

			if (!ConnectedToNetwork)
			{
				DisplayNetworkAlert ();
				return;
			}

			var isRegisteredForGCM = PushClient.IsRegistered (this.ApplicationContext);
			Android.Util.Log.Info(logTag, string.Format("Is Registered For GCM Msgs: {0}", isRegisteredForGCM));

			if (!isRegisteredForGCM)
			{
				RegisterForGcmMsgs ();
				isRegisteredForGCM = PushClient.IsRegistered (this.ApplicationContext);
				Android.Util.Log.Info(logTag, string.Format("Is Registered For GCM Msgs: {0}", isRegisteredForGCM));
			}
		}

		public static void SetupDevice()
		{
			//TODO: Currently assuming only one device per user.
			Results<Device> res = MainApp.Client.UserMojios(MainApp.Client.CurrentUser.Id);
			foreach (Device moj in res.Data)
			{
				MojioDevice = moj;
			}
		}


		protected bool CheckNetworkConnection()
		{
			Android.Util.Log.Info(logTag, "Checking: Network Connection..."); 
			ConnectivityManager connectivityManager = (ConnectivityManager)GetSystemService(ConnectivityService); 
			NetworkInfo activeConnection = connectivityManager.ActiveNetworkInfo;
			if ((activeConnection != null) && activeConnection.IsConnected)
			{
				Android.Util.Log.Info(logTag, "Network: Good"); 
				return true;
			}

			Android.Util.Log.Error(logTag, "Network: Bad"); 
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

		public bool GetNotificationTogglePref()
		{
			return GetNotificationSetting (NotificationTogglePref);
		}

		public bool GetNotificationSoundPref()
		{
			return GetNotificationSetting (NotificationSoundPref);
		}

		public bool GetNotificationVibrationPref()
		{
			return GetNotificationSetting (NotificationVibrationPref);
		}

		private bool GetNotificationSetting(String option)
		{
			var preferences = GetSharedPreferences(SharedPreferencesName, FileCreationMode.Private); 
			var result = Boolean.Parse(preferences.GetString (option, Boolean.TrueString));
			Android.Util.Log.Info(logTag, string.Format("Settings - {0}: {1}", option, result)); 
			return result;
		}

	}
}

