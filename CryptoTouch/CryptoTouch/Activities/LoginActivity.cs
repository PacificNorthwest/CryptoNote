using Android.App;
using Android.Widget;
using Android.OS;
using Android.Hardware.Fingerprints;
using Android.Content;

namespace CryptoTouch.Activities
{
    [Activity(Label = "CryptoTouch")]
    public class LoginActivity : Activity
    {
        protected override void OnCreate(Bundle bundle)
        {
            Window.RequestFeature(Android.Views.WindowFeatures.ActivityTransitions);
            Window.RequestFeature(Android.Views.WindowFeatures.ContentTransitions);
            base.OnCreate(bundle);
            SetContentView (Resource.Layout.LoginPage);
            FindViewById<Button>(Resource.Id.ButtonSubmitAuthorization).Click += (object sender, System.EventArgs e) => PasswordAuthorization();
            SecurityProvider.FingerprintAuthenticate(this);
        }

        private void PasswordAuthorization()
        {
            if (SecurityProvider.PasswordAuthenticate(FindViewById<EditText>(Resource.Id.AuthorizationPassword).Text))
            {
                SecurityProvider.LoadNotes();
                Intent intent = new Intent(this, typeof(MainPageActivity));
                StartActivity(intent);
            }
            else
            {
                FindViewById<EditText>(Resource.Id.AuthorizationPassword).Text = string.Empty;
                Toast.MakeText(this, "Incorrect password!", ToastLength.Long).Show();
            }
        }

        public void OnAuthenticationSucceeded()
        {
            SecurityProvider.FingerprintAuthenticationSucceeded();
            SecurityProvider.LoadNotes();
            ActivityOptions options = ActivityOptions.MakeSceneTransitionAnimation(this);
            Intent intent = new Intent(this, typeof(MainPageActivity));
            StartActivity(intent, options.ToBundle());
        }

        public void OnAuthenticationFailed()
        {
            Toast.MakeText(this, "Fingerprint scan failed", ToastLength.Long).Show();
            SecurityProvider.FingerprintAuthenticate(this);
        }
    }
}

