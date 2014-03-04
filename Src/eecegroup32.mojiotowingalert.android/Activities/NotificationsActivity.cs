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
			this.ActionBar.SetBackgroundDrawable (Resources.GetDrawable (Resource.Drawable.Black));
			this.ActionBar.SetTitle (Resource.String.notifications);
			notificationList = this.FindViewById<LinearLayout> (Resource.Id.notificationList);
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
			View eventView;
			ClearNotificationList ();
			TowManager.ClearNewEventNumber ();

			foreach (TowEvent eve in TowManager.GetAll ()) {
				eventView = MainApp.GetCurrentActivity ().LayoutInflater.Inflate (Resource.Layout.NotificationView, null);
				eventView.FindViewById<TextView> (Resource.Id.Text1).Text = eve.Time.ToString ("f");
				eventView.FindViewById<TextView> (Resource.Id.Text2).Text = "Event ID: " + eve.Id.ToString ();
				eventView.Clickable = true;
				eventView.Click += (sender, e) => OnEventItemClicked (eve);
				notificationList.AddView (eventView);
			}
		}

		private void ClearNotificationList ()
		{
			notificationList.RemoveAllViews ();
		}
		//TODO [GROUP32] update just the new event
		public void Update ()
		{
			MyLogger.Information (this.LocalClassName, "Notification List updated.");
			RefreshNotificationList ();			
		}
	}
}

