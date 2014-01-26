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

		protected override void OnCreate (Bundle bundle)
		{
			logger.Debug (this.LocalClassName, "Lifecycle Entered: OnCreate");

			base.OnCreate (bundle);
			SetContentView (Resource.Layout.Settings);
			InitiateContentView();
			LoadDongleList ();

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
			logger.Debug (this.LocalClassName, "Lifecycle Exited: OnResume");
		}

		protected override void OnPause()
		{
			logger.Debug (this.LocalClassName, "Lifecycle Entered: OnPause");
			base.OnPause();
			logger.Debug (this.LocalClassName, "Lifecycle Exited: OnPause");
		}

		private void OnNotificationToggleClicked(object sender, EventArgs e) 
		{
			SaveNotificationSetting (NotificationTogglePref, notificationToggle.Checked);
		}

		private void OnSoundCheckBoxClicked(object sender, EventArgs e) 
		{
			SaveNotificationSetting (NotificationSoundPref, soundCheckBox.Checked);
		}

		private void OnVibrationCheckBoxClicked(object sender, EventArgs e) 
		{
			SaveNotificationSetting (NotificationVibrationPref, vibrationCheckBox.Checked);
		}

		private void SaveNotificationSetting(String option, bool value)
		{
			var preferences = GetSharedPreferences(SharedPreferencesName, FileCreationMode.Private); 
			var edits = preferences.Edit();
			edits.PutString(option, value.ToString());
			edits.Commit();
			logger.Information (this.LocalClassName, string.Format("Settings: {0} [{1}] saved.", SharedPreferencesName, value)); 
		}

		//TODO Implement toggling individual device
		private void ToggleSubscribeDongle(string id)
		{
		}

		private void InitializeComponents()
		{
			notificationToggle = FindViewById<ToggleButton>(Resource.Id.NotificationToggleButton);
			soundCheckBox = FindViewById<CheckBox>(Resource.Id.SoundCheckBox);
			vibrationCheckBox = FindViewById<CheckBox>(Resource.Id.VibrationCheckBox);
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
					ToggleSubscribeDongle (moj.Id);
				};
				dongleListLayout.AddView (item);
				dongleButtonLayout.AddView (button);
				i++;
			}
			logger.Information (this.LocalClassName, string.Format("{0} dongle(s) loaded.", i));
		}
	}
}

