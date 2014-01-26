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

		private void AddNotificationsToScreen ()
		{
			TextView item;
			foreach (MyNotification notif in MyNotificationsMgr.GetAll ()) {
				item = new TextView (this);
				item.Text = (notif.NotificationID);
				notificationList.AddView (item);
			}

			foreach (MyNotification notif in MyNotificationsMgr.GetAll ()) {
				item = new TextView (this);
				item.Text = notif.MojioEvent.Time.ToString ("f");
				dateList.AddView (item);
			}
		}

		private void ClearNotificationList ()
		{
			notificationList.RemoveAllViews ();
			dateList.RemoveAllViews ();
		}

		private void ShowNotificationList()
		{
			ClearNotificationList ();
			AddNotificationsToScreen ();
		}
	}
}

