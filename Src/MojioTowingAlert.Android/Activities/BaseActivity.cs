using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Mojio;
using Mojio.Client;
using Android.Net;
using PushSharp.Client;
using Mojio.Events;

namespace MojioTowingAlert.Android
{
	public class BaseActivity : Activity
	{
		readonly protected ILogger logger = DependancyResolver.Get<ILogger> ();
		readonly protected MojioClient client = DependancyResolver.Get<MojioClient> ();

		const string SharedPreferencesName = "MojioClientTestPreferences";
		public static Device Dev;
		const string DevicePrefs = "MOJIO_DEVICE";

		public static bool ConnectedToNetwork;

		// TODO: Maybe this should be some where else?
		public static MojioClient GetClient(Context context)
		{
			return DependancyResolver.Get<MojioClient> ();
		}

		public MojioClient Client
		{
			get
			{
				return client;
			}
		}

		public ILogger Log {
			get {
				return logger;
			}
		}

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			ConnectedToNetwork = TestNetworkConnection();
			if (!ConnectedToNetwork)
			{
				AlertDialog.Builder alert = new AlertDialog.Builder(this);
//				alert.SetTitle(Resource.String.BadConnectionAlertTitle);
//				alert.SetMessage(Resource.String.BadConnectionAlertMessage);
				alert.SetPositiveButton("Exit", delegate { Finish(); });
				alert.Show();
				return;
			}

			// Lets make sure we have registered for GCM messages.
			if (!PushClient.IsRegistered(this.ApplicationContext))
			{
				//Check to ensure everything's setup right
				PushClient.CheckDevice(this.ApplicationContext);
				PushClient.CheckManifest(this.ApplicationContext);

				//Call to register
//				PushClient.Register(this.ApplicationContext, PushReceiver.SENDER_IDS);
			}
		}

		protected void LoadDeviceSelection()
		{
			Dev = null;
			var preferences = GetSharedPreferences(SharedPreferencesName, FileCreationMode.Private);
			if (preferences.Contains(DevicePrefs))
			{
				var device = preferences.GetString(DevicePrefs, null);
				// Device is not using Guid as object Id in sandbox
				Dev = Client.Get<Device>(device);
			}
		}

		//TODO: better if stored in server: preference on cloud
		protected void SaveDeviceSelection()
		{
			var preferences = GetSharedPreferences(SharedPreferencesName, FileCreationMode.Private); 
			var edits = preferences.Edit();
			if (Dev != null)
				edits.PutString(DevicePrefs, Dev.IdToString);
			else if (preferences.Contains(DevicePrefs))
				edits.Remove(DevicePrefs);

			edits.Commit();
		}

		protected bool TestNetworkConnection()
		{
			var connectivityManager = (ConnectivityManager)GetSystemService(ConnectivityService); 
			var activeConnection = connectivityManager.ActiveNetworkInfo;
			if ((activeConnection != null) && activeConnection.IsConnected)
				return true;
			return false;
		}


		// bookkeeping status of the app: in foreground or not
		protected override void OnResume()
		{
			base.OnResume();
			ActivityResumed();
		}

		protected override void OnPause()
		{
			base.OnPause();
			ActivityPaused();
		}

		public static bool IsActivityVisible()
		{
			return ActivityVisible;
		}

		public static void ActivityResumed()
		{
			ActivityVisible = true;
		}

		public static void ActivityPaused()
		{
			ActivityVisible = false;
		}

		private static bool ActivityVisible;
	}
}

