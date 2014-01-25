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
		LinearLayout notificationList;
		LinearLayout dateList;

		void InitializeComponents ()
		{
			notificationList = this.FindViewById<LinearLayout> (Resource.Id.notificationIDLayout);
			dateList = this.FindViewById<LinearLayout>(Resource.Id.dateLayout);
		}

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate(bundle);
			SetContentView(Resource.Layout.Notifications);
			InitializeComponents ();
		}

		protected override void OnResume()
		{
			base.OnResume();
			ShowNotifications();
		}

		private void AddNotificationsToScreen ()
		{
			TextView item;
			foreach (MyNotification notif in MyNotificationsMgr.getMyNotifications ()) {
				item = new TextView (this);
				item.Text = (notif.getmMyNotificationId ());
				notificationList.AddView (item);
			}
		}

		private void AddDatesToScreen ()
		{
			TextView item;
			foreach (MyNotification notif in MyNotificationsMgr.getMyNotifications ()) {
				item = new TextView (this);
				item.Text = notif.getEvent ().Time.ToString ("f");
				dateList.AddView (item);
			}
		}

		protected void ShowNotifications()
		{
			AddNotificationsToScreen ();
			AddDatesToScreen ();
		}
	}
}

