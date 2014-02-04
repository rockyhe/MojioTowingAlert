using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Android.OS;
using Android.App;
using Android.Views;
using Android.Widget;
using Android.Content;
using Android.Runtime;
using Mojio.Events;
using eecegroup32.mojiotowingalert.core;

namespace eecegroup32.mojiotowingalert.android
{
	[Activity (Label = "NotificationsActivity")]			
	public class NotificationsActivity : BaseActivity
	{
		private LinearLayout notificationList;
		private Button refreshButton;

		protected override void OnCreate (Bundle bundle)
		{
			MyLogger.Debug (this.LocalClassName, "Lifecycle Entered: OnCreate");
			base.OnCreate (bundle);
			SetContentView (Resource.Layout.Notifications);
			InitializeComponents ();
			InitializeEventHandlers ();
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
			MyLogger.Debug (this.LocalClassName, "Lifecycle Entered: OnRestart");
			base.OnRestart ();
			Update ();
			MyLogger.Debug (this.LocalClassName, "Lifecycle Entered: OnRestart");
		}

		protected override void OnPause ()
		{
			MyLogger.Debug (this.LocalClassName, "Lifecycle Entered: OnPause");
			base.OnPause ();
			MyLogger.Debug (this.LocalClassName, "Lifecycle Exited: OnPause");
		}

		private void InitializeEventHandlers ()
		{
			refreshButton.Click += OnRefreshClicked;
		}

		private void OnRefreshClicked (object sender, EventArgs e)
		{
			RefreshNotificationList ();
			NotifyViaToast ("Notification List Refreshed.");
		}

		private void InitializeComponents ()
		{
			notificationList = this.FindViewById<LinearLayout> (Resource.Id.notificationList);
			//dateList = this.FindViewById<LinearLayout> (Resource.Id.linearLayout5);
			refreshButton = this.FindViewById<Button> (Resource.Id.refreshNotification);
		}

		private void OnEventItemClicked (TowEvent towEvent)
		{
			MyLogger.Information (this.LocalClassName, string.Format ("Notificiation Clicked: {0}", towEvent.Id));
			var towDetailsActivity = new Intent (this, typeof(TowDetailsActivity));
			towDetailsActivity.PutExtra ("selectedEventId", towEvent.Id.ToString ());
			StartActivity (towDetailsActivity);  
		}
		//TODO [GROUP32] Instead of separate textview for id and date, combine them
		private void RefreshNotificationList ()
		{
			TextView eventID, eventDate;
			View eventView;
			ClearNotificationList ();
			TowManager.ClearNewEventNumber ();

			foreach (TowEvent eve in TowManager.GetAll ()) {
				eventID = new TextView (this);
				eventView = new View (this);
				eventView = MainApp.GetCurrentActivity ().LayoutInflater.Inflate (Resource.Layout.NotificationView, null);
				eventView.FindViewById<TextView>(Resource.Id.Text1).Text = eve.Time.ToString ("f");
				eventView.FindViewById<TextView> (Resource.Id.Text2).Text = "Event ID: " + eve.Id.ToString ();
				eventView.Clickable = true;
				eventView.Click += (sender, e) => OnEventItemClicked (eve);
				eventID.Text = ("Event Time: " + eve.Time.ToString ("f") + System.Environment.NewLine + "Event ID: " + eve.Id.ToString () + System.Environment.NewLine + "");
				eventID.Clickable = true;
				eventID.Click += (sender, e) => OnEventItemClicked (eve);
				//notificationList.AddView (eventID);
				notificationList.AddView (eventView);
				eventDate = new TextView (this);
				eventDate.Text = eve.Time.ToString ("f");
				eventDate.Clickable = true;
				eventDate.Click += (sender, e) => OnEventItemClicked (eve);
				//dateList.AddView (eventDate);
			}
		}

		private void ClearNotificationList ()
		{
			notificationList.RemoveAllViews ();
			//dateList.RemoveAllViews ();
		}
		//TODO [GROUP32] update just the new event
		public void Update ()
		{
			MyLogger.Information (this.LocalClassName, "Notification List updated.");
			RefreshNotificationList ();			
		}
	}
}

