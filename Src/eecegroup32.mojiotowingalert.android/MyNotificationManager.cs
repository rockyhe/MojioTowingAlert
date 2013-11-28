﻿using System;
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
		
		//Create notificaiton
		public MyNotification CreateMyNotification(Event incomingEvent)
		{
            MyNotification notification = new MyNotification(incomingEvent);
			return notification;
		}
		
        //Get notification with the input notification id
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

        //Add a new notification
        public void AddMyNotification(MyNotification incomingMyNotification)
        {
			if ((GetMyNotificationWithId(incomingMyNotification.getmMyNotificationId()) == null))
            	mMyNotifications.Add(incomingMyNotification);
        }
		
        //Clear notifications
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
