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
	public class MainMenuActivity : BaseActivity
	{
		Button notifcationButton;
		Button mapsButton;
		Button settingsButton;
		Button logOutButton;
		TextView welcome;

		//private static TestReceiver _receiver;
		//private static IntentFilter _intentFilter;
		//protected static Context CurContext;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			SetContentView (Resource.Layout.MainMenu);

			welcome = FindViewById<TextView> (Resource.Id.welcomeText);

			string username = string.Empty;
			if (Client != null && Client.CurrentUser != null)
			{
				username = Client.CurrentUser.UserName;
			}
		

			welcome.Text = "Welcome " + username;
			// Get button from the layout resource and attach an event to it
			notifcationButton = FindViewById<Button>(Resource.Id.notificationsButton);
			notifcationButton.Click += new EventHandler(OnNotificationsClicked);

			mapsButton = FindViewById<Button>(Resource.Id.mapsButton);
			mapsButton.Click += new EventHandler(OnMapsClicked);

			settingsButton = FindViewById<Button>(Resource.Id.settingsButton);
			settingsButton.Click += new EventHandler(OnSettingsClicked);

			logOutButton = FindViewById<Button>(Resource.Id.logOutButton);
			logOutButton.Click += new EventHandler(OnLogOutClicked);

//			// Instanciate a new push receiver.
//			if(_receiver == null)
//				_receiver = new TestReceiver();
//
//			// Retrieve the intent type broadcasted by Event Receiver.
//			if (_intentFilter == null)
//				_intentFilter = new IntentFilter(EventReceiver.IntentAction);
//
//			// Register the receiver to receive our GCM events
//			RegisterReceiver(_receiver, _intentFilter);
		}

		protected override void OnStart()
		{
			base.OnStart();
		}

		protected override void OnResume()
		{
			base.OnResume();
		}


		private void OnNotificationsClicked(object sender, EventArgs e)
		{
			var notif = new Intent(this, typeof(NotificationsActivity));
			StartActivity(notif);
		}

		private void OnMapsClicked(object sender, EventArgs e)
		{
			var maps = new Intent(this, typeof(MapsActivity));
			StartActivity(maps);
		}

		private void OnSettingsClicked(object sender, EventArgs e)
		{
			var settings = new Intent(this, typeof(SettingsActivity));
			/** NEED TO PREVENT MAIN MENU ACTIVITY FROM BEING DESTROYED **/
			StartActivity(settings);
		}

		private void OnLogOutClicked(object sender, EventArgs e)
		{
			//Clear the user session and go to login
			Client.ClearUser();
			GotoLogin();
		}

		private void GotoLogin()
		{
			var login = new Intent(this, typeof(LoginActivity));
			login.AddFlags(ActivityFlags.ClearTop);
			StartActivity(login);
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();
//			UnregisterReceiver(_receiver);
		}

		/**
		 * This is a demo event receiver.  This will be called any time an event
		 * is received via push notifications.  
		 * (It was defined as a receiver in EventBaseActivity::OnCreate)
		 * 
		 * Events to be broadcast to this device are set in 
		 * EventsBaseActivity::RegisterEventsNotice
		 */
//		public class TestReceiver : EventReceiver
//		{
//
//			protected override void OnEvent(Context context, Event ev)
//			{
//				if (context != CurContext)
//					return;
//
//				if( context is MainMenuActivity )
//					(context as MainMenuActivity).OnMojioEventReceived(ev);
//
//
//			}
//		}
//
//		protected void SendSystemNotification(Context context, Event eve)
//        {
//			// When event is received while app is inactive, lets create a notification popup.
//            var nMgr = (NotificationManager)context.GetSystemService(NotificationService);
//            var notification = new Notification(Resource.Drawable.Icon, "New Mojio event received");
//            var pendingIntent = PendingIntent.GetActivity(context, 0, new Intent(context, context.GetType()), 0);
//            notification.SetLatestEventInfo(context, "New Mojio event", eve.EventType.ToString(), pendingIntent);
//            notification.Flags = NotificationFlags.AutoCancel;
//            nMgr.Notify(0, notification);
//        }
//
//		protected virtual void OnMojioEventReceived(Event eve)
//		{
//			MyNotification notification = new MyNotification (eve);
//			myNotificationManager.AddMyNotification (notification);
//
//			if (!IsActivityVisible())
//				SendSystemNotification(CurContext, eve);
//		}

	}
}

