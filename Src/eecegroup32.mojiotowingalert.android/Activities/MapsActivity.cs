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
		private List<LatLng> mojioLocations;
		private List<MarkerOptions> dongleMarkers;
		private LatLngBounds locationBoundary;

		protected override void OnCreate (Bundle bundle)
		{
			logger.Debug (this.LocalClassName, "Lifecycle Entered: OnCreate");

			base.OnCreate (bundle);
			SetContentView (Resource.Layout.Maps);
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

			MapFragment mapFrag = (MapFragment) FragmentManager.FindFragmentById(Resource.Id.map);
			GoogleMap map = mapFrag.Map;

			if (map != null) 
			{
				GrabLocations();
				SetupBoundary();
				SetupMarkers ();

				map.UiSettings.ZoomControlsEnabled = true;

				foreach (MarkerOptions marker in dongleMarkers) 
				{
					map.AddMarker (marker);
				}

				map.MapType = GoogleMap.MapTypeNormal;
				map.MoveCamera (CameraUpdateFactory.NewLatLngZoom(locationBoundary.Center,10));
			}
		}

		private void GrabLocations()
		{
			LoadMojioDevices ();
			Device mojioDevice;
			mojioLocations = new List<LatLng>();
			for(int i =0; i < MojioDevices.Count ; i++)
			{
				mojioDevice = MojioDevices [i];
				mojioLocations.Add(new LatLng (mojioDevice.LastLocation.Lat, mojioDevice.LastLocation.Lng));
				logger.Information (this.LocalClassName, string.Format("Mojio Location: {0} found at Latitude {1} Longitude {2}", mojioDevice.Name, mojioDevice.LastLocation.Lat, mojioDevice.LastLocation.Lng)); 
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
				marker.SetTitle (MojioDevices[i].Name);
				dongleMarkers.Add (marker);
			}
		}
	}
}
