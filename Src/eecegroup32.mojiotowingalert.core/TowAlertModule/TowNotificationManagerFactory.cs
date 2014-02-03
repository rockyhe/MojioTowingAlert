using System;
using Mojio.Events;

namespace eecegroup32.mojiotowingalert.core
{
	public class TowNotificationManagerFactory
	{
		private static TowNotificationManagerFactory factory = null;
		private static Object padLock = new Object ();

		private TowNotificationManagerFactory ()
		{
		}

		public static TowNotificationManagerFactory GetFactory ()
		{
			if (factory == null) {
				lock (padLock) {
					if (factory == null) {
						factory = CreateFactory ();
					}
				}
			}
			
			return factory;
		}

		private static TowNotificationManagerFactory CreateFactory ()
		{
			return new TowNotificationManagerFactory ();
		}

		public AbstractNotificationManager Create (EventType type)
		{
			switch (type) {
			case EventType.Tow:
				return new TowNotificationManager ();
			default:
				return null;
			}
			
		}
	}
}

