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

namespace eecegroup32.mojiotowingalert.android
{

	[Activity (Label = "MapsActivity")]			
	public class MapsActivity : Activity
	{
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			// Create your application here
			SetContentView (Resource.Layout.Maps);

			try {
				MapsInitializer.Initialize(this);
			} catch (Exception e) {

			}

			MapFragment mapFrag = (MapFragment) FragmentManager.FindFragmentById(Resource.Id.map);
			GoogleMap map = mapFrag.Map;
			if (map != null) {

				LatLng location = new LatLng(49.261723, -122.857551);
				CameraPosition.Builder builder = CameraPosition.InvokeBuilder();
				builder.Target(location);
				builder.Zoom(18);
				builder.Bearing(155);
				builder.Tilt (50);
				CameraPosition cameraPosition = builder.Build();
				CameraUpdate cameraUpdate = CameraUpdateFactory.NewCameraPosition(cameraPosition);

				MarkerOptions markerOpt1 = new MarkerOptions();
				markerOpt1.SetPosition(new LatLng(49.261723, -122.857551));
				markerOpt1.SetTitle("Tim's Home");
				map.UiSettings.ZoomControlsEnabled = true;

				map.AddMarker(markerOpt1);
				map.MapType = GoogleMap.MapTypeNormal;
				map.MoveCamera(cameraUpdate);
			}
		}
	}
}

