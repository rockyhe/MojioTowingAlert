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

using Mojio.Client;

namespace eecegroup32.mojiotowingalert.android
{
	[Activity(Label = "LoginActivity")]
	public class LoginActivity : BaseActivity
	{
		string logTag = "LoginActivity";

		Button loginButton;
		EditText username;
		EditText password;

		private void InitializeComponents ()
		{
			loginButton = FindViewById<Button> (Resource.Id.logInButton);
			username = FindViewById<EditText>(Resource.Id.usernameEntry);
			password = FindViewById<EditText>(Resource.Id.passwordEntry);
		}

		private void InitializeEventHandlers ()
		{
			loginButton.Click += new EventHandler (OnLoginClicked);
		}

		protected override void OnCreate(Bundle bundle)
		{
			base.OnCreate(bundle);
			SetContentView(Resource.Layout.Login);
			InitializeComponents ();
			InitializeEventHandlers ();
		}

		private bool IsCredentialEmpty ()
		{
			bool isEmpty = string.IsNullOrEmpty (username.Text) || string.IsNullOrEmpty (password.Text);
			Android.Util.Log.Info(logTag, string.Format("Credential: {0}", isEmpty ? "Empty" : "Exists")); 
			return isEmpty;
		}

		private void PrepareForAsyncLogin ()
		{
			loginButton.Activated = false;
		}

		private void SubmitAsyncLoginRequest ()
		{
			Client.SetUserAsync (username.Text, password.Text).ContinueWith (r =>  {
				MojioResponse<Mojio.Token> response = r.Result;
				RunOnUiThread (() =>  {
					if (Client.IsLoggedIn ()) 
					{
						Android.Util.Log.Info(logTag, "Login Attempt: Pass"); 
						GotoMainMenu ();
					}
					else 
					{
						loginButton.Activated = true;
						Android.Util.Log.Info(logTag, "Login Attempt: Fail"); 
						ErrorMessage (Resource.String.wrongCredentials);
					}
				});
			});
		}

		private void OnLoginClicked(object sender, EventArgs e)
		{
			if (IsCredentialEmpty ()) {
				ErrorMessage (Resource.String.missingUsernameOrPassword);
				return;
			}

			PrepareForAsyncLogin ();
			SubmitAsyncLoginRequest ();
		}

		private void GotoMainMenu()
		{
			StartActivity(new Intent(this, typeof(MainMenuActivity)));
		}

		private void ErrorMessage(int errorId)
		{
			var temp = Toast.MakeText(this, errorId, ToastLength.Long);
			temp.SetGravity(GravityFlags.CenterVertical, 0, 0);
			temp.Show();
		}

	}
}

