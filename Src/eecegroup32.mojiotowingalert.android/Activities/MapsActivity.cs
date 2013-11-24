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
		Mojio.Device mojio;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			// Create your application here
			SetContentView (Resource.Layout.Maps);

			SetupMaps();
		
		}


		private void SetupMaps()
		{
			try {
				MapsInitializer.Initialize(this);
			} catch (Exception e) {

			}

			MapFragment mapFrag = (MapFragment) FragmentManager.FindFragmentById(Resource.Id.map);
			GoogleMap map = mapFrag.Map;
			if (map != null) {

				SetupMojio();

				float lat = mojio.LastLocation.Lat;
				float lang = mojio.LastLocation.Lng;

				LatLng location = new LatLng (lat, lang);
				CameraPosition.Builder builder = CameraPosition.InvokeBuilder ();
				builder.Target (location);
				builder.Zoom (18);
				builder.Bearing (0);
				builder.Tilt (0);
				CameraPosition cameraPosition = builder.Build ();
				CameraUpdate cameraUpdate = CameraUpdateFactory.NewCameraPosition (cameraPosition);

				MarkerOptions markerOpt1 = new MarkerOptions ();
				markerOpt1.SetPosition (new LatLng (lat, lang));
				markerOpt1.SetTitle ("Your Dongle");
				map.UiSettings.ZoomControlsEnabled = true;

				map.AddMarker (markerOpt1);
				map.MapType = GoogleMap.MapTypeNormal;
				map.MoveCamera (cameraUpdate);
			}
		}

		private void SetupMojio()
		{
			var devices = Client.UserMojios(Client.CurrentUser.Id);
			foreach(Device moj in devices.Data)
			{
				mojio = moj;
			}
		}
	}
}

