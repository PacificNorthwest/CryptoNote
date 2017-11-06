using System.Threading.Tasks;
using System;

using Android.App;
using Android.Widget;
using Android.OS;
using Android.Content;
using Android.Support.V7.App;
using Android.Graphics;
using Android.Views;
using Android.Animation;

using CryptoNote.Security;
using CryptoNote.Model;

namespace CryptoNote.Activities
{

    /// <summary>
    /// Login page activity
    /// </summary>
    [Activity(Label = "CryptoNote", Theme ="@style/AppTheme", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class LoginActivity : AppCompatActivity
    {
        private View _progressBar;

        /// <summary>
        /// Activity creation event
        /// </summary>
        /// <param name="bundle"></param>
        protected override void OnCreate(Bundle bundle)
        {
            Window.RequestFeature(Android.Views.WindowFeatures.ActivityTransitions);
            Window.RequestFeature(Android.Views.WindowFeatures.ContentTransitions);
            base.OnCreate(bundle);
            SetContentView (Resource.Layout.LoginPage);
            
            FindViewById<TextView>(Resource.Id.loginPageTitle).Typeface = Typeface.CreateFromAsset(Assets, "fonts/BROADW.ttf");
            FindViewById<TextView>(Resource.Id.fingerprintScanHint).Typeface = Typeface.CreateFromAsset(Assets, "fonts/AlbertusNovaThin.otf");
            FindViewById<TextView>(Resource.Id.passwordUsageHint).Typeface = Typeface.CreateFromAsset(Assets, "fonts/AlbertusNovaThin.otf");
            FindViewById<Button>(Resource.Id.ButtonSubmitAuthorization).Typeface = Typeface.CreateFromAsset(Assets, "fonts/AlbertusNovaThin.otf");
            FindViewById<Button>(Resource.Id.ButtonSubmitAuthorization).Click += (object sender, EventArgs e) => PasswordAuthorization();
            _progressBar = FindViewById<RelativeLayout>(Resource.Id.progressBar);
            SecurityProvider.FingerprintAuthenticate(this);
        }

        /// <summary>
        /// Updating textviews and reinitializing authentication mechanism
        /// </summary>
        protected override void OnStart()
        {
            base.OnStart();
            SecurityProvider.FingerprintAuthenticate(this);
            FindViewById<TextView>(Resource.Id.fingerprintScanHint).Text = Resources.GetString(Resource.String.FingerprintScanHint);
            FindViewById<TextView>(Resource.Id.passwordUsageHint).Text = Resources.GetString(Resource.String.PasswordUsageHint);
            FindViewById<Button>(Resource.Id.ButtonSubmitAuthorization).Text = Resources.GetString(Resource.String.LoginButton);
        }

        /// <summary>
        /// Authorization with password
        /// </summary>
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
                Toast.MakeText(this, this.Resources.GetString(Resource.String.IncorrectPsswordError), ToastLength.Long).Show();
            }
        }

        /// <summary>
        /// Fingerprint authorization succeeded event handler
        /// </summary>
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

        /// <summary>
        /// Fingerprint authorization failed event handler
        /// </summary>
        public void OnAuthenticationFailed()
        {
            Toast.MakeText(this, this.Resources.GetString(Resource.String.FingerprintScanFailedError), ToastLength.Long).Show();
            SecurityProvider.FingerprintAuthenticate(this);
        }

        /// <summary>
        /// Progress bar revelation animation
        /// </summary>
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

        /// <summary>
        /// Progress bar hiding animation
        /// </summary>
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

        public override void OnBackPressed()
        {
            this.FinishAffinity();
        }
    }
}

