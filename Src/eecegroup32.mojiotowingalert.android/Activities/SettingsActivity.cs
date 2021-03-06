using System;
using System.Net;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using Android.OS;
using Android.App;
using Android.Views;
using Android.Widget;
using Android.Runtime;
using Android.Content;
using PushSharp.Client;
using Mojio;
using Mojio.Events;
using eecegroup32.mojiotowingalert.core;

namespace eecegroup32.mojiotowingalert.android
{
	public delegate void ChangedEventHandler (Device device, EventType eventType, bool isChecked);
	[Activity (Label = "SettingsActivity")]			
	public class SettingsActivity : BaseActivity
	{
		public static event ChangedEventHandler OnSubscriptionChanged;

		private ToggleButton notificationToggle;
		private CheckBox soundCheckBox;
		private CheckBox vibrationCheckBox;
		private LinearLayout dongleListLayout;
		private LinearLayout dongleButtonLayout;

		protected override void OnCreate (Bundle bundle)
		{
			MyLogger.Debug (this.LocalClassName, "Lifecycle Entered: OnCreate");

			base.OnCreate (bundle);
			SetContentView (Resource.Layout.Settings);
			InitiateContentView ();
			LoadDongleList ();
			
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

		protected override void OnPause ()
		{
			MyLogger.Debug (this.LocalClassName, "Lifecycle Entered: OnPause");
			base.OnPause ();
			MyLogger.Debug (this.LocalClassName, "Lifecycle Exited: OnPause");
		}

		private void OnNotificationToggleClicked (object sender, EventArgs e)
		{
			CurrentUserPreference.NotificationChecked = notificationToggle.Checked;
			MyLogger.Information (this.LocalClassName, string.Format ("User Preference: {0} Set to {1}", "Notification Toggle", notificationToggle.Checked));
			SaveUserPreferences ();
		}

		private void OnSoundCheckBoxClicked (object sender, EventArgs e)
		{
			CurrentUserPreference.SoundChecked = soundCheckBox.Checked;
			MyLogger.Information (this.LocalClassName, string.Format ("User Preference: {0} Set to {1}", "Sound Toggle", soundCheckBox.Checked));
			SaveUserPreferences ();
		}

		private void OnVibrationCheckBoxClicked (object sender, EventArgs e)
		{
			CurrentUserPreference.VibrationChecked = vibrationCheckBox.Checked;	
			MyLogger.Information (this.LocalClassName, string.Format ("User Preference: {0} Set to {1}", "Vibration Toggle", vibrationCheckBox.Checked));
			SaveUserPreferences ();
		}

		private void SaveUserPreferences ()
		{	
			var succeed = MyDataManager.SaveUserPreference (CurrentUserPreference);
			var msg = string.Format ("User Preference: {0} ", succeed ? "Saved" : "Not Saved");
			MyLogger.Information (this.LocalClassName, msg);
			ShowToastAtCenter (msg);
		}

		private void ShowToastAtCenter (string msg)
		{
			var toast = Toast.MakeText (this, msg, ToastLength.Short);
			toast.SetGravity (GravityFlags.Center, 0, 0);
			toast.Show ();
		}

		private void OnDeviceSubscriptionToggleClicked (Device dev, EventType eventType, bool isChecked)
		{
			MyLogger.Information (this.LocalClassName, string.Format ("User Preference: {0} for {1} Set to {2}", eventType, dev, isChecked));
			if (isChecked)
				CurrentUserPreference.AddToSubscriptionList (EventType.TowStart, dev);
			else
				CurrentUserPreference.RemoveFromSubscriptionList (EventType.TowStart, dev);
			OnSubscriptionChanged (dev, eventType, isChecked);
			SaveUserPreferences ();
		}

		private void InitializeComponents ()
		{
			this.ActionBar.SetBackgroundDrawable (Resources.GetDrawable (Resource.Drawable.Black));
			this.ActionBar.SetTitle (Resource.String.settings);
			notificationToggle = FindViewById<ToggleButton> (Resource.Id.NotificationToggleButton);
			notificationToggle.SetBackgroundColor (Android.Graphics.Color.Rgb (1, 187, 225));
			notificationToggle.Checked = GetNotificationTogglePref ();
			soundCheckBox = FindViewById<CheckBox> (Resource.Id.SoundCheckBox);
			soundCheckBox.Checked = GetNotificationSoundPref ();
			vibrationCheckBox = FindViewById<CheckBox> (Resource.Id.VibrationCheckBox);
			vibrationCheckBox.Checked = GetNotificationVibrationPref ();
			dongleListLayout = FindViewById<LinearLayout> (Resource.Id.dongleListLayout);
			dongleButtonLayout = FindViewById<LinearLayout> (Resource.Id.dongleSubButtonLayout);
		}

		private void InitializeEventHandlers ()
		{
			notificationToggle.Click += new EventHandler (OnNotificationToggleClicked);
			soundCheckBox.Click += new EventHandler (OnSoundCheckBoxClicked);
			vibrationCheckBox.Click += new EventHandler (OnVibrationCheckBoxClicked);
		}

		private void InitiateContentView ()
		{
			InitializeComponents ();
			InitializeEventHandlers ();
		}
		//TODO: [GROUP32] Cleanup Impelmentation
		private void LoadDongleList ()
		{
			MyLogger.Information (this.LocalClassName, "Dongle List: loading..."); 
			int i = 0;
			ToggleButton button;
			TextView item;
			LoadMojioDevices ();
			foreach (Device moj in UserDevices) {
				item = new TextView (this);
				item.SetTextColor (Android.Graphics.Color.Rgb (1, 187, 225));
				item.SetTextSize (Android.Util.ComplexUnitType.Px, 35);
				item.Id = i;
				item.Text = string.Format ("   {0} \n", moj.Name);
				MyLogger.Information (this.LocalClassName, string.Format ("Dongle List: {0} loaded.", moj.Name));
				RelativeLayout.LayoutParams parameters = 
					new RelativeLayout.LayoutParams (RelativeLayout.LayoutParams.FillParent,
						RelativeLayout.LayoutParams.WrapContent);
				parameters.AddRule (LayoutRules.AlignParentBottom);
				parameters.AddRule (LayoutRules.CenterVertical);
				if (i != 0)
					parameters.AddRule (LayoutRules.Above, i - 1);
				item.LayoutParameters = parameters;
				button = new ToggleButton (this);
				button.SetBackgroundColor (Android.Graphics.Color.Rgb (1, 187, 225));
				button.Id = i;
				button.LayoutParameters = parameters;
				button.Click += (o, args) => {
					//TODO: [GROUP 32] When we add more event types for our app change this line so that
					// depending on the button clicked, corresponding event type is sent to the handler
					OnDeviceSubscriptionToggleClicked (moj, EventType.TowStart, (o as ToggleButton).Checked);
				};
				button.Checked = CurrentUserPreference.GetSubscriptionStatus (EventType.TowStart, moj);
				LinearLayout.LayoutParams layoutParams = new LinearLayout.LayoutParams (
					                                         LinearLayout.LayoutParams.FillParent, LinearLayout.LayoutParams.WrapContent);
				layoutParams.SetMargins (5, 0, 5, 15);
				layoutParams.Height = 65;
				dongleListLayout.AddView (item);
				dongleButtonLayout.AddView (button, layoutParams);
				i++;
			}
			MyLogger.Information (this.LocalClassName, string.Format ("{0} dongle(s) loaded.", i));
		}
	}
}

