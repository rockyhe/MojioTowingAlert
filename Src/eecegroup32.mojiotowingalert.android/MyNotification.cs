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
		
		//Constructor
		public MyNotification (Event incomingEvent, System.Guid incomingGuid)
		{
            mMyNotificationId = incomingGuid.ToString();
            mEvent = incomingEvent;
		}

        //Constructor
        public MyNotification(Event incomingEvent)
        {
            mMyNotificationId = System.Guid.NewGuid().ToString();
            mEvent = incomingEvent;
        }

        //Constructor
        public MyNotification()
        {
            mMyNotificationId = null;
            mEvent = null;
        }

        //Returns mMyNotificationId
        public string getmMyNotificationId()
        {
            return mMyNotificationId;
        }

        //Returns event
        public Event getEvent()
        {
            return mEvent;
        }
	
	}
}
