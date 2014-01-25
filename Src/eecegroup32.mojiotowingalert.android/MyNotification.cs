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
		private string _NotificationID { get; set; }
		private Event _MojioEvent { get; set; }

		public MyNotification (Event eve, System.Guid guid)
		{
            _NotificationID = guid.ToString();
			_MojioEvent = eve;
		}

        public MyNotification(Event eve)
        {
            _NotificationID = System.Guid.NewGuid().ToString();
			_MojioEvent = eve;
        }

        public MyNotification()
        {
            _NotificationID = null;
			_MojioEvent = null;
        }

        public string NotificationID
        {
			get 
			{
				return _NotificationID;
			}
        }
		        
        public Event MojioEvent
        {
			get
			{
				return _MojioEvent;
			}
        }
	
	}
}
