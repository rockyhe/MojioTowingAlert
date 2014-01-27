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
	public abstract class BaseActivity : Activity
	{
		protected static readonly string SharedPreferencesName = "MOJIO_APP_PREFERENCES";
		protected static readonly string SubscriptionTogglePref = "SUBSCRIPTION_TOGGLE_PREFERENCE";
		protected static readonly string NotificationTogglePref = "NOTIFICATION_TOGGLE_PREFERENCE";
		protected static readonly string NotificationSoundPref = "NOTIFICATION_SOUND_PREFERENCE";
		protected static readonly string NotificationVibrationPref = "NOTIFICATION_VIBRATION_PREFERENCE";

		protected static bool ActivityVisible;
		protected static bool ConnectedToNetwork;
		protected static IList<Device> MojioDevices;
		protected static readonly Mojio.Events.EventType MojioEventType = Mojio.Events.EventType.TripStart;
		protected static ILogger logger = MainApp.Logger;
		protected static MyNotificationManager MyNotificationsMgr = MainApp.MyNotificationsMgr;

		public MojioClient Client
		{
			get { return MainApp.Client; }
		}

		protected override void OnCreate(Bundle savedInstanceState)
		{
			logger.Debug (this.LocalClassName, "Lifecycle Entered: OnCreate");

			base.OnCreate(savedInstanceState);
			MojioDevices = new List<Device> ();

			if (!CheckNetworkConnection())
			{
				DisplayNetworkAlert ();
				return;
			}

			SetupGCM ();
			SetActivityVisible (true);

			logger.Debug (this.LocalClassName, "Lifecycle Exited: OnCreate");
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
			SetActivityVisible(true);
			logger.Debug (this.LocalClassName, "Lifecycle Exited: OnResume");
		}

		protected override void OnPause()
		{
			logger.Debug (this.LocalClassName, "Lifecycle Entered: OnPause");
			base.OnPause();
			SetActivityVisible(false);
			logger.Debug (this.LocalClassName, "Lifecycle Exited: OnPause");
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

		protected void SetupGCM ()
		{
			var isRegisteredForGCM = PushClient.IsRegistered (this.ApplicationContext);
			if (isRegisteredForGCM) {
				logger.Information (this.LocalClassName, "GCM Registration: already registered.");
			}
			else {
				logger.Information (this.LocalClassName, "GCM Registration: not registered.");
				RegisterForGcmMsgs ();
				isRegisteredForGCM = PushClient.IsRegistered (this.ApplicationContext);
				logger.Information (this.LocalClassName, string.Format ("GCM Registration: {0}.", isRegisteredForGCM));
			}
		}

		protected void DisplayNetworkAlert ()
		{
			AlertDialog.Builder alert = new AlertDialog.Builder (this);
			alert.SetTitle ("Error");
			alert.SetMessage (Resource.String.noConnectivity);
			alert.SetPositiveButton ("Exit", delegate {
				Finish ();
			});
			alert.Show ();
		}

		protected void RegisterForGcmMsgs ()
		{
			logger.Information (this.LocalClassName, "GCM Registration: Registering...");
			try {
				PushClient.CheckDevice (this.ApplicationContext);
				PushClient.CheckManifest (this.ApplicationContext);
				PushClient.Register (this.ApplicationContext, PushReceiver.SENDER_IDS);
				logger.Information (this.LocalClassName, "GCM Registration: Registration completed.");
			} catch (Exception ex) {
				logger.Error (this.LocalClassName, string.Format("GCM Registration: Registration failed. Exception: {0}", ex.Message));
			}
		}

		protected virtual void LoadMojioDevices()
		{
			logger.Information (this.LocalClassName, "Mojio Devices: Retrieving...");
			Results<Device> res = MainApp.Client.UserMojios(MainApp.Client.CurrentUser.Id);

			if (MojioDevices == null) 
			{
				MojioDevices = new List<Device> ();
			}

			MojioDevices.Clear ();

			foreach (Device moj in res.Data)
			{
				MojioDevices.Add(moj);
				logger.Information (this.LocalClassName, string.Format("Mojio Devices: {0} retrieved.", moj.Name));
			}
		}

		protected bool CheckNetworkConnection()
		{
			logger.Information (this.LocalClassName, "Network Connection: Checking..."); 
			ConnectivityManager connectivityManager = (ConnectivityManager)GetSystemService(ConnectivityService); 
			NetworkInfo activeConnection = connectivityManager.ActiveNetworkInfo;

			if ((activeConnection != null) && activeConnection.IsConnected) {
				logger.Information (this.LocalClassName, "Network Connection: Good"); 
				return true;
			} 
			else 
			{
				logger.Information (this.LocalClassName, "Network Connection: Bad"); 
				DisplayNetworkAlert ();
				return false;
			}
		}

		public bool IsActivityVisible()
		{
			return ActivityVisible;
		}

		private void SetActivityVisible(bool isVisible)
		{
			ActivityVisible = isVisible;
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

		public string GetDeviceSubscriptionPrefKey(string id)
		{
			return SubscriptionTogglePref + "=" + id;
		}

		public bool GetDeviceSubscriptionPref(string id)
		{
			return GetNotificationSetting (GetDeviceSubscriptionPrefKey(id));
		}

		protected bool GetNotificationSetting(String option)
		{
			var preferences = GetSharedPreferences(SharedPreferencesName, FileCreationMode.Private); 
			var resultString = preferences.GetString (option, Boolean.TrueString);
			if (resultString == null) 
			{
				logger.Information (this.LocalClassName, string.Format("Settings - {0}: {1}", option, "Not Found")); 
				return false;
			}
			var result = Boolean.Parse(resultString);
			logger.Information (this.LocalClassName, string.Format("Settings - {0}: {1}", option, result)); 
			return result;
		}

	}
}

