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
		private List<LatLng> locations;
		private List<MarkerOptions> markers;
		private LatLngBounds locationBoundary;
		private GoogleMap map;
		private bool stopUpdate;
		private Task task;
		Button filterButton;
		private List<Device> devicesToShow;

		protected override void OnCreate (Bundle bundle)
		{
			MyLogger.Debug (this.LocalClassName, "Lifecycle Entered: OnCreate");
			base.OnCreate (bundle);
			this.ActionBar.SetBackgroundDrawable (Resources.GetDrawable (Resource.Drawable.Black));
			this.ActionBar.SetTitle (Resource.String.map);
			SetContentView (Resource.Layout.Maps);			
			filterButton = FindViewById<Button> (Resource.Id.mapFilterButton);
			filterButton.Click += new EventHandler (OnFilterButtonClicked);			
			task = Task.Factory.StartNew (() => LoadLastEvents (EventsToSubscribe));
			devicesToShow = new List<Device> ();
			devicesToShow.AddRange (UserDevices);
			MyLogger.Debug (this.LocalClassName, "Lifecycle Exited: OnCreate");
		}

		private void OnFilterButtonClicked (object sender, EventArgs e)
		{
			Dialog dialog = new Dialog (this);
			dialog.SetTitle ("Select Dongles");

			dialog.Window.SetLayout (LinearLayout.LayoutParams.WrapContent, LinearLayout.LayoutParams.WrapContent);
			dialog.SetContentView (Resource.Layout.SelectDongles);
			dialog.Window.SetTitleColor (global::Android.Graphics.Color.LightYellow);

			var layout = dialog.FindViewById<LinearLayout> (Resource.Id.SelectDeviceLayout);
			if (layout == null || Client == null)
				return;

			// Query API for all of users mojio devices
			var res = Client.UserMojios (Client.CurrentUser.Id);
			foreach (Device moj in UserDevices) {
				ToggleButton button = new ToggleButton (this);
				if (devicesToShow.Contains (moj))
					button.Checked = true;
				else
					button.Checked = false;
				button.Text = string.Format ("Name:{0} \nId:{1}", moj.Name, moj.IdToString);
				button.Tag = moj.Id;
				button.Click += (o, args) => {
					var b = (ToggleButton)o;
					var dev = UserDevices.FirstOrDefault (x => x.Id.Equals ((string)b.Tag));
					b.Text = string.Format ("Name:{0} \nId:{1}", dev.Name, dev.IdToString);
					if (b.Checked) {						
						if (!devicesToShow.Contains (dev))
							devicesToShow.Add (dev);
					} else
						devicesToShow.Remove (dev);
				};
				layout.AddView (button);
			}
			dialog.Show ();
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
			SetupMaps ();
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
				PopulateMap ();			
			}
		}

		private void PopulateMap ()
		{
			Device mojioDevice;
			if (locations == null)
				locations = new List<LatLng> ();
			else
				locations.Clear ();
			if (markers == null)
				markers = new List<MarkerOptions> ();
			else
				markers.Clear ();
			for (int i = 0; i < UserDevices.Count; i++) {
				if (!devicesToShow.Contains (UserDevices [i]))
					continue;
				mojioDevice = UserDevices [i];
				var loc = new LatLng (mojioDevice.LastLocation.Lat, mojioDevice.LastLocation.Lng);
				locations.Add (loc);
				if (locationBoundary == null)
					locationBoundary = new LatLngBounds (locations [0], locations [0]);
				locationBoundary.Including (loc);
				MarkerOptions marker = new MarkerOptions ();
				marker.SetPosition (loc);
				marker.SetTitle (mojioDevice.Name);
				markers.Add (marker);
				MyLogger.Information (this.LocalClassName, string.Format ("Mojio Location: {0} found at Latitude {1} Longitude {2}", mojioDevice.Name, mojioDevice.LastLocation.Lat, mojioDevice.LastLocation.Lng));
			}
			while (!task.IsCompleted)
				Thread.Sleep (500);
			foreach (var e in TowManager.GetAll ().Where (x => x.EventType == EventType.Tow)) {
				var id = e.MojioId;
				if (devicesToShow.FirstOrDefault (x => x.Id.Equals (id)) == null)
					continue;
				if (((TowEvent)e).Location != null) {
					var loc = new LatLng (((TowEvent)e).Location.Lat, ((TowEvent)e).Location.Lng);
					locations.Add (loc);
					MarkerOptions marker = new MarkerOptions ();
					marker.SetPosition (loc);
					marker.SetTitle (string.Format ("Device: {0}", e.MojioId));
					marker.SetSnippet (string.Format ("Date: {0}", e.Time));
					markers.Add (marker);
				}
			}
			map.UiSettings.ZoomControlsEnabled = true;
			foreach (MarkerOptions marker in markers) {
				map.AddMarker (marker);
			}
			map.MapType = GoogleMap.MapTypeNormal;
			map.MoveCamera (CameraUpdateFactory.NewLatLngZoom (locationBoundary.Center, 0));
		}

		public void StartAutoUpdate ()
		{
			Task.Factory.StartNew (() => {
				while (stopUpdate == false) {
					LoadMojioDevices ();
					UpdateMarkers ();
					RunOnUiThread (() => {
						map.Clear ();
						foreach (MarkerOptions marker in markers) {
							map.AddMarker (marker);
							MyLogger.Information (this.LocalClassName, string.Format ("Mojio Location: Updated")); 
						}
					});
					Thread.Sleep (2000);
				}
			});
		}

		private void UpdateMarkers ()
		{
			locations.Clear ();
			markers.Clear ();
			for (int i = 0; i < UserDevices.Count; i++) {
				if (!devicesToShow.Contains (UserDevices [i]))
					continue;
				var mojioDevice = UserDevices [i];
				var loc = new LatLng (mojioDevice.LastLocation.Lat, mojioDevice.LastLocation.Lng);
				locations.Add (loc);
				MarkerOptions marker = new MarkerOptions ();
				marker.SetPosition (loc);
				marker.SetTitle (mojioDevice.Name);
				markers.Add (marker);
			}
			
			var events = TowManager.GetAll ().Where (x => x.EventType == EventType.Tow);
			if (events == null)
				return;
			
			try {
				events = events.Where (x => ((TowEvent)x).Location != null);
			} catch (Exception e) {				
				return;
			}
			
			if (events == null || events.Count () == 0)
				return;
			
			foreach (var e in events) {
				var id = e.MojioId;
				if (devicesToShow.FirstOrDefault (x => x.Id.Equals (id)) == null)
					continue;
				var loc = new LatLng (((TowEvent)e).Location.Lat, ((TowEvent)e).Location.Lng);
				locations.Add (loc);
				MarkerOptions marker = new MarkerOptions ();
				marker.SetPosition (loc);
				marker.SetTitle (string.Format ("Device: {0}", e.MojioId));
				marker.SetSnippet (string.Format ("Date: {0}", e.Time));
				markers.Add (marker);
			}
		}

		private bool HasNumberOfDevicesChanged ()
		{
			return locations.Count == UserDevices.Count;
		}
	}
}
