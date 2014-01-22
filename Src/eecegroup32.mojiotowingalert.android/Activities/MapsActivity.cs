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

	[Activity (Label = "MapsActivity")]			
	public class MapsActivity : BaseActivity
	{
		string logTag = "MapsActivity";

		List<Mojio.Device> mojioDevices;
		List<LatLng> mojioLocations;
		List<MarkerOptions> dongleMarkers;
		LatLngBounds locationBoundary;


		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			SetContentView (Resource.Layout.Maps);
			SetupMaps();
		}

		private void SetupMaps()
		{
			try {
				MapsInitializer.Initialize(this);
			} catch (Exception e) {
				Android.Util.Log.Error (logTag, string.Format("Exception while initializing the map: {0}", e.Message));
			}

			MapFragment mapFrag = (MapFragment) FragmentManager.FindFragmentById(Resource.Id.map);
			GoogleMap map = mapFrag.Map;

			if (map != null) {
				SetupMojio();
				GrabLocations();
				SetupBoundary();
				SetupMarkers ();

				map.UiSettings.ZoomControlsEnabled = true;

				foreach (MarkerOptions marker in dongleMarkers) {
					map.AddMarker (marker);
				}

				map.MapType = GoogleMap.MapTypeNormal;
				map.MoveCamera (CameraUpdateFactory.NewLatLngZoom(locationBoundary.Center,10));
			}
		}

		private void SetupMojio()
		{
			//TODO: Currently assuming only one device per user.
			mojioDevices = new List<Mojio.Device>();
			var devices = Client.UserMojios(Client.CurrentUser.Id);
			foreach(Device moj in devices.Data)
			{
				mojioDevices.Add(moj);
			}
		}

		private void GrabLocations()
		{
			mojioLocations = new List<LatLng>();
			for(int i =0; i < mojioDevices.Count ; i++)
			{
				mojioLocations.Add(new LatLng (mojioDevices [i].LastLocation.Lat, mojioDevices [i].LastLocation.Lng));
			}
		}

		private void SetupBoundary()
		{
			switch (mojioLocations.Count) 
			{
				case 1:
					locationBoundary = new LatLngBounds (mojioLocations [0], mojioLocations[0]);
					break;
				case 2:
					locationBoundary = new LatLngBounds (mojioLocations [0],mojioLocations [0]).Including(mojioLocations[1]);
					break;
				case 3:
					locationBoundary = new LatLngBounds (mojioLocations [0],mojioLocations [0]).Including(mojioLocations[1]).Including(mojioLocations[2]);
					break;
				case 4:
				locationBoundary = new LatLngBounds (mojioLocations [0],mojioLocations [0]).Including(mojioLocations[1]).Including(mojioLocations[2]).Including(mojioLocations[3]);
					break;
			}
		}

		private void SetupMarkers()
		{
			dongleMarkers = new List<MarkerOptions> ();
			for (int i = 0; i < mojioLocations.Count; i++) {
				MarkerOptions marker = new MarkerOptions ();
				marker.SetPosition (mojioLocations[i]);
				marker.SetTitle (mojioDevices[i].Name);
				dongleMarkers.Add (marker);
			}
		}
	}
}
