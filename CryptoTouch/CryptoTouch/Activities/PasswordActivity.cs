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
using Android.Support.V7.App;
using Android.Graphics;

namespace CryptoTouch.Activities
{
    [Activity(Label = "CryptoTouch", MainLauncher = true, 
              Icon = "@drawable/fingerprint",
              Theme = "@style/AppTheme",
              ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class PasswordActivity : AppCompatActivity
    {
        private Button _submitButton;
        private EditText _passwordEditText;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SecurityProvider.InitializeSecuritySystem();
            if (SecurityProvider.KeyExists())
                OpenAuthorizationPage();
            this.SetContentView(Resource.Layout.PasswordPage);
            InitializeViews();
        }

        protected override void OnStart()
        {
            base.OnStart();
            FindViewById<TextView>(Resource.Id.passwordPageHintTextView).Text = Resources.GetString(Resource.String.WelcomePassword);
            _submitButton.Text = Resources.GetString(Resource.String.SubmitButton);
        }

        private void InitializeViews()
        {
            _submitButton = FindViewById<Button>(Resource.Id.ButtonSubmitPassword);
            _passwordEditText = FindViewById<EditText>(Resource.Id.RegisterPassword);
            _submitButton.Click += (object sender, EventArgs e) => SubmitPassword();
            _passwordEditText.RequestFocus();
            Typeface font = Typeface.CreateFromAsset(Assets, "fonts/BROADW.ttf");
            FindViewById<TextView>(Resource.Id.passwordPageTitle).Typeface = font;
        }

        private void SubmitPassword()
        {
            if (_passwordEditText.Text != string.Empty)
            {
                SecurityProvider.InitializeUser(_passwordEditText.Text, this);
                Intent intent = new Intent(this, typeof(MainPageActivity));
                StartActivity(intent);
            }
            else
                Toast.MakeText(this, this.Resources.GetString(Resource.String.EmptyPasswordError), ToastLength.Long).Show();
        }

        private void OpenAuthorizationPage()
        {
            Intent intent = new Intent(this, typeof(LoginActivity));
            StartActivity(intent);
        }
    }
}