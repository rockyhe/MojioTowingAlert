using System;
using System.Net;
using System.Linq;
using System.Collections.Generic;
using Android.OS;
using Android.App;
using Android.Widget;
using Android.Views;
using Android.Content;
using Mojio;
using Mojio.Events;
using PushSharp.Client;
using eecegroup32.mojiotowingalert.core;

namespace eecegroup32.mojiotowingalert.android
{
	public abstract class EventBaseActivity : BaseActivity
	{
		protected static IntentFilter IntFilter{ get; set; }

		protected static PushEventReceiver Receiver{ get; set; }

		public class PushEventReceiver : EventReceiver
		{
			protected override void OnEvent (Context context, Event ev)
			{
				MyLogger.Information (this.Class.SimpleName, string.Format ("Event Received: Context-{0} EventType-{1}", context.GetType ().ToString (), ev.EventType.ToString ()));

				if (context is EventBaseActivity)
					(context as EventBaseActivity).OnMojioEventReceived (ev);
			}
		}

		protected override void OnCreate (Bundle bundle)
		{
			MyLogger.Debug (this.LocalClassName, "Lifecycle Entered: OnCreate");

			base.OnCreate (bundle);
			InitializeVariables ();
			LoadUserPreference ();
			LoadMojioDevices (); 
			RegisterReceiver (Receiver, IntFilter);
			RegisterSubscriptionEventListener ();
			MyLogger.Debug (this.LocalClassName, "Lifecycle Exited: OnCreate");
		}

		protected void RegisterSubscriptionEventListener ()
		{
			SettingsActivity.OnSubscriptionChanged += RegisterEvenForNotice;
		}

		private void LoadUserPreference ()
		{
			MyLogger.Information (this.LocalClassName, string.Format ("Loading User Preference for {0}.", Client.CurrentUser.UserName));
			var r = MyDataManager.GetUserPreference (Client.CurrentUser.UserName);
			if (r != null) {
				CurrentUserPreference = r;
				MyLogger.Information (this.LocalClassName, string.Format ("User Preference Retrieved for {0}.", Client.CurrentUser.UserName));
			} else {
				CurrentUserPreference = new UserPreference () { UserId = Client.CurrentUser.UserName };
				MyLogger.Information (this.LocalClassName, string.Format ("Default User Preference Created for {0}.", Client.CurrentUser.UserName));
			}
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
			try {
				UnregisterReceiver (Receiver);
			} catch (Exception ex) {
				MyLogger.Error (this.LocalClassName, string.Format ("Tried to unregister when not registered. Exception: {0}", ex.Message));
			}
			base.OnDestroy ();
			MyLogger.Debug (this.LocalClassName, "Lifecycle Exited: OnDestroy");
		}

		protected override void OnResume ()
		{
			MyLogger.Debug (this.LocalClassName, "Lifecycle Entered: OnResume");
			base.OnResume ();
			RegisterEventsNotice ();
			MyLogger.Debug (this.LocalClassName, "Lifecycle Exited: OnResume");
		}

		protected override void OnPause ()
		{
			MyLogger.Debug (this.LocalClassName, "Lifecycle Entered: OnPause");
			base.OnPause ();
			MyLogger.Debug (this.LocalClassName, "Lifecycle Exited: OnPause");
		}

		private void InitializeVariables ()
		{
			if (Receiver == null)
				Receiver = new PushEventReceiver ();

			if (IntFilter == null)
				IntFilter = new IntentFilter (EventReceiver.IntentAction);
		}

		protected Subscription SubscribeForEvent (string registrationId, out HttpStatusCode httpStatusCode, out string msg, Device mojioDevice, EventType eventToSubscribe)
		{
			return Client.SubscribeGcm (registrationId, new Subscription () {
				Event = eventToSubscribe,
				EntityId = mojioDevice.Id,
				EntityType = SubscriptionType.Mojio,
			}, out httpStatusCode, out msg);
		}

		protected bool CheckSubscriptionStatus (HttpStatusCode httpStatusCode)
		{
			return httpStatusCode == HttpStatusCode.NotModified;
		}

		protected bool UnsubscribeForEvent (Device device)
		{
			//Client.Unsubscribe((()))
			MyLogger.Information (this.LocalClassName, string.Format ("Subscription (TO BE Implemented): {0} unsubscribed for event type {1}", device.Id, "Event"));
			return true;
		}

		protected bool IsNullOrEmpty (IEnumerable<Object> collection)
		{
			return collection == null || collection.Count () == 0;
		}

		protected bool IsNullOrEmpty (IEnumerable<EventType> collection)
		{
			return collection == null || collection.Count () == 0;
		}

		/// <summary>
		/// Registers all the events notices for all the devices IAW the current user preference.
		/// </summary>
		protected void RegisterEventsNotice ()
		{
			if (IsNullOrEmpty (UserDevices)) {
				MyLogger.Error (this.LocalClassName, string.Format ("Event Notice Registration: Incomplete. UserDevice null or 0."));
				return;
			}
				
			if (IsNullOrEmpty (EventsToSubscribe)) {
				MyLogger.Error (this.LocalClassName, string.Format ("Event Notice Registration: Incomplete. Events to Subscribe null or 0."));
				return;
			}

			if (string.IsNullOrEmpty (RegistrationId))
				RegistrationId = PushClient.GetRegistrationId (this.ApplicationContext);
				
			MyLogger.Information (this.LocalClassName, string.Format ("Event Notice Registration: ID {0} Retrieved.", RegistrationId));
			if (String.IsNullOrWhiteSpace (RegistrationId)) {
				MyLogger.Error (this.LocalClassName, "Event Notice Registration: Failed - No Registration ID Retrieved.");
				return;
			}
			
			foreach (var eventToSubscribe in EventsToSubscribe) {
				var subscribedDevices = CurrentUserPreference.GetAllSubscribedDevices (eventToSubscribe);
				if (subscribedDevices == null || subscribedDevices.Count () == 0)
					continue;
				foreach (var userDevice in UserDevices) {					
					if (subscribedDevices.Contains (userDevice.Id))
						RegisterEvenForNotice (userDevice.Id, true);
					else
						RegisterEvenForNotice (userDevice.Id, false);					
				}            				
			}
		}

		protected virtual void ShowToastAtCenter (string msg)
		{
			Toast toast = Toast.MakeText (this, msg, ToastLength.Short);
			toast.SetGravity (GravityFlags.CenterVertical, 0, 0);
			toast.Show ();
		}

		protected void RegisterEvenForNotice (string deviceId, bool toSubscribe)
		{
			if (string.IsNullOrEmpty (RegistrationId))
				RegistrationId = PushClient.GetRegistrationId (this.ApplicationContext);
			var trials = 3; 
			var device = UserDevices.First (x => x.Id == deviceId);
			if (device == null) {
				MyLogger.Error (this.LocalClassName, string.Format ("Device {0} not found. Subscription canceled.", deviceId));
				return;
			}
			do {
				if (!toSubscribe) {
					if (UnsubscribeForEvent (device))
						return;	
				} else {
					if (SubscribeForEvent (device, RegistrationId))
						return;
				}
				trials--;
			} while (trials > 0);
			MyLogger.Error (this.LocalClassName, string.Format ("{0} Subscription/Unsubscription failed.", deviceId));
		}

		protected bool SubscribeForEvent (Device mojioDevice, string registrationId)
		{
			HttpStatusCode statusCode;
			string msg;
			bool succeed = true;
			foreach (var eventToSubscribe in EventsToSubscribe) {
				Subscription subscription = SubscribeForEvent (registrationId, out statusCode, out msg, mojioDevice, eventToSubscribe);
				if (subscription != null) {
					MyLogger.Information (this.LocalClassName, string.Format ("Event Subscription: {0} - {1}.", "successful", msg));	                    
					continue;
				}
				if (CheckSubscriptionStatus (statusCode)) {
					MyLogger.Information (this.LocalClassName, "Event Subscription: Event already subscribed.");	
					continue;
				}           
				succeed = false;         
			}
			return succeed;
		}

		protected virtual void OnMojioEventReceived (Event eve)
		{
			switch (eve.EventType) {
			case EventType.Tow:
				TowManager.Add (eve);
				if (ActivityVisible)
					NotifyViaToast ("Your Car Is Being Towed!");
				else
					NotifyViaLocalNotification ("MOJIO: Your Car Is Being Towed!");
				break;
			default:
				if (ActivityVisible)
					NotifyViaToast ();
				else
					NotifyViaLocalNotification ();
				break;
			}
			
			if (MainApp.GetCurrentActivity () is NotificationsActivity) {
				((NotificationsActivity)MainApp.GetCurrentActivity ()).Update ();
			}

		}
	}
}