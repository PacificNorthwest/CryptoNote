using Android.App;
using Android.Widget;
using Android.OS;
using Android.Hardware.Fingerprints;
using Android.Content;
using Android.Transitions;
using Android.Support.V7.App;
using Android.Graphics;
using Android.Views;
using System.Threading.Tasks;
using System;
using Android.Animation;

namespace CryptoTouch.Activities
{
    [Activity(Label = "CryptoNote", Theme ="@style/AppTheme", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class LoginActivity : AppCompatActivity
    {
        private View _progressBar;
        protected override void OnCreate(Bundle bundle)
        {
            Window.RequestFeature(Android.Views.WindowFeatures.ActivityTransitions);
            Window.RequestFeature(Android.Views.WindowFeatures.ContentTransitions);
            base.OnCreate(bundle);
            SetContentView (Resource.Layout.LoginPage);

            Typeface font = Typeface.CreateFromAsset(Assets, "fonts/BROADW.ttf");
            FindViewById<TextView>(Resource.Id.loginPageTitle).Typeface = font;
            FindViewById<Button>(Resource.Id.ButtonSubmitAuthorization).Click += (object sender, EventArgs e) => PasswordAuthorization();
            _progressBar = FindViewById<RelativeLayout>(Resource.Id.progressBar);
            SecurityProvider.FingerprintAuthenticate(this);
        }

        protected override void OnRestart()
        {
            base.OnRestart();
            SecurityProvider.FingerprintAuthenticate(this);
        }

        private async void PasswordAuthorization()
        {
            if (SecurityProvider.PasswordAuthenticate(FindViewById<EditText>(Resource.Id.AuthorizationPassword).Text))
            {
                RevealProgressBar();
                await Task.Run(() => SecurityProvider.LoadNotes());
                ActivityOptions options = ActivityOptions.MakeSceneTransitionAnimation(this);
                Intent intent = new Intent(this, typeof(MainPageActivity));
                HideProgressBar();
                StartActivity(intent, options.ToBundle());
            }
            else
            {
                FindViewById<EditText>(Resource.Id.AuthorizationPassword).Text = string.Empty;
                Toast.MakeText(this, "Incorrect password!", ToastLength.Long).Show();
            }
        }

        public async void OnAuthenticationSucceeded()
        {
            RevealProgressBar();
            SecurityProvider.FingerprintAuthenticationSucceeded();
            await Task.Run(() => SecurityProvider.LoadNotes());
            ActivityOptions options = ActivityOptions.MakeSceneTransitionAnimation(this);
            Intent intent = new Intent(this, typeof(MainPageActivity));
            HideProgressBar();
            StartActivity(intent, options.ToBundle());
        }

        public void OnAuthenticationFailed()
        {
            Toast.MakeText(this, "Fingerprint scan failed", ToastLength.Long).Show();
            SecurityProvider.FingerprintAuthenticate(this);
        }

        private void RevealProgressBar()
        {
            if (_progressBar.Visibility == ViewStates.Invisible && NoteStorage.Notes != null)
            {
                int centerX = _progressBar.Width / 2;
                int centerY = _progressBar.Height / 2;
                float radius = Math.Max(_progressBar.Height, _progressBar.Width);
                
                Animator reveal = ViewAnimationUtils.CreateCircularReveal(_progressBar, centerX, centerY, 0, radius);
                reveal.SetDuration(700);
                _progressBar.Visibility = ViewStates.Visible;
                reveal.Start();
            }
        }

        private void HideProgressBar()
        {
            if (_progressBar.Visibility == ViewStates.Visible)
            {
                int centerX = _progressBar.Width / 2;
                int centerY = _progressBar.Height / 2;
                float radius = Math.Max(_progressBar.Height, _progressBar.Width);
                Animator hide = ViewAnimationUtils.CreateCircularReveal(_progressBar, centerX, centerY, radius, 0);
                hide.SetDuration(300);
                hide.AnimationEnd += (object sender, EventArgs e) => _progressBar.Visibility = ViewStates.Invisible;
                hide.Start();
            }
        }
    }
}

