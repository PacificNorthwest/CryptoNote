using Android.App;
using Android.Widget;
using Android.OS;
using Android.Hardware.Fingerprints;
using Android.Content;

namespace CryptoTouch.Activities
{
    [Activity(Label = "CryptoTouch", MainLauncher = true, Icon = "@drawable/fingerprint")]
    public class LoginActivity : Activity
    {
        protected override void OnCreate(Bundle bundle)
        {
            Window.RequestFeature(Android.Views.WindowFeatures.ActivityTransitions);
            Window.RequestFeature(Android.Views.WindowFeatures.ContentTransitions);
            base.OnCreate(bundle);
            SetContentView (Resource.Layout.LoginPage);
            Authenticate();
        }

        private void Authenticate()
        {
            FingerprintManager fingerprint = this.GetSystemService(FingerprintService) as FingerprintManager;
            KeyguardManager keyGuard = GetSystemService(KeyguardService) as KeyguardManager;
            Android.Content.PM.Permission permission = CheckSelfPermission(Android.Manifest.Permission.UseFingerprint);
            if (fingerprint.IsHardwareDetected
                && keyGuard.IsKeyguardSecure
                && fingerprint.HasEnrolledFingerprints
                && permission == Android.Content.PM.Permission.Granted)
            {
                const int flags = 0;
                CryptoObjectFactory cryptoHelper = new CryptoObjectFactory();
                CancellationSignal cancellationSignal = new CancellationSignal();
                FingerprintManager.AuthenticationCallback authCallback = new AuthCallback(this);
                fingerprint.Authenticate(cryptoHelper.BuildCryptoObject(), cancellationSignal, flags, authCallback, null);
            }
        }

        public void OnAuthenticationSucceeded()
        {
            ActivityOptions options = ActivityOptions.MakeSceneTransitionAnimation(this);
            Intent intent = new Intent(this, typeof(MainPageActivity));
            StartActivity(intent, options.ToBundle());
        }

        public void OnAuthenticationFailed()
        {
            Toast.MakeText(this, "Fingerprint scan failed", ToastLength.Long).Show();
            Authenticate();
        }
    }
}

