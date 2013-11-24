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
using Android.Util;
using PushSharp.Client;
using Mojio.Client;
using System.Threading;

namespace eecegroup32.mojiotowingalert.android
{
	[Activity (Label = "NotificationsActivity")]			
	public class NotificationsActivity : BaseActivity
	{
		private static TestReceiver _receiver;
		private static IntentFilter _intentFilter;
		protected static List<Event> ReceivedEvents;
		protected static Context CurContext;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			// Create your application here
			// Initialize our list of events with the last 5 events.
			if (ReceivedEvents == null)
				ReceivedEvents = new List<Event> ();

			// Instanciate a new push receiver.
			if(_receiver == null)
				_receiver = new TestReceiver();

			// Retrieve the intent type broadcasted by Event Receiver.
			if (_intentFilter == null)
				_intentFilter = new IntentFilter(EventReceiver.IntentAction);

			// Register the receiver to receive our GCM events
			RegisterReceiver(_receiver, _intentFilter);

		}

		protected override void OnResume()
		{
			base.OnResume();
			ShowNotifications();
		}

		protected void ShowNotifications()
		{
			//Add to notifications screen
			var notificationList = this.FindViewById<LinearLayout>(Resource.Id.notificationIDLayout);
			TextView notificationToAdd = new TextView(this);
			foreach (MyNotification notif in myNotificationManager.getMyNotifications())
			{
				notificationToAdd.Text = (notif.getmMyNotificationId ());
				notificationList.AddView (notificationToAdd);
			}

			var dateList = this.FindViewById<LinearLayout>(Resource.Id.dateLayout);
			TextView dateToAdd = new TextView(this);;
			foreach (MyNotification notif in myNotificationManager.getMyNotifications())
			{
				dateToAdd.Text = notif.getEvent ().Time.ToString ("f");
				notificationList.AddView (dateToAdd);
			}

		}

		protected override void OnDestroy()
		{
			base.OnDestroy();
			UnregisterReceiver(_receiver);
		}

		/*		*
		 * This is a demo event receiver.  This will be called any time an event
		 * is received via push notifications.  
		 * (It was defined as a receiver in EventBaseActivity::OnCreate)
		 * 
		 * Events to be broadcast to this device are set in 
		 * EventsBaseActivity::RegisterEventsNotice
		 */
		public class TestReceiver : EventReceiver
		{

			protected override void OnEvent(Context context, Event ev)
			{
				if (context != CurContext)
					return;

				if( context is NotificationsActivity )
					(context as NotificationsActivity).OnMojioEventReceived(ev);

				//Log.Verbose(string.Format("Event Received Context: {0} EventType: {1}",
				//context.GetType().ToString(), ev.EventType.ToString()) );
			}
		}

		protected virtual void OnMojioEventReceived(Event eve)
		{
			// Add the event to our event list
			AddMojioEvent(eve);

			//Create notification
			MyNotification notification = new MyNotification (eve);

			//Add to notifications
			myNotificationManager.AddMyNotification (notification);


			if (!IsActivityVisible())
				SendSystemNotification(CurContext, eve);
		}

		protected void AddMojioEvent(Event eve)
		{
				ReceivedEvents.Add(eve);
		}

		protected void ClearMojioEvents() { ReceivedEvents.Clear(); }

		// create a notification popup.
		protected void SendSystemNotification(Context context, Event eve)
		{
			// When event is received while app is inactive, lets create a notification popup.
			var nMgr = (NotificationManager)context.GetSystemService(NotificationService);
			var notification = new Notification(Resource.Drawable.Icon, "New Mojio event received");
			var pendingIntent = PendingIntent.GetActivity(context, 0, new Intent(context, context.GetType()), 0);
			notification.SetLatestEventInfo(context, "New Mojio event", eve.EventType.ToString(), pendingIntent);
			notification.Flags = NotificationFlags.AutoCancel;
		}
	}
}

