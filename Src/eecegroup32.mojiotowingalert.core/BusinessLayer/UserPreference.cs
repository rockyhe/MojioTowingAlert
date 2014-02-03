using System;
using System.Linq;
using System.Collections.Generic;
using Mojio;
using Mojio.Events;

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
		public string DevicesForTowEventString { 
			get { return string.Join (";", _devicesForTowEvent); } 
			set { _devicesForTowEvent = new HashSet<string> (value.Split (new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries)); }
		}

		private HashSet<string> _devicesForTowEvent;

		public UserPreference ()
		{
			_devicesForTowEvent = new HashSet<string> ();
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

		public void RemoveFromSubscriptionList (EventType eventType, string deviceId)
		{
			switch (eventType) {
			case EventType.Tow:
				if (_devicesForTowEvent.Contains (deviceId))
					_devicesForTowEvent.Remove (deviceId);
				break;
			default:
				break;
			}
		}

		public void AddToSubscriptionList (EventType eventType, string deviceId)
		{
			switch (eventType) {
			case EventType.Tow:
				_devicesForTowEvent.Add (deviceId);
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
					_devicesForTowEvent.Add (dev.Id);
				}
				break;
			default:
				break;
			}
		}

		public IEnumerable<string> GetAllSubscribedDevices (EventType eventType)
		{
			switch (eventType) {
			case EventType.Tow:
				return _devicesForTowEvent;
			default:
				return null;
			}
		}

		public bool GetSubscriptionStatus (EventType eventType, string deviceId)
		{
			switch (eventType) {
			case EventType.Tow:
				return _devicesForTowEvent.Contains (deviceId);
			default:
				return false;
			}
		}
	}
}

