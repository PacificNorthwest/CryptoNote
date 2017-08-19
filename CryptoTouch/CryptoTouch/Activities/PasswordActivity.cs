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

namespace CryptoTouch.Activities
{
    [Activity(Label = "CryptoTouch", MainLauncher = true, Icon = "@drawable/fingerprint")]
    public class PasswordActivity : Activity
    {
        private Button _submitButton;
        private EditText _passwordEditText;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SecurityProvider.InitializeSecuritySystem();
            if (SecurityProvider.AliasExists())
                OpenAuthorizationPage();
            this.SetContentView(Resource.Layout.PasswordPage);
            InitializeViews();
        }

        private void InitializeViews()
        {
            _submitButton = FindViewById<Button>(Resource.Id.ButtonSubmitPassword);
            _passwordEditText = FindViewById<EditText>(Resource.Id.RegisterPassword);

            _submitButton.Click += (object sender, EventArgs e) => SubmitPassword();
        }

        private void SubmitPassword()
        {
            SecurityProvider.InitializeUser(_passwordEditText.Text, this);
            Intent intent = new Intent(this, typeof(MainPageActivity));
            StartActivity(intent);
        }

        private void OpenAuthorizationPage()
        {
            Intent intent = new Intent(this, typeof(LoginActivity));
            StartActivity(intent);
        }
    }
}