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
using eecegroup32.mojiotowingalert.core;
using System.Threading.Tasks;
using System.Threading;

namespace eecegroup32.mojiotowingalert.android
{
	[Activity (Label = "Mojio Towing Alert App", MainLauncher = true, NoHistory = true)]
	public class LoginActivity : BaseActivity
	{
		private Button loginButton;
		private EditText username;
		private EditText password;
		private ProgressDialog progressDialog;

		protected override void OnCreate (Bundle bundle)
		{
			MyLogger.Debug (this.LocalClassName, "Lifecycle Entered: OnCreate");
			base.OnCreate (bundle);
			SetContentView (Resource.Layout.Login);
			InitializeVariables ();
			InitializeEventHandlers ();
			MyLogger.Debug (this.LocalClassName, "Lifecycle Exited: OnCreate");
		}

		protected override void OnResume ()
		{
			MyLogger.Debug (this.LocalClassName, "Lifecycle Entered: OnResume");
			base.OnResume ();
			MainApp.SetCurrentActivity (this);
			MyLogger.Debug (this.LocalClassName, "Lifecycle Exited: OnResume");
		}

		private void InitializeVariables ()
		{
			this.ActionBar.SetBackgroundDrawable (Resources.GetDrawable (Resource.Drawable.Black));
			this.ActionBar.SetTitle (Resource.String.application);
			loginButton = FindViewById<Button> (Resource.Id.logInButton);
			username = FindViewById<EditText> (Resource.Id.usernameEntry);
			password = FindViewById<EditText> (Resource.Id.passwordEntry);
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

		private void SubmitAsyncLoginRequest ()
		{
			//NotifyViaToast (Resources.GetString (Resource.String.loggingIn));
			loginButton.Activated = false;
			progressDialog = ProgressDialog.Show (this, "Please wait...", "Checking account info...", true);	
			Client.SetUserAsync (username.Text, password.Text).ContinueWith (r => {
				MojioResponse<Mojio.Token> response = r.Result;
				RunOnUiThread (() => {					
					if (Client.IsLoggedIn ()) {
						MyLogger.Information (this.LocalClassName, string.Format ("Login Attempt: Pass - {0}", response.Data.ToString ())); 
						GotoMainMenu ();
					} else {
						loginButton.Activated = true;
						MyLogger.Information (this.LocalClassName, "Login Attempt: Fail"); 
						progressDialog.Dismiss ();
						NotifyViaToast (Resources.GetString (Resource.String.wrongCredentials));
					}
				});
			});
		}

		private void OnLoginClicked (object sender, EventArgs e)
		{
			if (IsCredentialEmpty ())
				NotifyViaToast (Resources.GetString (Resource.String.missingUsernameOrPassword));
			else
				SubmitAsyncLoginRequest ();
		}

		private void GotoMainMenu ()
		{
		
			var mainMenu = new Intent (this, typeof(MainMenuActivity));
			mainMenu.AddFlags (ActivityFlags.ClearTask);
			StartActivity (mainMenu);
			Finish ();
		}
	}
}

