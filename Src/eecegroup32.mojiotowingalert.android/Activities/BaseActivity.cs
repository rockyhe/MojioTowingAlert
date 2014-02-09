using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Android.OS;
using Android.Net;
using Android.App;
using Android.Widget;
using Android.Views;
using Android.Content;
using Mojio;
using Mojio.Events;
using Mojio.Client;
using PushSharp.Client;
using eecegroup32.mojiotowingalert.core;

namespace eecegroup32.mojiotowingalert.android
{
	public abstract class BaseActivity : Activity
	{
		protected static bool ActivityVisible { get; set; }

		protected static bool ConnectedToNetwork { get; set; }

		protected static IList<Device> UserDevices { get; set; }

		protected static IList<EventType> EventsToSubscribe { get; set; }

		protected static IList<AbstractNotificationManager> NotificationManagers { get; set; }

		protected static AbstractNotificationManager TowManager { get; set; }

		protected static UserPreference CurrentUserPreference { get; set; }

		protected static string RegistrationId { get; set; }

		public MojioClient Client { get { return MainApp.Client; } }

		protected override void OnCreate (Bundle savedInstanceState)
		{
			MyLogger.Debug (this.LocalClassName, "Lifecycle Entered: OnCreate");
			base.OnCreate (savedInstanceState);
			var connected = CheckNetworkConnection ();
			if (!connected)
				DisplayNetworkAlert ();		
			InitializeVariables ();
			CreateNotificationManagers ();
			SetupGCM ();
			SetActivityVisible (true);
			MyLogger.Debug (this.LocalClassName, "Lifecycle Exited: OnCreate");
		}

		private void CreateNotificationManagers ()
		{
			MyLogger.Information (this.LocalClassName, "Creating Notification Managers");
			if (TowManager == null)
				TowManager = TowNotificationManagerFactory.GetFactory ().Create (EventType.Tow);
			if (NotificationManagers == null) {
				NotificationManagers = new List<AbstractNotificationManager> ();
				NotificationManagers.Add (TowManager);
			}
			MyLogger.Information (this.LocalClassName, "Notification Managers Created");
		}

		protected void LoadLastEvents (IEnumerable<EventType> eventsToLoad, int count = 10)
		{
			foreach (var eveType in eventsToLoad) {
				MyLogger.Information (this.LocalClassName, string.Format ("Querying last {0} events from the server.", count));
				var query = from e in Client.Queryable<Event> ()
				            where e.EventType.Equals (eveType)
				            select e;
				query.Take (count);
			
				foreach (var e in query) {
					TowManager.Add (e);
					MyLogger.Information (this.LocalClassName, string.Format ("{0} is retrieved from the Mojio sever", e.Id));
				}
			}
		}
		//[GROUP 32] Load all events available for this app but allow the user to choose what
		//event types to see in the notification list (like a tabview?)
		private void InitializeVariables ()
		{
			if (EventsToSubscribe == null)
				EventsToSubscribe = new List<EventType> () { EventType.Tow };
				
			if (UserDevices == null)
				UserDevices = new List<Device> ();
		}

		protected override void OnDestroy ()
		{
			MyLogger.Debug (this.LocalClassName, "Lifecycle Entered: OnDestroy");
			base.OnDestroy ();
			MyLogger.Debug (this.LocalClassName, "Lifecycle Exited: OnDestroy");
		}

		protected override void OnResume ()
		{
			MyLogger.Debug (this.LocalClassName, "Lifecycle Entered: OnResume");
			base.OnResume ();
			SetActivityVisible (true);
			MyLogger.Debug (this.LocalClassName, "Lifecycle Exited: OnResume");
		}

		protected override void OnPause ()
		{
			MyLogger.Debug (this.LocalClassName, "Lifecycle Entered: OnPause");
			base.OnPause ();
			SetActivityVisible (false);
			MyLogger.Debug (this.LocalClassName, "Lifecycle Exited: OnPause");
		}

		protected override void OnStart ()
		{
			MyLogger.Debug (this.LocalClassName, "Lifecycle Entered: OnStart");
			base.OnStart ();		
			MyLogger.Debug (this.LocalClassName, "Lifecycle Exited: OnStart");
		}

		protected override void OnStop ()
		{
			MyLogger.Debug (this.LocalClassName, "Lifecycle Entered: OnStop");
			base.OnStop ();		
			MyLogger.Debug (this.LocalClassName, "Lifecycle Exited: OnStop");
		}

		private void SetupGCM ()
		{
			var isRegisteredForGCM = PushClient.IsRegistered (this.ApplicationContext);
			if (isRegisteredForGCM) {
				MyLogger.Information (this.LocalClassName, "GCM Registration: already registered.");
			} else {
				MyLogger.Information (this.LocalClassName, "GCM Registration: not registered.");
				RegisterForGcmMsgs ();
				isRegisteredForGCM = PushClient.IsRegistered (this.ApplicationContext);
				MyLogger.Information (this.LocalClassName, string.Format ("GCM Registration: {0}.", isRegisteredForGCM));
			}
		}

		protected virtual void LoadMojioDevices ()
		{
			MyLogger.Information (this.LocalClassName, "Mojio Devices: Retrieving...");			
			Results<Device> res = MainApp.Client.UserMojios (MainApp.Client.CurrentUser.Id);
			UserDevices.Clear ();
			foreach (Device moj in res.Data) {
				UserDevices.Add (moj);
				MyLogger.Information (this.LocalClassName, string.Format ("Mojio Devices: {0} Retrieved", moj.Id));
			}
			MyLogger.Information (this.LocalClassName, "Mojio Devices: Done Retrieving");
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

		private void RegisterForGcmMsgs ()
		{
			MyLogger.Information (this.LocalClassName, "GCM Registration: Registering...");
			try {
				PushClient.CheckDevice (this.ApplicationContext);
				PushClient.CheckManifest (this.ApplicationContext);
				PushClient.Register (this.ApplicationContext, PushReceiver.SENDER_IDS);
				MyLogger.Information (this.LocalClassName, "GCM Registration: Registration completed.");
			} catch (Exception ex) {
				MyLogger.Error (this.LocalClassName, string.Format ("GCM Registration: Registration failed. Exception: {0}", ex.Message));
			}
		}

		protected bool CheckNetworkConnection ()
		{
			MyLogger.Information (this.LocalClassName, "Network Connection: Checking..."); 
			ConnectivityManager connectivityManager = (ConnectivityManager)GetSystemService (ConnectivityService); 
			NetworkInfo activeConnection = connectivityManager.ActiveNetworkInfo;

			if ((activeConnection != null) && connectivityManager.ActiveNetworkInfo.IsConnected) {
				MyLogger.Information (this.LocalClassName, "Network Connection: Good"); 
				return true;
			} else {
				MyLogger.Information (this.LocalClassName, "Network Connection: Bad"); 
				DisplayNetworkAlert ();
				return false;
			}
		}

		public bool IsActivityVisible ()
		{
			return ActivityVisible;
		}

		private void SetActivityVisible (bool isVisible)
		{
			ActivityVisible = isVisible;
		}

		protected bool GetNotificationTogglePref ()
		{
			var r = MyDataManager.GetUserPreference (Client.CurrentUser.UserName);
			return r == null ? true : r.NotificationChecked;
		}

		protected bool GetNotificationVibrationPref ()
		{
			var r = MyDataManager.GetUserPreference (Client.CurrentUser.UserName);
			return r == null ? true : r.VibrationChecked;
		}

		protected bool GetNotificationSoundPref ()
		{
			var r = MyDataManager.GetUserPreference (Client.CurrentUser.UserName);
			return r == null ? true : r.SoundChecked;
		}

		protected void NotifyViaToast (string msg = "New Event Arrived!")
		{
			var toast = Toast.MakeText (MainApp.GetCurrentActivity (), msg, ToastLength.Long);
			toast.SetGravity (GravityFlags.CenterVertical, 0, 0);
			toast.Show ();
			MyLogger.Information (this.LocalClassName, "Toast Notification: Sent.");
		}

		protected void NotifyViaLocalNotification (string msg = "New Event Arrived!")
		{
			var isNotificationEnabled = GetNotificationTogglePref ();
			MyLogger.Information (this.LocalClassName, string.Format ("Notification Toggle Preference: {0}", isNotificationEnabled ? "On" : "Off"));

			if (!isNotificationEnabled)
				return;
            
			var notification = new Notification (Resource.Drawable.Icon, msg);
			var pendingIntent = PendingIntent.GetActivity (this, 0, new Intent (this, this.GetType ()), 0);
			notification.SetLatestEventInfo (this, "New Mojio Event", msg, pendingIntent);
			notification.Flags = NotificationFlags.AutoCancel;

			ConfigureNotificationSound (notification);
			ConfigureNotificationVibration (notification);

			var nMgr = (NotificationManager)this.GetSystemService (NotificationService);
			nMgr.Notify (0, notification);
			MyLogger.Information (this.LocalClassName, "Local Notification: Sent.");
		}

		protected void ConfigureNotificationSound (Notification notif)
		{
			var isSoundEnabled = GetNotificationSoundPref ();
			MyLogger.Information ("NOTIFICATION", string.Format ("Sound Preference: {0}", isSoundEnabled ? "On" : "Off"));

			if (isSoundEnabled)
				notif.Defaults |= NotificationDefaults.Sound;
			else
				notif.Sound = null;
		}

		protected void ConfigureNotificationVibration (Notification notif)
		{
			var isVibrationEnabled = GetNotificationVibrationPref ();
			MyLogger.Information ("NOTIFICATION", string.Format ("Vibration Preference: {0}", isVibrationEnabled ? "On" : "Off"));

			if (isVibrationEnabled)
				notif.Defaults |= NotificationDefaults.Vibrate;
			else
				notif.Vibrate = null;
		}
	}
}

