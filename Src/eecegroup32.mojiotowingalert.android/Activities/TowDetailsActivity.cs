using System;
using System.Linq;
using Android.OS;
using Android.App;
using Android.Widget;
using Android.Locations;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using eecegroup32.mojiotowingalert.core;
using Mojio.Events;

namespace eecegroup32.mojiotowingalert.android
{
	[Activity (Label = "TowDetailsActivity")]			
	public class TowDetailsActivity : BaseActivity
	{
		private TextView eventDateText;
		private TextView eventTimeText;
		private TextView eventLocationText;
		private TextView eventDongleIDText;
		private TowEvent selectedEvent;

		protected override void OnCreate (Bundle bundle)
		{
			MyLogger.Debug (this.LocalClassName, "Lifecycle Entered: OnCreate");
			base.OnCreate (bundle);			
			string selectedEventId = Intent.GetStringExtra ("selectedEventId");
			if (string.IsNullOrEmpty (selectedEventId)) {
				MyLogger.Error (this.LocalClassName, "Selected Event ID not passed. Activity closed.");
				Finish ();
			}
			selectedEvent = (TowEvent)TowManager.First (x => x.Id.ToString () == (selectedEventId));			
			SetContentView (Resource.Layout.TowNotificationDetail);
			InitializeVariables ();
			SetMojioEventInfo ();
			SetupMaps ();
			MyLogger.Debug (this.LocalClassName, "Lifecycle Exited: OnCreate");
		}

		protected override void OnStart ()
		{
			MyLogger.Debug (this.LocalClassName, "Lifecycle Entered: OnStart");
			base.OnStart ();
			MyLogger.Debug (this.LocalClassName, "Lifecycle Exited: OnStart");
		}

		protected override void OnStop ()
		{
			MyLogger.Debug (this.LocalClassName, "Lifecycle Entered: OnStop");
			base.OnStop ();		
			MyLogger.Debug (this.LocalClassName, "Lifecycle Exited: OnStop");
		}

		protected override void OnDestroy ()
		{
			MyLogger.Debug (this.LocalClassName, "Lifecycle Entered: OnDestroy");
			base.OnDestroy ();		
			MyLogger.Debug (this.LocalClassName, "Lifecycle Exited: OnDestroy");
		}

		protected override void OnResume ()
		{
			MyLogger.Debug (this.LocalClassName, "Lifecycle Entered: OnResume");
			base.OnResume ();
			MainApp.SetCurrentActivity (this);
			MyLogger.Debug (this.LocalClassName, "Lifecycle Exited: OnResume");
		}

		protected override void OnPause ()
		{
			MyLogger.Debug (this.LocalClassName, "Lifecycle Entered: OnPause");
			base.OnPause ();
			MyLogger.Debug (this.LocalClassName, "Lifecycle Exited: OnPause");
		}

		private void InitializeVariables ()
		{
			this.ActionBar.SetBackgroundDrawable (Resources.GetDrawable (Resource.Drawable.Black));
			this.ActionBar.SetTitle (Resource.String.towEventDetails);
			eventDateText = FindViewById<TextView> (Resource.Id.eventDateText);
			eventTimeText = FindViewById<TextView> (Resource.Id.eventTimeText);
			eventDongleIDText = FindViewById<TextView> (Resource.Id.eventDongleIDText);
			eventLocationText = FindViewById<TextView> (Resource.Id.eventLocationText);
		}

		private void SetMojioEventInfo ()
		{
			eventDateText.Text = selectedEvent.Time.ToLongDateString ();
			MyLogger.Information (this.LocalClassName, string.Format ("Event Detail Date Set: {0}", selectedEvent.Time.ToLongDateString ()));
			eventTimeText.Text = selectedEvent.Time.ToLongTimeString ();
			MyLogger.Information (this.LocalClassName, string.Format ("Event Detail Time Set: {0}", selectedEvent.Time.ToLongTimeString ()));
			eventDongleIDText.Text = selectedEvent.MojioId;
			MyLogger.Information (this.LocalClassName, string.Format ("Event Detail DongleID Set: {0}", selectedEvent.MojioId));
			if (selectedEvent.Location == null)
				eventLocationText.Text = string.Format ("Location - Not Available");
			else {
				String myAddress;
				myAddress = GetAddress (selectedEvent.Location.Lat, selectedEvent.Location.Lng);
				if (myAddress != null)
					eventLocationText.Text = string.Format ("{0}", myAddress);
				else
					eventLocationText.Text = string.Format ("Lat,Lng: {0}, {1} ", selectedEvent.Location.Lat, selectedEvent.Location.Lng);
			}
				MyLogger.Information (this.LocalClassName, eventLocationText.Text);
		}

		private void SetupMaps ()
		{			
			try {
				MapsInitializer.Initialize (this);
			} catch (Exception e) {
				MyLogger.Error (this.LocalClassName, string.Format ("Exception while initializing the map: {0}", e.Message));
			}
			MapFragment mapFrag = (MapFragment)FragmentManager.FindFragmentById (Resource.Id.eventMapFragment);
			GoogleMap map = mapFrag.Map;
			if (map != null) {
				map.UiSettings.ZoomControlsEnabled = true;
				map.AddMarker (GetMarkerOption (selectedEvent));
				map.MapType = GoogleMap.MapTypeNormal;
				map.MoveCamera (CameraUpdateFactory.NewLatLngZoom (GetLocationBoundary (selectedEvent).Center, 15));
			}
		}

		private LatLng GetEventLocation (TowEvent e)
		{
			if (selectedEvent.Location == null)
				return new LatLng (49.2389f, -123.1201f); //Vancouver LatLng
			else
				return new LatLng (e.Location.Lat, e.Location.Lng);
		}

		private LatLngBounds GetLocationBoundary (TowEvent e)
		{
			var location = GetEventLocation (e);
			return new LatLngBounds (location, location);
		}

		private MarkerOptions GetMarkerOption (TowEvent e)
		{
			MarkerOptions marker = new MarkerOptions ();
			marker.SetPosition (GetEventLocation (e));
			marker.SetTitle (e.Location == null ? "Location Not Available!" : string.Format ("{0}", e.MojioId));
			return marker;
		}

		private String GetAddress (float latitude, float longitude){
			try {
				String locationAddress = "";
				Android.Locations.Geocoder geocoder;
				geocoder = new Android.Locations.Geocoder(this);
				var addresses = geocoder.GetFromLocation (latitude ,longitude, 1);

				var list = addresses.ToList<Android.Locations.Address>();
				locationAddress += list[0].GetAddressLine(0)+ "\n";
				locationAddress += list[0].GetAddressLine(1)+ "\n";

				return locationAddress;

			} catch (Exception e) {
				return null;
			}
		}
	}
}
