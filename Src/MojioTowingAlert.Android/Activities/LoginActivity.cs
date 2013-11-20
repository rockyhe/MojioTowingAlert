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

using Mojio;
using Mojio.Client;

namespace MojioTowingAlert.Android
{
	[Activity (Label = "LoginActivity")]                        
	public class LoginActivity : Activity
	{
		Button loginButton;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			SetContentView (Resource.Layout.Login);

			// Get button from layout and attach event
			loginButton = FindViewById<Button>(Resource.Id.logInButton);
			loginButton.Click += new EventHandler (OnLoginClicked);

		}

		public void OnLoginClicked(object sender, EventArgs e)
		{
			EditText username = FindViewById<EditText>(Resource.Id.usernameEntry);
			EditText password = FindViewById<EditText>(Resource.Id.passwordEntry);

			GotoMainMenu (username.Text);

		}

		private void GotoMainMenu(string username)
		{
			var login = new Intent(this, typeof(MainMenuActivity));
			login.PutExtra ("UsernameData", username);
			StartActivity(login);
		}

	}
}

