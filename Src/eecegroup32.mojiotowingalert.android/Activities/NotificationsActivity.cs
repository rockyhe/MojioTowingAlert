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
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			SetContentView (Resource.Layout.Notifications);
			// Create your application here
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
	}
}

