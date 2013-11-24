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
using Mojio;
using PushSharp.Client;
using System.Net;

namespace eecegroup32.mojiotowingalert.android
{
	[Activity (Label = "SettingsActivity")]			
	public class SettingsActivity : BaseActivity
	{

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			SetContentView (Resource.Layout.Settings);
			InitiateView();

			LoadDongles ();
		}

		private void OnNotificationToggleClicked() 
		{
			SaveNotificationSetting ();
		}

		private void OnSoundCheckBoxClicked() 
		{
			SaveNotificationSetting ();
		}

		private void OnVibrationCheckBoxClicked() 
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

		private void SubscribeDongle(int id)
		{
			// Fetch registration ID given to this app
			var registrationId = PushClient.GetRegistrationId(this.ApplicationContext);

			if (String.IsNullOrWhiteSpace (registrationId)) {
				return;
			}

			int trials = 3; 
			HttpStatusCode stat;
			string msg;
			Subscription sub;
			bool succeed = false;
			do
			{
				// Notify mojio servers what types of events we wish to receive.
				sub = Client.SubscribeGcm(registrationId, new Subscription()
					{
						Event = EventType.TripStart,			// We want to register to TripStart events
						EntityId = id,						// For this particular mojio device
						EntityType = SubscriptionType.Mojio,
					}, out stat, out msg);

				if (sub != null)
				{
					succeed = true;
					break;
				}
				if(stat == HttpStatusCode.NotModified)
				{
					// We were already registered to this event type.
					succeed = true;
					break;
				}

				trials--;
			}
			while (trials > 0);

			if (!succeed)
			{
				// Write the checkpoint to Test Flight.

				Toast tmp = Toast.MakeText(this, "Subscription failed, please check network status", ToastLength.Long);
				tmp.SetGravity(GravityFlags.CenterVertical, 0, 0);
				tmp.Show();
			}
		}

		private void InitiateView()
		{
			Button notificationToggle = FindViewById<Button>(Resource.Id.notificationToggleButton);
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
					SubscribeDongle (item.Id);
				};
				dongleListLayout.AddView (item);
				dongleButtonLayout.AddView (button);
				i++;
			}
		}


	}
}

