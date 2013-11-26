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
		Button loginButton;

		protected override void OnCreate(Bundle bundle)
		{
			base.OnCreate(bundle);
			SetContentView(Resource.Layout.Login);

			//Get the login button from Resource and add a click event handler
			loginButton = FindViewById<Button>(Resource.Id.logInButton);
			loginButton.Click += new EventHandler(OnLoginClicked);

			//Check for network connectivity
			if (!ConnectedToNetwork)
			{
				return;
			}

		}

		private void OnLoginClicked(object sender, EventArgs e)
		{
			//Get the username and password entry text fields from Resource
			EditText username = FindViewById<EditText>(Resource.Id.usernameEntry);
			EditText password = FindViewById<EditText>(Resource.Id.passwordEntry);

			//Check if username or password is empty
			if (string.IsNullOrEmpty(username.Text) || string.IsNullOrEmpty(password.Text))
			{
				ErrorMessage(Resource.String.missingUsernameOrPassword);
				return;
			}

			//Deactivate the button before calling async login
			loginButton.Activated = false;

			//Submit an async login request
			Client.SetUserAsync(username.Text, password.Text).ContinueWith(r => {
				MojioResponse<Mojio.Token> response = r.Result;

				RunOnUiThread(() => {
					//Reactivate login button
					loginButton.Activated = true;

					//If login succeeds, go to main menu. Otherwise reactivate button and show error message.
					if (Client.IsLoggedIn())
					{
						GotoMainMenu();
					}
					else
					{
						loginButton.Activated = true;
						ErrorMessage(Resource.String.wrongCredentials);
					}
				});
			});
		}

		private void GotoMainMenu()
		{
			var menu = new Intent(this, typeof(MainMenuActivity));
			StartActivity(menu);
		}

		private void ErrorMessage(int errorId)
		{
			Toast temp = Toast.MakeText(this, errorId, ToastLength.Long);
			temp.SetGravity(GravityFlags.CenterVertical, 0, 0);
			temp.Show();
		}

	}
}

