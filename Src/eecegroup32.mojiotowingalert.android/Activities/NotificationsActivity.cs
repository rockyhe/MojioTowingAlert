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
using eecegroup32.mojiotowingalert.core;
using System.Threading;
using System.Threading.Tasks;

namespace eecegroup32.mojiotowingalert.android
{
	[Activity (Label = "NotificationsActivity")]			
	public class NotificationsActivity : BaseActivity
	{
		private LinearLayout notificationList;
		private LinearLayout dateList;

		protected override void OnCreate (Bundle bundle)
		{
			MyLogger.Debug (this.LocalClassName, "Lifecycle Entered: OnCreate");

			base.OnCreate (bundle);
			SetContentView (Resource.Layout.Notifications);
			InitializeComponents ();
			Task.Factory.StartNew (() => LoadLastEvents (EventsToSubscribe)).ContinueWith (e => {
				RunOnUiThread (() => {
					Update ();
				});
			});
			MyLogger.Debug (this.LocalClassName, "Lifecycle Exited: OnCreate");
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
			MainApp.SetCurrentActivity (this);
			
			MyLogger.Debug (this.LocalClassName, "Lifecycle Exited: OnResume");
		}

		protected override void OnRestart ()
		{
			base.OnRestart ();
			Update ();
		}

		protected override void OnPause ()
		{
			MyLogger.Debug (this.LocalClassName, "Lifecycle Entered: OnPause");
			base.OnPause ();
			MyLogger.Debug (this.LocalClassName, "Lifecycle Exited: OnPause");
		}

		private void InitializeComponents ()
		{
			notificationList = this.FindViewById<LinearLayout> (Resource.Id.notificationIDLayout);
			dateList = this.FindViewById<LinearLayout> (Resource.Id.dateLayout);
		}

		private void OnEventItemClicked (TowEvent towEvent)
		{
			MyLogger.Information (this.LocalClassName, string.Format ("Notificiation Clicked: {0}", towEvent.Id));
			var towDetailsActivity = new Intent (this, typeof(TowDetailsActivity));
			towDetailsActivity.PutExtra ("selectedEventId", towEvent.Id.ToString ());
			StartActivity (towDetailsActivity);  
		}
		//TODO Instead of separate textview for id and date, combine them
		private void RefreshNotificationList ()
		{
			TextView eventID, eventDate;
			ClearNotificationList ();
			foreach (TowEvent eve in TowManager.GetAll ()) {
				eventID = new TextView (this);
				eventID.Text = (eve.Id.ToString ());
				eventID.Clickable = true;
				eventID.Click += (sender, e) => OnEventItemClicked (eve);
				notificationList.AddView (eventID);
				eventDate = new TextView (this);
				eventDate.Text = eve.Time.ToString ("f");
				eventDate.Clickable = true;
				eventDate.Click += (sender, e) => OnEventItemClicked (eve);
				dateList.AddView (eventDate);
			}
		}

		private void ClearNotificationList ()
		{
			notificationList.RemoveAllViews ();
			dateList.RemoveAllViews ();
		}
		//TODO update just the new event
		public void Update ()
		{//reset newnotificaiton number
			MyLogger.Information (this.LocalClassName, "Notification List updated.");
			RefreshNotificationList ();
		}
	}
}

