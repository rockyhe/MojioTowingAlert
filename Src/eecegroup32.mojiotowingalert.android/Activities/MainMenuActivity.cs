using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using Android.OS;
using Android.Locations;
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
using Android.Graphics;

namespace eecegroup32.mojiotowingalert.android
{
	[Activity (Label = "MainMenuActivity",
		AlwaysRetainTaskState = true)]			
	public class MainMenuActivity : EventBaseActivity
	{
		private GoogleMap map;
		private bool stopUpdate;
		private HashSet<Device> devicesToShow;
		private HashSet<Marker> deviceMarkers;
		private HashSet<Marker> eventMarkers;
		private HashSet<TowEvent> towEventsOnMap;
		private Object padlock = new Object ();
		private ManualResetEvent manualResetEvent;
		private ManualResetEvent manualResetEventForUpdate;
		private ManualResetEvent manualResetEventForUpdate2;
		private Button notifcationButton;
		private Button settingsButton;
		private Button logOutButton;
		private Button locateButton;
		private Button refreshButton;
		private Dialog locationDialog;
		private TextView welcome;

		protected override void OnCreate (Bundle bundle)
		{
			MyLogger.Debug (this.LocalClassName, "Lifecycle Entered: OnCreate");
			base.OnCreate (bundle);
			SetContentView (Resource.Layout.MainMenu);
			InitializeActionBar ();
			InitializeVariables ();
			InitializeEventHandlers ();
			InitializeWelcomeScreen ();
			Task.Factory.StartNew (() => {
				try {
					LoadLastEvents (EventsToSubscribe);
					AddEventMarkers ();
				} catch (Exception) {
					NotifyViaToast ("Mojio Server Error. Failed To Load Events.");
				}
			}).Wait (1000);

			try {
				DrawMap ();
			} catch (Exception e) {
				NotifyViaToast ("Mojio Server Error. Please Try Later.");
				Finish ();
			}
			MyLogger.Debug (this.LocalClassName, "Lifecycle Exited: OnCreate");
		}

		protected override void OnResume ()
		{
			MyLogger.Debug (this.LocalClassName, "Lifecycle Entered: OnResume");
			base.OnResume ();
			MainApp.SetCurrentActivity (this);
			UpdateNumberOfNewEvents ();
			StartAutoUpdate ();
			MyLogger.Debug (this.LocalClassName, "Lifecycle Exited: OnResume");
		}

		private void InitializeVariables ()
		{
		
			welcome = FindViewById<TextView> (Resource.Id.welcomeText);
			notifcationButton = FindViewById<Button> (Resource.Id.notificationsButton);
			settingsButton = FindViewById<Button> (Resource.Id.settingsButton);
			logOutButton = FindViewById<Button> (Resource.Id.logOutButton);
			refreshButton = FindViewById<Button> (Resource.Id.refreshButton);
			locateButton = FindViewById<Button> (Resource.Id.locateButton);
			devicesToShow = new HashSet<Device> ();
			foreach (var dev in UserDevices)
				devicesToShow.Add (dev);
			deviceMarkers = new HashSet<Marker> ();
			eventMarkers = new HashSet<Marker> ();
			towEventsOnMap = new HashSet<TowEvent> ();
			manualResetEvent = new ManualResetEvent (false);
			manualResetEventForUpdate = new ManualResetEvent (false);
			manualResetEventForUpdate2 = new ManualResetEvent (true);
			stopUpdate = true;
		}

		private void InitializeActionBar ()
		{
			ActionBar.SetBackgroundDrawable (Resources.GetDrawable (Resource.Drawable.Black));
			ActionBar.SetTitle (Resource.String.mainMenu);
		}

		private void InitializeEventHandlers ()
		{

			notifcationButton.Click += new EventHandler (OnNotificationsClicked);
			settingsButton.Click += new EventHandler (OnSettingsClicked);
			logOutButton.Click += new EventHandler (OnLogOutClicked);
			refreshButton.Click += new EventHandler (OnRefreshClicked);
			locateButton.Click += new EventHandler (OnLocateClicked);
		}

		private void InitializeWelcomeScreen ()
		{
			string username = string.Empty;
			if (Client != null && Client.CurrentUser != null) {
				username = Client.CurrentUser.UserName;
			}
			welcome.Text = "Welcome " + username;
		}

		protected override void OnMojioEventReceived (Event eve)
		{
			base.OnMojioEventReceived (eve);
			UpdateNumberOfNewEvents ();
		}

		private void UpdateNumberOfNewEvents ()
		{
			if (!(MainApp.GetCurrentActivity () is MainMenuActivity)) {
				MyLogger.Information (this.LocalClassName, string.Format ("Notification Button not updated because MainMenuActivity is not visible."));
			} else {
				int numberOfNewEvents = TowManager.GetNewEventNumber ();
				MyLogger.Information (this.LocalClassName, string.Format ("{0} New Events found.", numberOfNewEvents));
				if (numberOfNewEvents == 0) {
					notifcationButton.Text = Resources.GetString (Resource.String.notifications);
				} else {
					var msg = string.Format ("{0} ({1})", Resources.GetString (Resource.String.notifications), numberOfNewEvents);
					notifcationButton.Text = msg;
				}
			}
		}

		private void OnNotificationsClicked (object sender, EventArgs e)
		{
			StartActivity (new Intent (this, typeof(NotificationsActivity)));
		}
			
		private void OnSettingsClicked (object sender, EventArgs e)
		{
			StartActivity (new Intent (this, typeof(SettingsActivity)));
		}

		private void OnRefreshClicked (object sender, EventArgs e)
		{
			UpdateDeviceMarkers ();
			UpdateEventMarkers ();
			NotifyViaToast ("Map Updated");
//			stopUpdate = !stopUpdate;
//			if (stopUpdate) {
//				NotifyViaToast ("Map Auto Refresh: Off");
//				refreshButton.SetBackgroundDrawable (Resources.GetDrawable (Resource.Drawable.refresh_button));
//			} else {
//				NotifyViaToast ("Map Auto Refresh: On");
//				refreshButton.SetBackgroundDrawable (Resources.GetDrawable (Resource.Drawable.refresh_button_inverted));
//			}
//			StartAutoUpdate ();
		}

		private void OnLocateClicked (object sender, EventArgs e)
		{
			locationDialog = CreateDeviceDialog ();
			AddDevicesToDialog (locationDialog);
			AddEventToDialog (locationDialog);
			locationDialog.Show ();
		}

		private Dialog CreateDeviceDialog ()
		{
			Dialog dialog = new Dialog (this);
			dialog.SetTitle ("Select Device or Event");
			dialog.Window.SetLayout (LinearLayout.LayoutParams.WrapContent, LinearLayout.LayoutParams.WrapContent);
			dialog.SetContentView (Resource.Layout.SelectDongles);
			dialog.Window.SetTitleColor (Color.White);
			return dialog;
		}

		private void OnLogOutClicked (object sender, EventArgs e)
		{
			Client.ClearUser ();
			GotoLogin ();
			Finish ();
		}

		private void GotoLogin ()
		{
			var login = new Intent (this, typeof(LoginActivity));
			login.AddFlags (ActivityFlags.ClearTop);
			StartActivity (login);
		}

		private void AddDevicesToDialog (Dialog dialog)
		{
			var layout = dialog.FindViewById<LinearLayout> (Resource.Id.SelectDeviceLayout);		
			foreach (Device dev in UserDevices) {
				Button listItem = CreateDeviceSelectionItem (dev);
				//layout.SetPadding (5, 10, 5, 10);

				LinearLayout.LayoutParams layoutParams = new LinearLayout.LayoutParams(
					LinearLayout.LayoutParams.FillParent, LinearLayout.LayoutParams.WrapContent);
				layoutParams.SetMargins (5, 5, 5, 5);
				layoutParams.Height = 100;
				layout.AddView (listItem,layoutParams);
			}
		}

		private Button CreateDeviceSelectionItem (Device moj)
		{
			Button button = new Button (this);
			//button.SetPaddingRelative(5, 5, 5, 5);
			//button.SetBackgroundColor(Color.Rgb(1,187,225));
			button.SetBackgroundDrawable(Resources.GetDrawable(Resource.Drawable.android_button));
			button.SetMaxHeight (25);
			button.Text = string.Format (moj.Name);
			button.Tag = moj.Id;
			button.Click += OnDeviceSelected;
			return button;
		}

		private void OnDeviceSelected (object sender, EventArgs e)
		{
			var selectedButton = (Button)sender;
			var selectedDevice = UserDevices.FirstOrDefault (x => x.Id.Equals ((string)selectedButton.Tag));
			Device device = (Device)selectedDevice;
			LatLng latln = new LatLng (device.LastLocation.Lat, device.LastLocation.Lng);
			map.MoveCamera (CameraUpdateFactory.NewLatLngZoom (latln, 15));
			locationDialog.Hide ();
		}

		private void AddEventToDialog (Dialog dialog)
		{
			var layout = dialog.FindViewById<LinearLayout> (Resource.Id.SelectDeviceLayout);		
			foreach (Event eve in TowManager.Get(1)) {
				Button listItem = CreateEventItem (eve);
				LinearLayout.LayoutParams layoutParams = new LinearLayout.LayoutParams(
					LinearLayout.LayoutParams.FillParent, LinearLayout.LayoutParams.WrapContent);
				layoutParams.SetMargins (5, 5, 5, 5);
				layoutParams.Height = 100;
				layout.AddView (listItem, layoutParams);
			}
		}

		private Button CreateEventItem (Event Event)
		{
			Button button = new Button (this);
			button.SetBackgroundDrawable(Resources.GetDrawable(Resource.Drawable.android_button));
			//button.SetPadding(5, 5, 5, 5);
			//button.SetBackgroundColor(Color.Rgb(1,187,225));
			button.Text = string.Format ("Latest Tow Event");
			button.Click += (sender, e) => {
				LatLng latln = new LatLng (Event.Location.Lat, Event.Location.Lng);
				map.MoveCamera (CameraUpdateFactory.NewLatLngZoom (latln, 15));
				locationDialog.Hide ();
			};
			;
			return button;
		}

		private void DrawMap ()
		{
			InitializeGoogleMap ();			
			AddMarkers ();
		}

		void InitializeGoogleMap ()
		{
			try {
				MapsInitializer.Initialize (this);
			} catch (Exception e) {
				MyLogger.Error (this.LocalClassName, string.Format ("Exception while initializing the map: {0}", e.Message));
			}
			MapFragment mapFrag = (MapFragment)FragmentManager.FindFragmentById (Resource.Id.map);
			map = mapFrag.Map;
			map.UiSettings.ZoomControlsEnabled = true;
			map.MapType = GoogleMap.MapTypeNormal;
			map.InfoWindowClick += OnEventMarkerClicked;
		}

		private void AddMarkers ()
		{			
			AddDeviceMarkers ();
			AddEventMarkers ();		
			var deviceLocations = deviceMarkers.Select (x => x.Position);
			ZoomMapTo (deviceLocations);
		}

		private void AddDeviceMarkers ()
		{
			foreach (var marker in deviceMarkers)
				RemoveMarker (marker);
			deviceMarkers.Clear ();
			for (int i = 0; i < UserDevices.Count; i++) {
				var dev = UserDevices [i];
				if (devicesToShow.FirstOrDefault (x => x.Id.Equals (dev.Id)) != null)
					deviceMarkers.Add (AddDeviceMarkerToMap (dev));								
			}

		}

		public void UpdateEvents()
		{
			AddEventMarkers ();
			foreach (Event eve in TowManager.Get(1)) {
				LatLng latln = new LatLng (eve.Location.Lat, eve.Location.Lng);
				map.MoveCamera (CameraUpdateFactory.NewLatLngZoom (latln, 15));
			}
		}

		private Marker AddDeviceMarkerToMap (Device dev)
		{
			var loc = new LatLng (dev.LastLocation.Lat, dev.LastLocation.Lng);				
			MarkerOptions marker = new MarkerOptions ();
			marker.InvokeIcon (BitmapDescriptorFactory.DefaultMarker (BitmapDescriptorFactory.HueGreen));
			marker.SetPosition (loc);
			marker.SetSnippet (dev.Id);
			marker.SetTitle (dev.Name);
			MyLogger.Information (this.LocalClassName, string.Format ("Device Marker Added: {0} at {1}", dev.Name, loc.ToString ()));
			return AddMarkerToMap (marker);
		}

		private void AddEventMarkers ()
		{
			foreach (var marker in eventMarkers)
				RemoveMarker (marker);
			eventMarkers.Clear ();
			var towEvents = GetTowEvents ();
			foreach (var towEvent in towEvents) {
				var id = towEvent.MojioId;
				if (devicesToShow.FirstOrDefault (x => x.Id.Equals (id)) != null) {
					towEventsOnMap.Add (towEvent);
					eventMarkers.Add (AddTowEventMarkerToMap (towEvent));
				}	
			}

		}

		private IEnumerable<TowEvent> GetTowEvents ()
		{
			return TowManager.GetAll ().Where (x => x.EventType == EventType.TowStart).Cast <TowEvent> ();
		}

		private Marker AddTowEventMarkerToMap (TowEvent towEvent)
		{
			if (towEvent.Location != null) {
				var loc = new LatLng (towEvent.Location.Lat, towEvent.Location.Lng);
				MarkerOptions marker = new MarkerOptions ();
				marker.SetPosition (loc);
				marker.SetTitle (string.Format ("Device: {0}", towEvent.MojioId));
				marker.SetSnippet (string.Format ("Date: {0}", towEvent.Time));
				MyLogger.Information (this.LocalClassName, string.Format ("Event Marker Added: {0} at {1}", towEvent.MojioId, loc.ToString ()));
				return AddMarkerToMap (marker);
			}

			return null;
		}

		private Marker AddMarkerToMap (MarkerOptions markerOption)
		{
			lock (padlock) {
				manualResetEvent.Reset ();
				Marker marker = null;
				RunOnUiThread (() => {
					marker = map.AddMarker (markerOption);
					manualResetEvent.Set ();
				});
				manualResetEvent.WaitOne ();
				return marker;
			}
		}

		private void ZoomMapTo (IEnumerable<LatLng> locations)
		{
			LatLngBounds locationBoundary = null;

			if (locations != null || locations.Count () != 0)
				locationBoundary = new LatLngBounds (locations.First (), locations.First ());

			if (locations.Count () > 1)
				foreach (var loc in locations)
					locationBoundary.Including (loc);

			map.MoveCamera (CameraUpdateFactory.NewLatLngZoom (locationBoundary.Center, 10));
		}

		public void StartAutoUpdate ()
		{
//			Task.Factory.StartNew (() => {
//				try {
//					while (stopUpdate == false) {
//						manualResetEventForUpdate2.WaitOne ();
//						manualResetEventForUpdate.Reset ();
//						UpdateDeviceMarkers ();
//						UpdateEventMarkers ();					
//						manualResetEventForUpdate.Set ();
//						Thread.Sleep (2000);					
//					}
//				} catch (Exception e) {
//					NotifyViaToast ("Mojio Server Error. Please Try Later.");
//					Finish ();
//				}
//			});
		}

		private void UpdateDeviceMarkers ()
		{
			var suceed = Task.Factory.StartNew (LoadMojioDevices).Wait (5000);
			if (!suceed) {
				MyLogger.Error (this.LocalClassName, "Mojio device not updated within the timelimit");
				return;
			}
			foreach (var marker in deviceMarkers)
				RemoveMarker (marker);
			AddDeviceMarkers ();
		}

		private void RemoveMarker (Marker marker)
		{
			if (marker == null)
				return;
			RunOnUiThread (() => {
				marker.Remove ();
			});
		}

		private void UpdateEventMarkers ()
		{
			var towEvents = GetTowEvents ();
			foreach (var currentEvent in towEvents) {				
				if (towEventsOnMap.FirstOrDefault (x => x.Id == currentEvent.Id) == null) {
					towEventsOnMap.Add (currentEvent);
					AddTowEventMarkerToMap (currentEvent);
				}				
			}			
		}

		protected override void OnPause ()
		{
			base.OnPause ();
		}

		private void OnEventMarkerClicked (object sender, GoogleMap.InfoWindowClickEventArgs e)
		{
			var marker = e.P0;
			var towEvent = GetTowEvents ().FirstOrDefault (x => marker.Snippet.Contains (x.Time.ToString ()));
			if (towEvent == null)
				NotifyViaToast ("Event Info Not Available!");
			var towDetailsActivity = new Intent (this, typeof(TowDetailsActivity));
			towDetailsActivity.PutExtra ("selectedEventId", towEvent.Id.ToString ());
			StartActivity (towDetailsActivity);  
		}
	}
}

