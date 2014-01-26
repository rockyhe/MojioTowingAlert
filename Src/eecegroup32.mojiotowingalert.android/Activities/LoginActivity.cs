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
		private Button loginButton;
		private EditText username;
		private EditText password;

		protected override void OnCreate(Bundle bundle)
		{
			logger.Debug (this.LocalClassName, "Lifecycle Entered: OnCreate");

			base.OnCreate(bundle);
			SetContentView(Resource.Layout.Login);
			InitializeComponents ();
			InitializeEventHandlers ();

			logger.Debug (this.LocalClassName, "Lifecycle Exited: OnCreate");
		}

		protected override void OnStart()
		{
			logger.Debug (this.LocalClassName, "Lifecycle Entered: OnStart");
			base.OnStart();		
			logger.Debug (this.LocalClassName, "Lifecycle Exited: OnStart");
		}

		protected override void OnStop()
		{
			logger.Debug (this.LocalClassName, "Lifecycle Entered: OnStop");
			base.OnStop();		
			logger.Debug (this.LocalClassName, "Lifecycle Exited: OnStop");
		}

		protected override void OnDestroy()
		{
			logger.Debug (this.LocalClassName, "Lifecycle Entered: OnDestroy");
			base.OnDestroy();		
			logger.Debug (this.LocalClassName, "Lifecycle Exited: OnDestroy");
		}

		protected override void OnResume()
		{
			logger.Debug (this.LocalClassName, "Lifecycle Entered: OnResume");
			base.OnResume();
			MainApp.SetCurrentActivity (this);
			logger.Debug (this.LocalClassName, "Lifecycle Exited: OnResume");
		}

		protected override void OnPause()
		{
			logger.Debug (this.LocalClassName, "Lifecycle Entered: OnPause");
			base.OnPause();
			logger.Debug (this.LocalClassName, "Lifecycle Exited: OnPause");
		}

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

		private bool IsCredentialEmpty ()
		{
			bool isEmpty = string.IsNullOrEmpty (username.Text) || string.IsNullOrEmpty (password.Text);
			return isEmpty;
		}

		private void AsyncLoginPrep ()
		{
			loginButton.Activated = false;
		}

		private void AyncLoginCleanup()
		{
			loginButton.Activated = true;
		}

		private void SubmitAsyncLoginRequest ()
		{
			AsyncLoginPrep ();
			Client.SetUserAsync (username.Text, password.Text).ContinueWith (r =>  {
				MojioResponse<Mojio.Token> response = r.Result;
				RunOnUiThread (() =>  {
					if (Client.IsLoggedIn ()) 
					{
						logger.Information (this.LocalClassName, string.Format("Login Attempt: Pass - {0}", response.Data.ToString())); 
						GotoMainMenu ();
					}
					else 
					{
						AyncLoginCleanup();
						logger.Information (this.LocalClassName, "Login Attempt: Fail"); 
						ShowErrorMessage (Resource.String.wrongCredentials);
					}
				});
			});
		}

		private void OnLoginClicked(object sender, EventArgs e)
		{
			if (IsCredentialEmpty ()) {
				ShowErrorMessage (Resource.String.missingUsernameOrPassword);
				return;
			}

			SubmitAsyncLoginRequest ();
		}

		private void GotoMainMenu()
		{
			var mainMenu = new Intent(this, typeof(MainMenuActivity));
			mainMenu.AddFlags(ActivityFlags.ClearTask);
			StartActivity(mainMenu);
		}

		private void ShowErrorMessage(int errorId)
		{
			var temp = Toast.MakeText(this, errorId, ToastLength.Long);
			temp.SetGravity(GravityFlags.CenterVertical, 0, 0);
			temp.Show();
		}

	}
}

