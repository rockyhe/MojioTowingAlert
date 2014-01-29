using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

using PushSharp.Client;
using Mojio;
using Mojio.Events;

namespace eecegroup32.mojiotowingalert.android
{
	[Activity (Label = "SettingsActivity")]			
	public class SettingsActivity : BaseActivity
	{
		private ToggleButton notificationToggle;
		private CheckBox soundCheckBox;
		private CheckBox vibrationCheckBox;
		private LinearLayout dongleListLayout;
		private LinearLayout dongleButtonLayout;
		private ISharedPreferencesEditor preferencesEdit;

		protected override void OnCreate (Bundle bundle)
		{
			logger.Debug (this.LocalClassName, "Lifecycle Entered: OnCreate");

			base.OnCreate (bundle);
			SetContentView (Resource.Layout.Settings);
			InitiateContentView();
			LoadDongleList ();
			OpenPreferenceEdit(); 

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
			ClosePreferenceEdit();
			base.OnDestroy();		
			logger.Debug (this.LocalClassName, "Lifecycle Exited: OnDestroy");
		}

		protected override void OnResume()
		{
			logger.Debug (this.LocalClassName, "Lifecycle Entered: OnResume");
			base.OnResume();
			MainApp.SetCurrentActivity (this);
			logger.Debug (this.LocalClassName, "Lifecycle Exited: OnResume");
		}

		protected override void OnPause()
		{
			logger.Debug (this.LocalClassName, "Lifecycle Entered: OnPause");
			base.OnPause();
			logger.Debug (this.LocalClassName, "Lifecycle Exited: OnPause");
		}

		private string GetSharedPreferencesName()
		{
			return string.Format ("{0}_{1}", SharedPreferencesName, Client.CurrentUser);
		}

		private void OpenPreferenceEdit()
		{
			preferencesEdit = GetSharedPreferences(GetSharedPreferencesName(), FileCreationMode.Private).Edit(); 
			logger.Information (this.LocalClassName, string.Format ("{0} opened. Settings ready to be edited.", SharedPreferencesName));
		}

		private void ClosePreferenceEdit()
		{
			preferencesEdit.Commit();
			logger.Information (this.LocalClassName, string.Format ("{0} closed. All changes saved.", SharedPreferencesName));
		}

		private void OnNotificationToggleClicked(object sender, EventArgs e) 
		{
			EditNotificationSetting (NotificationTogglePref, notificationToggle.Checked);
		}

		private void OnSoundCheckBoxClicked(object sender, EventArgs e) 
		{
			EditNotificationSetting (NotificationSoundPref, soundCheckBox.Checked);
		}

		private void OnVibrationCheckBoxClicked(object sender, EventArgs e) 
		{
			EditNotificationSetting (NotificationVibrationPref, vibrationCheckBox.Checked);
		}

		private void EditNotificationSetting(String option, bool value)
		{
			preferencesEdit.PutString(option, value.ToString());
			logger.Information (this.LocalClassName, string.Format("Settings: {0} edited to {1}.", option, value)); 
		}

		private void OnDeviceSubscriptionToggleClicked(string id, bool toggleStatus) 
		{
			EditNotificationSetting (GetDeviceSubscriptionPrefKey(id), toggleStatus);
		}
			
		private void InitializeComponents()
		{
			notificationToggle = FindViewById<ToggleButton>(Resource.Id.NotificationToggleButton);
			notificationToggle.Checked = GetNotificationTogglePref ();
			soundCheckBox = FindViewById<CheckBox>(Resource.Id.SoundCheckBox);
			soundCheckBox.Checked = GetNotificationSoundPref ();
			vibrationCheckBox = FindViewById<CheckBox>(Resource.Id.VibrationCheckBox);
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

		private void InitiateContentView()
		{
			InitializeComponents ();
			InitializeEventHandlers ();
		}

		//TODO maybe async load?
		private void LoadDongleList()
		{
			logger.Information (this.LocalClassName, "Dongle List: loading..."); 
			int i = 0;
			ToggleButton button;
			TextView item;
			LoadMojioDevices ();
			foreach (Device moj in MojioDevices) {
				item = new TextView (this);
				item.Id = i;
				item.Text = string.Format ("Name:{0} \nId:{1}", moj.Name, moj.IdToString);
				logger.Information (this.LocalClassName, string.Format ("Dongle List: {0} loaded.", moj.Name));
				RelativeLayout.LayoutParams parameters = 
					new RelativeLayout.LayoutParams (RelativeLayout.LayoutParams.FillParent, 
						RelativeLayout.LayoutParams.WrapContent);
				parameters.AddRule (LayoutRules.AlignParentBottom);
				if (i != 0)
					parameters.AddRule (LayoutRules.Above, i - 1);
				item.LayoutParameters = parameters;
				button = new ToggleButton (this);
				button.Id = i;				                
				button.LayoutParameters = parameters;
				button.Click += (o, args) => {
					OnDeviceSubscriptionToggleClicked (moj.Id, button.Checked);
				};
				button.Checked = GetDeviceSubscriptionPref (moj.Id);
				dongleListLayout.AddView (item);
				dongleButtonLayout.AddView (button);
				i++;
			}
			logger.Information (this.LocalClassName, string.Format("{0} dongle(s) loaded.", i));
		}
	}
}

