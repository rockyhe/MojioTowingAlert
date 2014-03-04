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
using Android.Graphics;
using Mojio;

namespace eecegroup32.mojiotowingalert.android
{
	[Activity (Label = "NotificationsActivity")]			
	public class NotificationsActivity : BaseActivity
	{
		private LinearLayout notificationList;
		private Button refreshButton;
		private Button notificationFilterButton;
		private HashSet<Device> devicesToShow;

		protected override void OnCreate (Bundle bundle)
		{
			MyLogger.Debug (this.LocalClassName, "Lifecycle Entered: OnCreate");
			base.OnCreate (bundle);
			SetContentView (Resource.Layout.Notifications);
			devicesToShow = new HashSet<Device> ();
			foreach (var dev in UserDevices)
				devicesToShow.Add (dev);
			InitializeComponents ();
			InitializeEventHandlers ();			
			MyLogger.Debug (this.LocalClassName, "Lifecycle Exited: OnCreate");
		}

		protected override void OnResume ()
		{
			MyLogger.Debug (this.LocalClassName, "Lifecycle Entered: OnResume");
			base.OnResume ();			
			MainApp.SetCurrentActivity (this);			
			Update ();
			MyLogger.Debug (this.LocalClassName, "Lifecycle Exited: OnResume");
		}

		private void InitializeEventHandlers ()
		{
			refreshButton.Click += OnRefreshClicked;
			notificationFilterButton.Click += OnFilterButtonClicked;
		}

		private async void OnRefreshClicked (object sender, EventArgs e)
		{
			await Task.Factory.StartNew (() => LoadLastEvents (EventsToSubscribe));
			RefreshNotificationList ();
			NotifyViaToast ("Notification List Refreshed.");
		}

		private void InitializeComponents ()
		{
			this.ActionBar.SetBackgroundDrawable (Resources.GetDrawable (Resource.Drawable.Black));
			this.ActionBar.SetTitle (Resource.String.notifications);
			notificationList = this.FindViewById<LinearLayout> (Resource.Id.notificationList);
			refreshButton = this.FindViewById<Button> (Resource.Id.refreshNotification);
			notificationFilterButton = this.FindViewById<Button> (Resource.Id.notificationFilterButton);
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

			var events = TowManager.GetAll ();
			List<Event> filteredEvents = new List<Event> ();
			foreach (var dev in devicesToShow) {
				var temp = events.Where (x => x.MojioId.Equals (dev.Id));
				filteredEvents.AddRange (temp);
			}
			foreach (TowEvent eve in filteredEvents) {
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

		private void OnFilterButtonClicked (object sender, EventArgs e)
		{
			Dialog dialog = CreateFilterDialog ();
			AddDevicesToFilterDialog (dialog);			
			dialog.Show ();
		}

		private Dialog CreateFilterDialog ()
		{
			Dialog dialog = new Dialog (this);
			dialog.SetTitle ("Select Dongles To Show");
			dialog.Window.SetLayout (LinearLayout.LayoutParams.WrapContent, LinearLayout.LayoutParams.WrapContent);
			dialog.SetContentView (Resource.Layout.SelectDongles);
			dialog.Window.SetTitleColor (Color.LightYellow);
			return dialog;
		}

		private void AddDevicesToFilterDialog (Dialog dialog)
		{
			var layout = dialog.FindViewById<LinearLayout> (Resource.Id.SelectDeviceLayout);		
			foreach (Device dev in UserDevices) {
				ToggleButton listItem = CreateDeviceSelectionItem (dev);
				layout.AddView (listItem);
			}
		}

		private ToggleButton CreateDeviceSelectionItem (Device moj)
		{
			ToggleButton button = new ToggleButton (this);
			
			if (devicesToShow.Contains (moj))
				button.Checked = true;
			else
				button.Checked = false;
			
			button.Text = string.Format ("Name:{0} \nId:{1}", moj.Name, moj.IdToString);
			button.Tag = moj.Id;
			button.Click += OnDeviceSelected;
			return button;
		}

		private void OnDeviceSelected (object sender, EventArgs e)
		{
			var selectedButton = (ToggleButton)sender;
			var selectedDevice = UserDevices.FirstOrDefault (x => x.Id.Equals ((string)selectedButton.Tag));
			selectedButton.Text = string.Format ("Name:{0} \nId:{1}", selectedDevice.Name, selectedDevice.IdToString);
			if (selectedButton.Checked)
				devicesToShow.Add (selectedDevice);
			else
				devicesToShow.Remove (selectedDevice);
			
			
			RefreshNotificationList ();
		}
	}
}

