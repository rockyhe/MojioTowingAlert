using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Mojio;
using Mojio.Client;
using Mojio.Events;

using Android.Gms.Maps;
using Android.Gms.Maps.Model;

namespace eecegroup32.mojiotowingalert.android
{
	public class MyNotification
	{
		private Event _MojioEvent;
		private LatLng _LatLng;
		private string _NotificationID;
		private string _DongleID;
		private string _Date;
		private string _Time;

        public MyNotification(Event eve)
        {   
			_MojioEvent = eve;         
			PopulateEventInfo (eve);
        }

		private void PopulateEventInfo(Event eve)
		{
			TripEvent e = (TripEvent) eve;
			_NotificationID = e.Id.ToString();
			_LatLng = new LatLng (e.Location.Lat, e.Location.Lng);
			_Date = e.Time.ToLongDateString();
			_Time = e.Time.ToLongTimeString();
			_DongleID = e.MojioId;
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

		public LatLng LatLng 
		{
			get 
			{
				return _LatLng;
			}
		}

		public string DongleID 
		{
			get 
			{
				return _DongleID;
			}
		}

		public string Date
		{
			get 
			{
				return _Date;
			}
		}

		public string Time 
		{
			get 
			{
				return _Time;
			}
		}
	
	}
}
