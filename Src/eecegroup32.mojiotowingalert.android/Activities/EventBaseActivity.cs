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
        private static TestReceiver _receiver;
        private static IntentFilter _intentFilter;
        protected static List<Event> ReceivedEvents;
        protected static Context CurContext;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

			// Initialize our list of events with the last 5 events.
			if (ReceivedEvents == null)
				ReceivedEvents = new List<Event> ();

			// Instanciate a new push receiver.
            if(_receiver == null)
                _receiver = new TestReceiver();

			// Retrieve the intent type broadcasted by Event Receiver.
            if (_intentFilter == null)
                _intentFilter = new IntentFilter(EventReceiver.IntentAction);

			SetupDevice();

			// Register the receiver to receive our GCM events
            RegisterReceiver(_receiver, _intentFilter);
			if (Dev != null)
			{
				RegisterEventsNotice ();
			}
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            UnregisterReceiver(_receiver);
        }

		private void LoadLastEvents(int count = 10)
		{
			// Use Linq queries to query our API for list of events.
			var query = from e in Client.Queryable<Event> ()
							where e.EventType.Equals (EventType.TripStart)
							orderby e.Time descending
						select e;

			// Specify how many entries to receive
			query.Take (count);

			// No requests have been sent to our server until this point.
			//  Now make API call and fetch entries and iterate over them
			foreach (var eve in query)
				RunOnUiThread( () => AddMojioEvent (eve) );
		}
 
        private void RegisterEventsNotice()
        {
			// Fetch registration ID given to this app
            var registrationId = PushClient.GetRegistrationId(this.ApplicationContext);

			if (String.IsNullOrWhiteSpace (registrationId)) {
				//Log.Error ("Could not register for events, no registration ID.");
				return;
			}

            int trials = 3; 
            HttpStatusCode stat;
            string msg;
            Subscription sub;
            bool succeed = false;
            do
            {
				// Notify mojio servers what types of events we wish to receive.
                sub = Client.SubscribeGcm(registrationId, new Subscription()
                {
					Event = EventType.TripStart,			// We want to register to TripStart events
                    EntityId = Dev.Id,						// For this particular mojio device
                    EntityType = SubscriptionType.Mojio,
                }, out stat, out msg);

                if (sub != null)
                {
					//Log.Verbose("Event subscription succeeded: " + msg);
                    succeed = true;
                    break;
                }
                if(stat == HttpStatusCode.NotModified)
                {
					// We were already registered to this event type.
					//Log.Notice("Event already subscribed.");
                    succeed = true;
                    break;
                }

                trials--;
				//Log.Notice("GPSEvent subscription failed, trials left: " + trials);
				//Log.Notice("HttpStatusCode: "+ stat.ToString() + " Message:" + msg);
            }
            while (trials > 0);

            if (!succeed)
            {
                // Write the checkpoint to Test Flight.
				//Log.Error("User GPSEvent subscription failure.");

                Toast tmp = Toast.MakeText(this, "Subscription failed, please check network status", ToastLength.Long);
                tmp.SetGravity(GravityFlags.CenterVertical, 0, 0);
                tmp.Show();
            }
        }

		/**
		 * This is a demo event receiver.  This will be called any time an event
		 * is received via push notifications.  
		 * (It was defined as a receiver in EventBaseActivity::OnCreate)
		 * 
		 * Events to be broadcast to this device are set in 
		 * EventsBaseActivity::RegisterEventsNotice
		 */
        public class TestReceiver : EventReceiver
        {
			//private readonly ILogger Log = DependancyResolver.Get <ILogger> ();

            protected override void OnEvent(Context context, Event ev)
            {
                if (context != CurContext)
                    return;

				if( context is EventBaseActivity )
                	(context as EventBaseActivity).OnMojioEventReceived(ev);

				//Log.Verbose(string.Format("Event Received Context: {0} EventType: {1}",
				//                        context.GetType().ToString(), ev.EventType.ToString()) );
            }
        }

        protected virtual void OnMojioEventReceived(Event eve)
        {
			// Add the event to our event list
            AddMojioEvent(eve);

			// Create a Notification object and add it to our notification list
			MyNotification notification = new MyNotification(eve);
			myNotificationManager.AddMyNotification(notification);

			//Send a system notification only if activity is not visible
            if (!IsActivityVisible())
			{
                SendSystemNotification(CurContext, eve);
        	}
		}

        protected void AddMojioEvent(Event eve)
        {
			if (ReceivedEvents.Exists (e => e.Id == eve.Id))
				return;//Log.Error ("Duplicate event received!  " + eve.Id);;
			else
                ReceivedEvents.Add(eve);
        }

        protected void ClearMojioEvents() { ReceivedEvents.Clear(); }

        protected void SendSystemNotification(Context context, Event eve)
        {
			// When event is received while app is inactive, lets create a notification popup.
            var nMgr = (NotificationManager)context.GetSystemService(NotificationService);
            var notification = new Notification(Resource.Drawable.Icon, "New Mojio event received");
            var pendingIntent = PendingIntent.GetActivity(context, 0, new Intent(context, context.GetType()), 0);
            notification.SetLatestEventInfo(context, "New Mojio event", eve.EventType.ToString(), pendingIntent);
            notification.Flags = NotificationFlags.AutoCancel;
            nMgr.Notify(0, notification);
        }
    }
}