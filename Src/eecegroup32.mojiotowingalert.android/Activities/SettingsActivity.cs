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
	public class SettingsActivity : EventBaseActivity
	{
		ToggleButton notificationToggle;
		CheckBox soundCheckBox;
		CheckBox vibrationCheckBox;
		LinearLayout dongleListLayout;
		LinearLayout dongleButtonLayout;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			SetContentView (Resource.Layout.Settings);
			InitiateContentView();
			LoadDongleList ();
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
		}

		protected override void OnStart()
		{
			base.OnStart();
			CurContext = this;
		}

		protected override void OnResume()
		{
			base.OnResume();
		}

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

		private IEnumerable<Device> GetMojioDevices()
		{
			return Client.UserMojios (Client.CurrentUser.Id).Data;
		}

		private void LoadDongleList()
		{
			int i = 0;
			foreach (Device moj in GetMojioDevices()) {
				TextView item = new TextView (this);
				item.Id = i;
				item.Text = string.Format ("Name:{0} \nId:{1}", moj.Name, moj.IdToString);
				ToggleButton button = new ToggleButton (this);
				button.Id = i;				                
				RelativeLayout.LayoutParams parameters = 
					new RelativeLayout.LayoutParams (RelativeLayout.LayoutParams.FillParent, 
						RelativeLayout.LayoutParams.WrapContent);
				parameters.AddRule (LayoutRules.AlignParentBottom);
				if (i != 0)
					parameters.AddRule (LayoutRules.Above, i - 1);
				item.LayoutParameters = parameters;
				button.LayoutParameters = parameters;
				button.Click += (o, args) => {
					ToggleSubscribeDongle (moj.Id);
				};
				dongleListLayout.AddView (item);
				dongleButtonLayout.AddView (button);
				i++;
			}
		}
	}
}

