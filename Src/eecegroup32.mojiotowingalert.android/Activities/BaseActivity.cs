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
using Android.Net;

using Mojio.Client;

namespace eecegroup32.mojiotowingalert.android
{
	public class BaseActivity : Activity
	{
		public static bool ConnectedToNetwork;

		public MojioClient Client
		{
			get { return MainApp.Client; }
		}

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			ConnectedToNetwork = CheckNetworkConnection();

			//If not connected to network, display an alert with an Exit button
			//If Exit button is clicked, close the app
			if (!ConnectedToNetwork)
			{
				AlertDialog.Builder alert = new AlertDialog.Builder(this);
				alert.SetTitle("Error");
				alert.SetMessage(Resource.String.noConnectivity);
				alert.SetPositiveButton("Exit", delegate { Finish(); });
				alert.Show();
				return;
			}
		}

		protected bool CheckNetworkConnection()
		{
			ConnectivityManager connectivityManager = (ConnectivityManager)GetSystemService(ConnectivityService); 
			NetworkInfo activeConnection = connectivityManager.ActiveNetworkInfo;
			if ((activeConnection != null) && activeConnection.IsConnected)
			{
				return true;
			}
			return false;
		}

	}
}

