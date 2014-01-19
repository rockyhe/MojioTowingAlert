using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Mojio.Client;
using Mojio.Events;

namespace eecegroup32.mojiotowingalert.android
{
    public class MyNotificationManager
	{
		private List<MyNotification> mMyNotifications = new List<MyNotification>();

		public MyNotification CreateMyNotification(Event incomingEvent)
		{
			return new MyNotification(incomingEvent);
		}

		public MyNotification GetMyNotificationWithId(string notifId)
		{
			foreach (MyNotification notification in mMyNotifications)
			{
				if (notifId == notification.getmMyNotificationId())
				{
					return notification;
				}
			}
            return null;
		}

        public void AddMyNotification(MyNotification incomingMyNotification)
        {
            mMyNotifications.Add(incomingMyNotification);
        }

		public void ClearMyNotifications()
		{
			mMyNotifications.Clear();
		}

        public List<MyNotification> getMyNotifications()
        {
            return mMyNotifications;
        }

	}
}
