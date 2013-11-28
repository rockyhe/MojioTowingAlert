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

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			SetContentView (Resource.Layout.Settings);
			InitiateView();
			LoadDongles ();
		}

		private void OnNotificationToggleClicked(object sender, EventArgs e) 
		{
			SaveNotificationSetting ();
		}

		private void OnSoundCheckBoxClicked(object sender, EventArgs e) 
		{
			SaveNotificationSetting ();
		}

		private void OnVibrationCheckBoxClicked(object sender, EventArgs e) 
		{
			SaveNotificationSetting ();
		}

		private void SaveNotificationSetting()
		{
			var preferences = GetSharedPreferences(SharedPreferencesName, FileCreationMode.Private); 
			var edits = preferences.Edit();
			if (Notif != null)
				edits.PutString(NotificationPref, Notif.ToString());
			else if (preferences.Contains(NotificationPref))
				edits.Remove(NotificationPref);
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

		private void InitiateView()
		{
			Button notificationToggle = FindViewById<Button>(Resource.Id.NotificationToggleButton);
			notificationToggle.Click += new EventHandler(OnNotificationToggleClicked);
			CheckBox SoundCheckBox = FindViewById<CheckBox>(Resource.Id.SoundCheckBox);
			SoundCheckBox.Click += new EventHandler(OnSoundCheckBoxClicked);
			CheckBox VibrationCheckBox = FindViewById<CheckBox>(Resource.Id.VibrationCheckBox);
			VibrationCheckBox.Click += new EventHandler (OnVibrationCheckBoxClicked);
		}

		private void LoadDongles()
		{
			var dongleListLayout = this.FindViewById<LinearLayout> (Resource.Id.dongleListLayout);
			var dongleButtonLayout = this.FindViewById<LinearLayout> (Resource.Id.dongleSubButtonLayout);
			var res = Client.UserMojios (Client.CurrentUser.Id);
			int i = 0;
			foreach (Device moj in res.Data) {
				TextView item = new TextView (this);
				item.Id = i;
				ToggleButton button = new ToggleButton (this);
				button.Id = i;
				item.Text = string.Format ("Name:{0} \nId:{1}", moj.Name, moj.IdToString);                
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

