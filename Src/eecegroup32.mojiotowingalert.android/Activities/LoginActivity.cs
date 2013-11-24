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

			//Get button from layout and attach event handler
			loginButton = FindViewById<Button>(Resource.Id.logInButton);
			loginButton.Click += new EventHandler(OnLoginClicked);

			//Check for network connectivity
			if (!ConnectedToNetwork)
			{
				return;
			}

		}

		public void OnLoginClicked(object sender, EventArgs e)
		{
			EditText username = FindViewById<EditText>(Resource.Id.usernameEntry);
			EditText password = FindViewById<EditText>(Resource.Id.passwordEntry);

			//Check if username or password is empty
			if (string.IsNullOrEmpty(username.Text) || string.IsNullOrEmpty(password.Text))
			{
				ErrorMessage (Resource.String.missingUsernameOrPassword);
				return;
			}

			loginButton.Activated = false;

			//Submit login request
			Client.SetUserAsync(username.Text, password.Text).ContinueWith(r => {
				MojioResponse<Mojio.Token> response = r.Result;

				RunOnUiThread(() => {
					//Re-activate login button
					loginButton.Activated = true;

					//Check if login was a success or not
					if (Client.IsLoggedIn())
					{
						GotoMainMenu(username.Text);
					}
					else
					{
						loginButton.Activated = true;
						ErrorMessage(Resource.String.wrongCredentials);
					}
				});
			});
		}

		private void GotoMainMenu(string username)
		{
			var menu = new Intent(this, typeof(MainMenuActivity));
			menu.PutExtra("UsernameData", username);
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

