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

namespace eecegroup32.mojiotowingalert.android
{

	[Activity (Label = "MapsActivity")]			
	public class MapsActivity : BaseActivity
	{
		Mojio.Client.MojioClient client;
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
				builder.Bearing (155);
				builder.Tilt (50);
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
				Guid appID = new Guid("1e9dac04-5acb-477a-9a0f-f3ce8600498b");
				Guid secretKey = new Guid("c22d37ca-4997-4d35-a159-d6ac993af8f0");

				client = new Mojio.Client.MojioClient(
					appID, 
					secretKey,
					Mojio.Client.MojioClient.Sandbox // or MojioClient.Live
				);

				client.SetUser( "timmy.nan@gmail.com", "ilafC123");
				mojio = client.Get<Mojio.Device> ("SimTest_kSI7kitwwuq4Igd1eN59");
		}
	}
}

