using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Mojio;
using Mojio.Client;

namespace MojioTowingAlert.Android
{
	[Activity(Label = "LoginActivity")]
	public class LoginActivity : BaseActivity
	{
		Button loginButton;

		protected override void OnCreate(Bundle bundle)
		{
//			base.OnCreate(bundle);
//			SetContentView(Resource.Layout.Login);
//
//			// Get button from the layout resource and attach an event to it
//			loginButton = FindViewById<Button>(Resource.Id.LoginButton);
//			loginButton.Click += new EventHandler(OnLoginClicked);
//
//			if (!ConnectedToNetwork)
//				return;
		}

		public void OnLoginClicked(object sender, EventArgs e)
		{
			EditText username = FindViewById<EditText>(Resource.Id.Username);
			EditText password = FindViewById<EditText>(Resource.Id.Password);

			if (String.IsNullOrWhiteSpace (username.Text) || String.IsNullOrWhiteSpace (password.Text)) {
//				Log.Verbose ("No username or password submitted");
//				LoginError (Resource.String.MissingUsernameOrPassword);
				return;
			}

			// Deactivate login button prior to async login call
			loginButton.Activated = false;

			// Submit login request
//			Client.SetUserAsync (username.Text, password.Text).ContinueWith (r => {
//				var response = r.Result;
//
//				Log.Verbose("Login API call response: " + response.StatusCode.ToString() );
//
//				RunOnUiThread (() => {
//					// Re activate login button
//					loginButton.Activated = true;
//
//					// Check if login was a success or not
//					if (Client.IsLoggedIn())
//						GotoShowUser ();
//					else {
//						loginButton.Activated = true;
//						LoginError( Resource.String.WrongCredential );
//					}
//				});
//			});
		}

		void LoginError(int messageId)
		{
			Toast tmp = Toast.MakeText (this, messageId, ToastLength.Long);
			tmp.SetGravity (GravityFlags.CenterVertical, 0, 0);
			tmp.Show ();
		}

		void GotoShowUser()
		{
//			var info = new Intent(this, typeof(UserInfoActivity));
//			// Set flag so that user do not back into login activity from userinfo activity
//			info.AddFlags(ActivityFlags.ClearTop);
//			StartActivity(info);
		}
	}
}

