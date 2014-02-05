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
using System.Threading.Tasks;

namespace eecegroup32.mojiotowingalert.android
{
	public abstract class EventBaseActivity : BaseActivity
	{
		protected static IntentFilter IntFilter{ get; set; }

		protected static PushEventReceiver Receiver{ get; set; }

		protected static IList<Subscription> Subscriptions { get; set; }

		public class PushEventReceiver : EventReceiver
		{
			protected override void OnEvent (Context context, Event ev)
			{
				MyLogger.Information (this.Class.SimpleName, string.Format ("Event Received: Mojio Id ({0}) Type ({1}) ", ev.MojioId, ev.EventType.ToString ()));

				if (context is EventBaseActivity)
					(context as EventBaseActivity).OnMojioEventReceived (ev);
				else
					MyLogger.Error (this.Class.SimpleName, string.Format ("Received Event Didn't Invoke OnMojioEventReceived ({0})", ev.ToString ()));
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
			RegisterEventsNotice ();		
			MyLogger.Debug (this.LocalClassName, "Lifecycle Exited: OnCreate");
		}

		/// <summary>
		/// Registers the subscription event listener.
		/// OnSubscriptionChanged is invoked when the 
		/// subscription preference for a dongle is changed.
		/// </summary>
		private void RegisterSubscriptionEventListener ()
		{
			SettingsActivity.OnSubscriptionChanged += RegisterEventForNotice;
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
				MyLogger.Information (this.LocalClassName, string.Format ("GCM Unregistered."));
			} catch (Exception ex) {
				MyLogger.Error (this.LocalClassName, string.Format ("Tried to unregister GCM when not registered. Exception: {0}", ex.Message));
			}
			base.OnDestroy ();
			MyLogger.Debug (this.LocalClassName, "Lifecycle Exited: OnDestroy");
		}

		protected override void OnResume ()
		{
			MyLogger.Debug (this.LocalClassName, "Lifecycle Entered: OnResume");
			base.OnResume ();
			//RegisterEventsNotice ();			
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
				
			if (Subscriptions == null)
				Subscriptions = new List<Subscription> ();
		}

		private Subscription SubscribeForEvent (string registrationId, out HttpStatusCode httpStatusCode, out string msg, Device mojioDevice, EventType eventToSubscribe)
		{
			return Client.SubscribeGcm (registrationId, new Subscription () {
				Event = eventToSubscribe,
				EntityId = mojioDevice.Id,
				EntityType = SubscriptionType.Mojio,
			}, out httpStatusCode, out msg);
		}
		//TODO: [GROUP 32] Just deleting subscriptions that aren't supposed to exist. Maybe check if subscriptions exist too?
		//Deleting subscriptions that are subscribed for events that we don't use for this app too.
		//Also very messy for now. Need major improvement.
		private void unsubscribeInvalidSubscription ()
		{
			MyLogger.Information (this.LocalClassName, "Checking if there are subscriptions that shouldn't exist...");
			var subs = from s in Client.Queryable<Subscription> ()
			           where s.ChannelType.Equals (ChannelType.Android) && s.ChannelTarget.Equals (RegistrationId)
			           select s;
			           
			if (IsNullOrEmpty (subs)) {
				MyLogger.Information (this.LocalClassName, "No subscription found.");
				return;
			}
			
			foreach (var sub in subs) {
				if (!EventsToSubscribe.Contains (sub.Event)) {
					Client.Delete (sub);
					MyLogger.Information (this.LocalClassName, string.Format ("Device {0} found subscribed to Event {1}. Deleted cuz our app doesn't support it.", sub.EntityId, sub.Event.ToString ()));
				}
			}
			MyLogger.Information (this.LocalClassName, string.Format ("{0} subscriptions found.", subs.Count ()));
			foreach (var sub in subs) {
				MyLogger.Information (this.LocalClassName, string.Format ("Subscription found: Device Id {0} | Owner Id {1} | Eventtype {2}", sub.EntityId, sub.OwnerId.ToString (), sub.Event.ToString ()));
			}
			foreach (var eventType in EventsToSubscribe) {
				var devices = CurrentUserPreference.GetAllSubscribedDevices (eventType);
				if (IsNullOrEmpty (devices)) {
					MyLogger.Information (this.LocalClassName, string.Format ("No Device Found for Eventtype {0}.", eventType.ToString ()));
					foreach (var sub in subs) {
						if (sub.OwnerId == Client.CurrentUser.Id && sub.Event == eventType) {
							Client.Delete (sub);
							MyLogger.Information (this.LocalClassName, string.Format ("Device {0} unsubscribed to Event {1} cuz the user has not set it.", sub.EntityId, sub.Event.ToString ()));
						}
					}
					continue;
				}
				foreach (var sub in subs) {
					if (sub.OwnerId == Client.CurrentUser.Id && devices.FirstOrDefault (x => x.Id == sub.EntityId) == null) {
						Client.Delete (sub);
						MyLogger.Information (this.LocalClassName, string.Format ("Device {0} found subscribed to Event {1} when it's not supposed to. Deleted.", sub.EntityId, sub.Event.ToString ()));
					}
				}
			}
			MyLogger.Information (this.LocalClassName, "Checking if there are subscriptions that shouldn't exist...Done");
		}

		private bool IsAlreadySubscribed (HttpStatusCode httpStatusCode)
		{
			return httpStatusCode == HttpStatusCode.NotModified;
		}

		protected bool UnsubscribeForEvent (Device device, EventType eventType)
		{
			Subscription subscription = Subscriptions.First (x => x.EntityId == device.Id);
			bool succeed = Client.Delete (subscription);
			MyLogger.Information (this.LocalClassName, string.Format ("Unsubscription: {0} for event type {1} - {2}", device.Id, eventType, succeed ? "Successful" : "Failed"));
			return succeed;
		}

		public static bool IsNullOrEmpty (IEnumerable<Object> collection)
		{
			return collection == null || (collection != null && collection.Count () == 0);
		}

		public static bool IsNullOrEmpty (IEnumerable<EventType> collection)
		{
			return collection == null || (collection != null && collection.Count () == 0);
		}

		/// <summary>
		/// Registers all the events notices for all the devices IAW the current user preference.
		/// </summary>
		protected async void RegisterEventsNotice ()
		{
			if (string.IsNullOrEmpty (RegistrationId))
				RegistrationId = PushClient.GetRegistrationId (this.ApplicationContext);
				
			await Task.Factory.StartNew (() => unsubscribeInvalidSubscription ());
			
			if (IsNullOrEmpty (UserDevices)) {
				MyLogger.Error (this.LocalClassName, string.Format ("Event Notice Registration: Incomplete. UserDevice null or 0."));
				return;
			}
				
			if (IsNullOrEmpty (EventsToSubscribe)) {
				MyLogger.Error (this.LocalClassName, string.Format ("Event Notice Registration: Incomplete. Events to Subscribe null or 0."));
				return;
			}
				
			MyLogger.Information (this.LocalClassName, string.Format ("Event Notice Registration: ID {0} Retrieved.", RegistrationId));
			if (String.IsNullOrWhiteSpace (RegistrationId)) {
				MyLogger.Error (this.LocalClassName, "Event Notice Registration: Failed - No Registration ID Retrieved.");
				return;
			}
			
			foreach (var eventToSubscribe in EventsToSubscribe) {
				var subscribedDevices = CurrentUserPreference.GetAllSubscribedDevices (eventToSubscribe);
				if (IsNullOrEmpty (subscribedDevices))
					continue;
				foreach (var userDevice in UserDevices) {					
					if (subscribedDevices.Contains (userDevice))
						RegisterEventForNotice (userDevice, eventToSubscribe, true);
					else
						RegisterEventForNotice (userDevice, eventToSubscribe, false);					
				}            				
			}
		}
		//TODO [GROUP 32] I know this is bad implementation. For now, given a device, it checks the user preference to see
		//which events need to be subscribed for the device. Should improve the implementation.
		protected void RegisterEventForNotice (Device dev, EventType eventType, bool toSubscribe)
		{
			if (string.IsNullOrEmpty (RegistrationId))
				RegistrationId = PushClient.GetRegistrationId (this.ApplicationContext);
			var trials = 3; 
			var device = UserDevices.First (x => x.Id == dev.Id);
			if (device == null) {
				MyLogger.Error (this.LocalClassName, string.Format ("Device {0} not found. Subscription canceled.", dev.Id));
				return;
			}
			do {
				if (!toSubscribe) {
					if (UnsubscribeForEvent (device, eventType))
						return;	
				} else {
					if (SubscribeForEvent (device, eventType))
						return;
				}
				trials--;
			} while (trials > 0);
			MyLogger.Error (this.LocalClassName, string.Format ("{0} Subscription/Unsubscription failed.", dev));
		}

		protected bool SubscribeForEvent (Device mojioDevice, EventType eventToSubscribe)
		{
			HttpStatusCode statusCode;
			string msg;
			bool succeed = true;
			Subscription subscription = SubscribeForEvent (RegistrationId, out statusCode, out msg, mojioDevice, eventToSubscribe);
			Subscriptions.Add (subscription);
			if (subscription != null)
				MyLogger.Information (this.LocalClassName, string.Format ("Event Subscription: {0} - {1}.", "Successful", msg));	                    
			if (IsAlreadySubscribed (statusCode))
				MyLogger.Information (this.LocalClassName, string.Format ("Event Subscription: {0} Event already subscribed.", eventToSubscribe.ToString ()));	
			if (subscription == null && !IsAlreadySubscribed (statusCode)) {
				succeed = false;         
				MyLogger.Error (this.LocalClassName, string.Format ("Event Subscription: {0} - {1}.", "Fail", msg));
			}
				                    
			
			return succeed;
		}

		/// <summary>
		/// This is all the action starts once a mojio event is received.
		/// </summary>
		/// <param name="eve">Mojio Event</param>
		protected virtual void OnMojioEventReceived (Event eve)
		{
			switch (eve.EventType) {
			case EventType.Tow:
				LoadMojioDevices ();
				var location = UserDevices.First (x => x.Id == eve.MojioId).LastLocation;				
				(eve as TowEvent).Location = location;
				MyLogger.Error (this.LocalClassName, string.Format ("Location ({0}) added to the event just received.", location.ToString ()));	
				TowManager.Add (eve);
				TowManager.IncrementNewEventNumber ();
				if (ActivityVisible)
					NotifyViaToast (Resources.GetString (Resource.String.towEventNotice));
				else
					NotifyViaLocalNotification (Resources.GetString (Resource.String.towEventNotice));
				break;
			default:
//				if (ActivityVisible)
//					NotifyViaToast ();
//				else
//					NotifyViaLocalNotification ();
				break;
			}
			
			if (MainApp.GetCurrentActivity () is NotificationsActivity) {
				((NotificationsActivity)MainApp.GetCurrentActivity ()).Update ();
				TowManager.ClearNewEventNumber ();
			}

		}
	}
}