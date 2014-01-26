using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;

using Mojio.Events;
using Mojio;

namespace eecegroup32.mojiotowingalert.android
{
	[Activity (Label = "DetailsActivity")]			
	public class DetailsActivity : BaseActivity
	{
		private MyNotification notification;
		private TextView eventDateText;
		private TextView eventTimeText;
		private TextView eventLocationText;
		private TextView eventDongleIDText;

		protected override void OnCreate (Bundle bundle)
		{
			logger.Debug (this.LocalClassName, "Lifecycle Entered: OnCreate");

			base.OnCreate (bundle);
			SetContentView (Resource.Layout.NotificationDetail);
			InitializeComponents ();
			SetMojioEventInfo ();
			SetupMaps();

			logger.Debug (this.LocalClassName, "Lifecycle Exited: OnCreate");
		}

		protected override void OnStart()
		{
			logger.Debug (this.LocalClassName, "Lifecycle Entered: OnStart");
			base.OnStart();
			logger.Debug (this.LocalClassName, "Lifecycle Exited: OnStart");
		}

		protected override void OnStop()
		{
			logger.Debug (this.LocalClassName, "Lifecycle Entered: OnStop");
			base.OnStop();		
			logger.Debug (this.LocalClassName, "Lifecycle Exited: OnStop");
		}

		protected override void OnDestroy()
		{
			logger.Debug (this.LocalClassName, "Lifecycle Entered: OnDestroy");
			base.OnDestroy();		
			logger.Debug (this.LocalClassName, "Lifecycle Exited: OnDestroy");
		}

		protected override void OnResume()
		{
			logger.Debug (this.LocalClassName, "Lifecycle Entered: OnResume");
			base.OnResume();
			MainApp.SetCurrentActivity (this);
			logger.Debug (this.LocalClassName, "Lifecycle Exited: OnResume");
		}

		protected override void OnPause()
		{
			logger.Debug (this.LocalClassName, "Lifecycle Entered: OnPause");
			base.OnPause();
			logger.Debug (this.LocalClassName, "Lifecycle Exited: OnPause");
		}

		private MyNotification GetSelectedNotification()
		{
			return MainApp.SelectedNotification;
		}

		private void InitializeComponents ()
		{
			eventDateText = FindViewById<TextView> (Resource.Id.eventDateText);
			eventTimeText = FindViewById<TextView>(Resource.Id.eventTimeText);
			eventDongleIDText = FindViewById<TextView>(Resource.Id.eventDongleIDText);
			eventLocationText = FindViewById<TextView>(Resource.Id.eventLocationText);
			notification = GetSelectedNotification();
		}

		private void SetMojioEventInfo ()
		{
			eventDateText.Text = notification.Date;
			logger.Information (this.LocalClassName, string.Format ("Notificiation Detail Date Set: {0}", notification.Date));
			eventTimeText.Text = notification.Time;
			logger.Information (this.LocalClassName, string.Format ("Notificiation Detail Time Set: {0}", notification.Time));
			eventDongleIDText.Text = notification.DongleID;
			logger.Information (this.LocalClassName, string.Format ("Notificiation Detail DongleID Set: {0}", notification.DongleID));
			eventLocationText.Text = string.Format("Lat,Lng - {0:0.00}, {1:0.00}", notification.LatLng.Latitude, notification.LatLng.Longitude);
			logger.Information (this.LocalClassName, eventLocationText.Text);
		}

		private void SetupMaps()
		{
			try 
			{
				MapsInitializer.Initialize(this);
			} 
			catch (Exception e) 
			{
				logger.Error (this.LocalClassName, string.Format("Exception while initializing the map: {0}", e.Message));
			}

			MapFragment mapFrag = (MapFragment) FragmentManager.FindFragmentById(Resource.Id.eventMapFragment);
			GoogleMap map = mapFrag.Map;

			if (map != null) 
			{
				map.UiSettings.ZoomControlsEnabled = true;
				map.AddMarker (GetMarkerOption(notification));
				map.MapType = GoogleMap.MapTypeNormal;
				map.MoveCamera (CameraUpdateFactory.NewLatLngZoom(GetLocationBoundary(notification).Center,10));
			}
		}

		private LatLng GetEventLocation(MyNotification notification)
		{
			return notification.LatLng;
		}

		private LatLngBounds GetLocationBoundary(MyNotification notification)
		{
			return new LatLngBounds (GetEventLocation(notification), GetEventLocation(notification));
		}

		private MarkerOptions GetMarkerOption(MyNotification notification)
		{
			MarkerOptions marker = new MarkerOptions ();
			marker.SetPosition (notification.LatLng);
			marker.SetTitle (notification.DongleID);
			return marker;
		}
	}
}
