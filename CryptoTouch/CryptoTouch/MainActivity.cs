using Android.App;
using Android.Widget;
using Android.OS;
using Android.Hardware.Fingerprints;
using Android.Content;

namespace CryptoTouch
{
    [Activity(Label = "CryptoTouch", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView (Resource.Layout.Main);
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
            AlertDialog.Builder dlgAlert = new AlertDialog.Builder(this);
            dlgAlert.SetMessage("Scan succeeded");
            dlgAlert.SetTitle("CryptoTouch");
            dlgAlert.SetPositiveButton("OK", (senderAlert, args) => { dlgAlert.Dispose(); });
            dlgAlert.SetCancelable(true);
            dlgAlert.Create().Show();
        }

        public void OnAuthenticationFailed()
        {
            AlertDialog.Builder dlgAlert = new AlertDialog.Builder(this);
            dlgAlert.SetMessage("Scan failed");
            dlgAlert.SetTitle("CryptoTouch");
            dlgAlert.SetPositiveButton("OK", (senderAlert, args) => { dlgAlert.Dispose(); });
            dlgAlert.SetCancelable(true);
            dlgAlert.Create().Show();
        }
    }
}

