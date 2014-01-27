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
	[Activity (Label = "NotificationsActivity")]			
	public class NotificationsActivity : BaseActivity
	{
		private LinearLayout notificationList;
		private LinearLayout dateList;

		protected override void OnCreate (Bundle bundle)
		{
			logger.Debug (this.LocalClassName, "Lifecycle Entered: OnCreate");

			base.OnCreate(bundle);
			SetContentView(Resource.Layout.Notifications);
			InitializeComponents ();

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
			ShowNotificationList();
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
			notificationList = this.FindViewById<LinearLayout> (Resource.Id.notificationIDLayout);
			dateList = this.FindViewById<LinearLayout>(Resource.Id.dateLayout);
		}

		private void OnEventItemClicked(MyNotification notif)
		{
			MainApp.SelectedNotification = notif;
			logger.Information (this.LocalClassName, string.Format ("Notificiation Clicked: {0}", notif.NotificationID));
			StartActivity(new Intent(this, typeof(DetailsActivity)));
		}

		//TODO Instead of separate textview for id and date, combine them
		//TODO notification id not so meaningful. Use something else.
		private void AddNotificationsToScreen ()
		{
			TextView eventID, eventDate;

			foreach (MyNotification notif in MyNotificationsMgr.GetAll ()) {
				eventID = new TextView (this);
				eventID.Text = (notif.NotificationID);
				eventID.Clickable = true;
				eventID.Click += (sender, e) => OnEventItemClicked (notif);
				notificationList.AddView (eventID);
				eventDate = new TextView (this);
				eventDate.Text = notif.MojioEvent.Time.ToString ("f");
				eventDate.Clickable = true;
				eventDate.Click += (sender, e) => OnEventItemClicked (notif);
				dateList.AddView (eventDate);
			}
		}

		private void ClearNotificationList ()
		{
			notificationList.RemoveAllViews ();
			dateList.RemoveAllViews ();
		}

		//TODO load events stored in the server too
		private void ShowNotificationList()
		{
			ClearNotificationList ();
			AddNotificationsToScreen ();
		}

		//TODO update just the new event
		public void Update()
		{
			logger.Information (this.LocalClassName, "Notification List updated.");
			ShowNotificationList ();
		}
	}
}

