using System;
using System.Linq;
using System.Collections.Generic;
using Mojio;
using Mojio.Events;
using Newtonsoft.Json;

namespace eecegroup32.mojiotowingalert.core
{
	[Table ("Preferences")]
	public class UserPreference
	{
		[PrimaryKey]
		public int Id { get { return ToId (UserId); } set { } }

		[Unique]
		public string UserId { get; set; }

		public bool NotificationChecked { get; set; }

		public bool SoundChecked { get; set; }

		public bool VibrationChecked { get; set; }

		/// <summary>
		/// Gets or sets the list of devices ids subscribed for towing event.
		/// Used for SQLite to make it easy to store _devicesForTowEvent.
		/// </summary>
		/// <value>a string of device ids deliminated with ";"</value>
		public string SubscriptionsJson { 
			get { return JsonConvert.SerializeObject (_subscriptions); } 
			set {
				_subscriptions = JsonConvert.DeserializeObject<List<List<Device>>> (value);
				//TODO [Group 32] Need to improve this cuz for now I have to remember the order 
				//I added all the list of devices to _subscriptions in order to reassign them
				if (_subscriptions [0] != null)
					_devicesForTowEvent = _subscriptions [0];
			}
		}

		private List<Device> _devicesForTowEvent;
		private List<List<Device>> _subscriptions;

		public UserPreference ()
		{			
			_subscriptions = new List<List<Device>> ();
			_subscriptions.Add (_devicesForTowEvent = new List<Device> ());
			UserId = "Default";
			NotificationChecked = true;
			SoundChecked = true;
			VibrationChecked = true;
		}

		/// <summary>
		/// UserPreference is stored in SQLite which requires an integer as the primary key.
		/// Converts a unique id (of some other type rather than int) to an int.
		/// </summary>
		/// <returns>A unique id of Integer type.</returns>
		/// <param name="userName">User ID.</param>
		public static int ToId (string userName)
		{
			return userName.GetHashCode ();
		}

		public void RemoveFromSubscriptionList (EventType eventType, Device device)
		{
			switch (eventType) {
			case EventType.Tow:
				if (_devicesForTowEvent.Contains (device))
					_devicesForTowEvent.Remove (device);
				break;
			default:
				break;
			}
		}

		public void AddToSubscriptionList (EventType eventType, Device device)
		{
			switch (eventType) {
			case EventType.Tow:
				if (_devicesForTowEvent.Contains (device))
					_devicesForTowEvent.Remove (device);
				_devicesForTowEvent.Add (device);
				break;
			default:
				break;
			}
		}

		public void AddAllToSubscriptionList (EventType eventType, IEnumerable<Device> devices)
		{
			switch (eventType) {
			case EventType.Tow:
				foreach (var dev in devices) {
					AddToSubscriptionList (eventType, dev);
				}
				break;
			default:
				break;
			}
		}

		public IEnumerable<Device> GetAllSubscribedDevices (EventType eventType)
		{
			switch (eventType) {
			case EventType.Tow:
				return _devicesForTowEvent;
			default:
				return null;
			}
		}

		public bool GetSubscriptionStatus (EventType eventType, Device device)
		{
			switch (eventType) {
			case EventType.Tow:
				return _devicesForTowEvent.Contains (device);
			default:
				return false;
			}
		}
	}
}

