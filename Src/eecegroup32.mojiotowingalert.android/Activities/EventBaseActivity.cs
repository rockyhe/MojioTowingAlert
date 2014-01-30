using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Mojio.Events;
using Android.Util;
using PushSharp.Client;
using Mojio;
using Mojio.Client;
using System.Threading;

namespace eecegroup32.mojiotowingalert.android
{
    public abstract class EventBaseActivity : BaseActivity
    {
		protected static IntentFilter IntFilter;
		//protected static Context CurrentContext;
		protected static PushEventReceiver Receiver;

		public class PushEventReceiver : EventReceiver
		{
			protected override void OnEvent(Context context, Event ev)
			{
				logger.Information (this.Class.SimpleName, string.Format("Event Received: Context-{0} EventType-{1}", context.GetType().ToString(), ev.EventType.ToString()) );

				if( context is EventBaseActivity )
					(context as EventBaseActivity).OnMojioEventReceived(ev);

			}
		}

        protected override void OnCreate(Bundle bundle)
        {
			logger.Debug (this.LocalClassName, "Lifecycle Entered: OnCreate");

            base.OnCreate(bundle);
            InitializeComponents ();
			LoadMojioDevices();
            RegisterReceiver(Receiver, IntFilter);

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
			base.OnStop ();
			logger.Debug (this.LocalClassName, "Lifecycle Exited: OnStop");
		}

		protected override void OnDestroy()
		{
			logger.Debug (this.LocalClassName, "Lifecycle Entered: OnDestroy");
			try 
			{
				UnregisterReceiver(Receiver);
			}
			catch (Exception ex) 
			{
				logger.Error (this.LocalClassName, string.Format ("Tried to unregister when not registered. Exception: {0}", ex.Message));
			}
			base.OnDestroy();
			logger.Debug (this.LocalClassName, "Lifecycle Exited: OnDestroy");
		}

		protected override void OnResume()
		{
			logger.Debug (this.LocalClassName, "Lifecycle Entered: OnResume");
			base.OnResume();
			RegisterEventsNotice ();
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
			if (Receiver == null)
				Receiver = new PushEventReceiver ();

			if (IntFilter == null)
				IntFilter = new IntentFilter (EventReceiver.IntentAction);
		}

		protected void LoadLastEvents(int count = 5)
		{
			var query = from e in Client.Queryable<Event> ()
				            where e.EventType.Equals (EventToSubscribe)
							select e;
			query.Take (count);
			logger.Information (this.LocalClassName, "Querying...");
			// No requests have been sent to our server until this point.
			//  Now make API call and fetch entries and iterate over them
			foreach (var eve in query) {
				MainApp.MyNotificationsMgr.Add (new MyNotification (eve));
				logger.Information (this.LocalClassName, string.Format ("{0} is added from the Mojio sever", eve.Id));
			}
		}

		protected Subscription SubscribeForEvent (string registrationId, out HttpStatusCode httpStatusCode, out string msg, Device mojioDevice)
		{
			return Client.SubscribeGcm (registrationId, new Subscription () {
				Event = EventToSubscribe,
				EntityId = mojioDevice.Id,
				EntityType = SubscriptionType.Mojio,
			}, out httpStatusCode, out msg);
		}

		protected bool CheckSubscriptionStatus (HttpStatusCode httpStatusCode)
		{
			return httpStatusCode == HttpStatusCode.NotModified;
		}

		protected void UnsubscribeForEvent (string dongleID)
		{
			//TODO unsubscribe
			logger.Information (this.LocalClassName, string.Format ("Subscription (TO BE IMPLEMENTED): {0} unsubscribed for event type {1}", dongleID, EventToSubscribe));
//			Client.Unsubscribe(dongleID, new List<EventType> { MojioEventType });
//			logger.Information (this.LocalClassName, string.Format ("Subscription: {0} unsubscribed for event type {1}", dongleID, MojioEventType));
		}
 
        private void RegisterEventsNotice()
        {
			if (MojioDevices == null || MojioDevices.Count == 0)
				return;

            var registrationId = PushClient.GetRegistrationId(this.ApplicationContext);
			logger.Information (this.LocalClassName, string.Format("Event Notice Registration: ID {0} retrieved.", registrationId));
			if (String.IsNullOrWhiteSpace (registrationId)) {
				logger.Error (this.LocalClassName, "Event Notice Registration: failed - no registration ID retrieved.");
				return;
			}

			int trials; 
			bool isSubscribed = false;

			foreach (var mojioDevice in MojioDevices) 
			{
				if (!GetDeviceSubscriptionPref (mojioDevice.Id)) {
					UnsubscribeForEvent(mojioDevice.Id);
					continue;
				} 
				else 
				{
					trials = 3; 
					do {
						isSubscribed = SubscribeForEvent(mojioDevice, registrationId);
						if (isSubscribed)
							break;
						trials--;
					} while (trials > 0);
				}
            }            

            if (!isSubscribed)
            {
				logger.Error (this.LocalClassName, "Event Subscription: Failed.");
                Toast tmp = Toast.MakeText(this, "Subscription failed, please check network status", ToastLength.Long);
                tmp.SetGravity(GravityFlags.CenterVertical, 0, 0);
                tmp.Show();
            }
        }

		protected bool SubscribeForEvent(Device mojioDevice, string registrationId)
		{
			HttpStatusCode statusCode;
			string msg;
			Subscription subscription = SubscribeForEvent (registrationId, out statusCode, out msg, mojioDevice);
			if (subscription != null) {
				logger.Information (this.LocalClassName, string.Format ("Event Subscription: {0} - {1}.", "successful", msg));	                    
				return true;
			}

			if (CheckSubscriptionStatus (statusCode)) {
				logger.Information (this.LocalClassName, "Event Subscription: Event already subscribed.");	                    
				return true;
			}

			return false;
		}

        protected virtual void OnMojioEventReceived(Event eve)
        {
			MyNotificationsMgr.Add(new MyNotification(eve));
			if (MainApp.GetCurrentActivity () is NotificationsActivity)
				MyNotificationsMgr.ClearNumberOfNewNotifications ();
			else
				MyNotificationsMgr.IncrementNumberOfNewNotifications();
			if (MainApp.GetCurrentActivity() is NotificationsActivity) 
			{
				((NotificationsActivity)MainApp.GetCurrentActivity ()).Update ();
			}

			if (ActivityVisible) {
				logger.Information (this.Class.SimpleName, "App Visibility: Visible.");
				SendNotificationToast ();
			} 
			else 
			{
				logger.Information (this.Class.SimpleName, "App Visibility: Not Visible.");

				SendSystemNotification (this, eve);
			}
		}

		protected void SendNotificationToast(string msg="")
		{
			if (msg == "")
				msg = "New Notification Arrived!";
			logger.Information (this.Class.SimpleName, "Creating a toast for the new notification...");
			var temp = Toast.MakeText(MainApp.GetCurrentActivity(), msg, ToastLength.Long);
			temp.SetGravity(GravityFlags.CenterVertical, 0, 0);
			temp.Show();
		}

        protected void SendSystemNotification(Context context, Event eve)
        {
			logger.Information ("NOTIFICATION", "Local Notification: Preparing...");
			var isNotificationEnabled = GetNotificationTogglePref ();
			logger.Information ("NOTIFICATION", string.Format("Enable Preference: {0}", isNotificationEnabled));

			if (!isNotificationEnabled)
				return;
            
			var notificationMsg = string.Format ("New {0} Mojio Event Received!", eve.EventType.ToString ());
			var notification = new Notification(Resource.Drawable.Icon, notificationMsg);
            var pendingIntent = PendingIntent.GetActivity(context, 0, new Intent(context, context.GetType()), 0);
            notification.SetLatestEventInfo(context, "New Mojio event", eve.EventType.ToString(), pendingIntent);
            notification.Flags = NotificationFlags.AutoCancel;

			ConfigureNotificationSound (notification);
			ConfigureNotificationVibration (notification);

			var nMgr = (NotificationManager) context.GetSystemService(NotificationService);
            nMgr.Notify(0, notification);
			logger.Information ("NOTIFICATION", "Local Notification: Completed.");
        }

		protected void ConfigureNotificationSound(Notification notif)
		{
			var isSoundEnabled = GetNotificationSoundPref ();
			logger.Information ("NOTIFICATION", string.Format("Sound Preference: {0}", isSoundEnabled));

			if (isSoundEnabled)
				notif.Defaults |= NotificationDefaults.Sound;
			else
				notif.Sound = null;
		}

		protected void ConfigureNotificationVibration(Notification notif)
		{
			var isVibrationEnabled = GetNotificationVibrationPref ();
			logger.Information ("NOTIFICATION", string.Format("Vibration Preference: {0}", isVibrationEnabled));

			if (isVibrationEnabled)
				notif.Defaults |= NotificationDefaults.Vibrate;
			else
				notif.Vibrate = null;
		}
    }
}