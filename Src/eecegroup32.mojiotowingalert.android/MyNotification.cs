using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Mojio.Client;
using Mojio.Events;

namespace eecegroup32.mojiotowingalert.android
{
    public class MyNotification
	{
		private string mMyNotificationId { get; set; }
		private Event mEvent { get; set; }

		public MyNotification (Event incomingEvent, System.Guid incomingGuid)
		{
            mMyNotificationId = incomingGuid.ToString();
            mEvent = incomingEvent;
		}

        public MyNotification(Event incomingEvent)
        {
            mMyNotificationId = System.Guid.NewGuid().ToString();
            mEvent = incomingEvent;
        }

        public MyNotification()
        {
            mMyNotificationId = null;
            mEvent = null;
        }

        public string getmMyNotificationId()
        {
            return mMyNotificationId;
        }
		        
        public Event getEvent()
        {
            return mEvent;
        }
	
	}
}
