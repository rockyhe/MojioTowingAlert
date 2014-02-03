using System;
using Mojio.Events;

namespace eecegroup32.mojiotowingalert.core
{
	public class TowNotificationManager: AbstractNotificationManager
	{
		public TowNotificationManager ()
		{
		}
		//TODO: Remove this when Mojio add Location to its Tow events
		public override bool Add (Event e)
		{
			((TowEvent)e).Location = new Mojio.Location () {
				Lat = 49.2839f,
				Lng = -123.1201f
			};			
			return base.Add (e);
		}
	}
}

