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
		public static readonly string logTag = "MyNotificationManager";

		private List<MyNotification> notifications;
		private ILogger logger = MainApp.Logger;
		private int NumberOfNewNotifications;

		public MyNotificationManager()
		{
			notifications = new List<MyNotification>();
		}

		public MyNotification Create(Event incomingEvent)
		{
			return new MyNotification(incomingEvent);
		}

		public MyNotification Find (string notifId)
		{
			foreach (MyNotification notification in notifications)
			{
				if (notifId == notification.NotificationID)
				{
					return notification;
				}
			}
            return null;
		}

        public void Add(MyNotification incomingMyNotification)
        {
			var isDuplicate = notifications.Exists (x => x.NotificationID == incomingMyNotification.NotificationID);
			if (isDuplicate) 
			{
				logger.Error(logTag, string.Format("Duplicate notification found - {0}. Not added.", incomingMyNotification.NotificationID)); 
				return;
			}
            notifications.Add(incomingMyNotification);
			NumberOfNewNotifications++;
        }

		public void Clear()
		{
			notifications.Clear();
		}

        public List<MyNotification> GetAll()
        {
			notifications.Sort ((e1, e2) =>  e1.Date.CompareTo(e2.Date));
            return notifications;
        }

		public void SetNumberOfNewNotifications (int n)
		{
			NumberOfNewNotifications = n;
		}

		public void ClearNumberOfNewNotifications()
		{
			NumberOfNewNotifications = 0;
		}

		public int GetNumberOfNewNotifications()
		{
			return NumberOfNewNotifications;
		}

	}
}
