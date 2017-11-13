using Android.Support.V4.Hardware.Fingerprint;
using Javax.Crypto;

using CryptoNote.Activities;


namespace CryptoNote.Security
{
    /// <summary>
    /// Fingerprint authentication result callback
    /// No need to touch this class, it's complete and doesn't need to be reworked
    /// </summary>
    class AuthCallback : FingerprintManagerCompat.AuthenticationCallback
    {
        private static readonly byte[] SECRET_BYTES = { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
        private static readonly string TAG = "CryptoNote";
        private readonly LoginActivity _currentActivity;

        public AuthCallback() { }
        public AuthCallback(LoginActivity activity) { _currentActivity = activity; }

        /// <summary>
        /// Succeeded event handler
        /// </summary>
        /// <param name="result"></param>
        public override void OnAuthenticationSucceeded(FingerprintManagerCompat.AuthenticationResult result)
        {
            if (result.CryptoObject.Cipher != null)
            {
                try
                {
                    byte[] doFinalResult = result.CryptoObject.Cipher.DoFinal(SECRET_BYTES);
                    _currentActivity.OnAuthenticationSucceeded();
                }
                catch (BadPaddingException bpe)
                {
                    _currentActivity.OnAuthenticationFailed();
                }
                catch (IllegalBlockSizeException ibse)
                {
                    _currentActivity.OnAuthenticationFailed();
                }
            }
            else { _currentActivity.OnAuthenticationSucceeded(); }
        }

        /// <summary>
        /// Failed event handler
        /// </summary>
        public override void OnAuthenticationFailed() { _currentActivity.OnAuthenticationFailed(); }
    }
}