using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mojio.Events;
using Mojio.Client;

namespace eecegroup32.mojiotowingalert.core
{
	public abstract class AbstractNotificationManager: HashSet<Event>
	{
		protected int _NumberOfNewNotifications;

		public AbstractNotificationManager ()
		{
		}

		public int NewEventsCount { get; set; }

		public void IncrementNewEventNumber ()
		{
			_NumberOfNewNotifications++;
		}

		public void ClearNewEventNumber ()
		{
			_NumberOfNewNotifications = 0;
		}

		public int GetNewEventNumber ()
		{
			return _NumberOfNewNotifications;
		}

		public new virtual bool Add (Event e)
		{
			var suceed = base.Add (e);
			MyLogger.Information (this.GetType ().Name, string.Format ("{0}{1}Added", e.OwnerId, suceed ? " " : " Not "));
			return suceed;
		}

		public virtual Event Find (string id)
		{
			var result = this.First (x => x.OwnerId.ToString ().Equals (id));
			MyLogger.Information (this.GetType ().Name, string.Format ("{0}{1}Returned", id, result != null ? " " : " Null "));
			return result;
		}

		public virtual IEnumerable<IEvent> GetAll ()
		{
			var result = this.OrderByDescending (e => e.Time);
			MyLogger.Information (this.GetType ().Name, string.Format ("{0} Returned", result != null ? result.Count ().ToString () : " Null"));
			return result;
		}

		public virtual IEnumerable<IEvent> Get (int count)
		{
			var result = this.OrderByDescending (e => e.Time).Take (count);
			MyLogger.Information (this.GetType ().Name, string.Format ("{0} Returned", result != null ? result.Count ().ToString () : " Null"));
			return result;
		}
	}
}

