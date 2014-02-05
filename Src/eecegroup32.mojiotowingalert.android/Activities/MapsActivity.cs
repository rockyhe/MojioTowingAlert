using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using Android.OS;
using Android.App;
using Android.Views;
using Android.Widget;
using Android.Runtime;
using Android.Content;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Mojio;
using Mojio.Events;
using eecegroup32.mojiotowingalert.core;
using System.Threading.Tasks;
using System.Threading;

namespace eecegroup32.mojiotowingalert.android
{
	[Activity (Label = "MapsActivity")]			
	public class MapsActivity : BaseActivity
	{
		private List<LatLng> mojioLocations;
		private List<MarkerOptions> dongleMarkers;
		private LatLngBounds locationBoundary;
		private GoogleMap map;
		private bool stopUpdate;

		protected override void OnCreate (Bundle bundle)
		{
			MyLogger.Debug (this.LocalClassName, "Lifecycle Entered: OnCreate");
			base.OnCreate (bundle);
			this.ActionBar.SetBackgroundDrawable (Resources.GetDrawable (Resource.Drawable.Black));
			this.ActionBar.SetTitle (Resource.String.map);
			SetContentView (Resource.Layout.Maps);
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
			stopUpdate = false;
			StartAutoUpdate ();
			MyLogger.Debug (this.LocalClassName, "Lifecycle Exited: OnResume");
		}

		protected override void OnPause ()
		{
			MyLogger.Debug (this.LocalClassName, "Lifecycle Entered: OnPause");
			base.OnPause ();
			stopUpdate = true;
			MyLogger.Debug (this.LocalClassName, "Lifecycle Exited: OnPause");
		}

		private void SetupMaps ()
		{
			try {
				MapsInitializer.Initialize (this);
			} catch (Exception e) {
				MyLogger.Error (this.LocalClassName, string.Format ("Exception while initializing the map: {0}", e.Message));
			}

			MapFragment mapFrag = (MapFragment)FragmentManager.FindFragmentById (Resource.Id.map);
			map = mapFrag.Map;

			if (map != null) {
				GrabLocations ();
				SetupBoundary ();
				SetupMarkers ();

				map.UiSettings.ZoomControlsEnabled = true;

				foreach (MarkerOptions marker in dongleMarkers) {
					map.AddMarker (marker);
				}

				map.MapType = GoogleMap.MapTypeNormal;
				map.MoveCamera (CameraUpdateFactory.NewLatLngZoom (locationBoundary.Center, 0));
			}
		}

		public void StartAutoUpdate ()
		{
			Task.Factory.StartNew (() => {
				while (stopUpdate == false) {
					LoadMojioDevices ();
					if (HasNumberOfDevicesChanged ()) {
						GrabLocations ();
						SetupBoundary ();
					}					
					SetupMarkers ();
					RunOnUiThread (() => {
						map.Clear ();
						foreach (MarkerOptions marker in dongleMarkers) {
							map.AddMarker (marker);
							MyLogger.Information (this.LocalClassName, string.Format ("Mojio Location: Updated")); 
						}
					});
					Thread.Sleep (2000);
				}
			});
		}

		private bool HasNumberOfDevicesChanged ()
		{
			return mojioLocations.Count == UserDevices.Count;
		}

		private void GrabLocations ()
		{
			Device mojioDevice;
			mojioLocations = new List<LatLng> ();
			for (int i = 0; i < UserDevices.Count; i++) {
				mojioDevice = UserDevices [i];
				mojioLocations.Add (new LatLng (mojioDevice.LastLocation.Lat, mojioDevice.LastLocation.Lng));
				MyLogger.Information (this.LocalClassName, string.Format ("Mojio Location: {0} found at Latitude {1} Longitude {2}", mojioDevice.Name, mojioDevice.LastLocation.Lat, mojioDevice.LastLocation.Lng)); 
			}
		}

		private void SetupBoundary ()
		{
			locationBoundary = new LatLngBounds (mojioLocations [0], mojioLocations [0]);
			for (int i = 1; i < mojioLocations.Count; i++) {
				locationBoundary.Including (mojioLocations [i]);
			}			
		}

		private void SetupMarkers ()
		{
			if (dongleMarkers == null)
				dongleMarkers = new List<MarkerOptions> ();
			else
				dongleMarkers.Clear ();
			for (int i = 0; i < mojioLocations.Count; i++) {
				MarkerOptions marker = new MarkerOptions ();
				marker.SetPosition (mojioLocations [i]);
				marker.SetTitle (UserDevices [i].Name);
				dongleMarkers.Add (marker);
			}
		}
	}
}
