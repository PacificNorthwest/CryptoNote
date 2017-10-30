using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.IO;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using Android.Security;
using Android.Hardware.Fingerprints;

using Java.Security;
using Java.Util;
using Javax.Crypto;

using Newtonsoft.Json;
using CryptoNote.Model;

namespace CryptoNote.Security
{
    class SecurityProvider
    {
        private const string STORE_NAME = "AndroidKeyStore";
        private const string ALIAS = "RSA_Keys";
        private const string ASYMMETRIC_ALGORITHM = "RSA/ECB/PKCS1Padding";
        
        private static string _tDES_key_path;
        private static string _notes_path;
        private static byte[] _tDES_key;
        private static KeyStore _keyStore;
        private static KeyPair _keyPair;

        public static bool KeyExists() => File.Exists(_tDES_key_path);

        public static void InitializeSecuritySystem()
        {
            _keyStore = KeyStore.GetInstance(STORE_NAME);
            _keyStore.Load(null);

            _notes_path = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "notes.dat");
            _tDES_key_path = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "key.dat");
        }

        public static void SaveNotes()
        {
            string json = JsonConvert.SerializeObject(NoteStorage.Notes);
            TripleDESCryptoServiceProvider tDES = new TripleDESCryptoServiceProvider
            {
                Key = _tDES_key,
                Mode = System.Security.Cryptography.CipherMode.ECB,
                Padding = PaddingMode.PKCS7
            };

            ICryptoTransform crypt = tDES.CreateEncryptor();
            File.WriteAllBytes(_notes_path, crypt.TransformFinalBlock(Encoding.UTF8.GetBytes(json), 0, Encoding.UTF8.GetByteCount(json)));
        }

        public static async void SaveNotesAsync()
        {
            await Task.Factory.StartNew(SaveNotes);
        }

        public static void LoadNotes()
        {
            if (_tDES_key != null && File.Exists(_notes_path))
            {
                TripleDESCryptoServiceProvider tDES = new TripleDESCryptoServiceProvider
                {
                    Key = _tDES_key,
                    Mode = System.Security.Cryptography.CipherMode.ECB,
                    Padding = PaddingMode.PKCS7
                };

                ICryptoTransform crypt = tDES.CreateDecryptor();
                byte[] buffer = File.ReadAllBytes(_notes_path);
                NoteStorage.Notes = JsonConvert.DeserializeObject<List<Note>>(Encoding.UTF8.GetString(crypt.TransformFinalBlock(buffer, 0, buffer.Length)));
            }
        }
        
        public static byte[] DecryptKey()
        {
            if (_keyPair != null && File.Exists(_tDES_key_path))
            {

                byte[] buffer = File.ReadAllBytes(_tDES_key_path);
                Cipher cipher = Cipher.GetInstance(ASYMMETRIC_ALGORITHM, "AndroidKeyStoreBCWorkaround");
                cipher.Init(Javax.Crypto.CipherMode.DecryptMode, _keyPair.Private);
                return cipher.DoFinal(buffer);
            }
            return null;

        }

        public static byte[] EncryptKey(byte[] key)
        {
            Cipher cipher = Cipher.GetInstance(ASYMMETRIC_ALGORITHM, "AndroidKeyStoreBCWorkaround");
            cipher.Init(Javax.Crypto.CipherMode.EncryptMode, _keyPair.Public);
            return cipher.DoFinal(key);
        }

        public static bool PasswordAuthenticate(string password)
        {
            AccessKeyStore();
            if (ComputeHash(password).SequenceEqual(DecryptKey()))
            {
                _tDES_key = ComputeHash(password);
                return true;
            }
             else return false;
        }

        public static void FingerprintAuthenticate(Activity activity)
        {
            FingerprintManager fingerprint = activity.GetSystemService(Context.FingerprintService) as FingerprintManager;
            KeyguardManager keyGuard = activity.GetSystemService(Context.KeyguardService) as KeyguardManager;
            Android.Content.PM.Permission permission = activity.CheckSelfPermission(Android.Manifest.Permission.UseFingerprint);
            if (fingerprint.IsHardwareDetected
                && keyGuard.IsKeyguardSecure
                && fingerprint.HasEnrolledFingerprints
                && permission == Android.Content.PM.Permission.Granted)
            {
                const int flags = 0;
                CryptoObjectFactory cryptoHelper = new CryptoObjectFactory();
                CancellationSignal cancellationSignal = new CancellationSignal();
                FingerprintManager.AuthenticationCallback authCallback = new AuthCallback(activity as Activities.LoginActivity);
                fingerprint.Authenticate(cryptoHelper.BuildCryptoObject(), cancellationSignal, flags, authCallback, null);
            }
        }

        public static void FingerprintAuthenticationSucceeded()
        {
            AccessKeyStore();
            _tDES_key = DecryptKey();
        }

        private static void AccessKeyStore()
        {
            if (_keyStore.ContainsAlias(ALIAS))
            {
                IPrivateKey privateKey = (_keyStore.GetEntry(ALIAS, null) as KeyStore.PrivateKeyEntry).PrivateKey;
                IPublicKey publicKey = _keyStore.GetCertificate(ALIAS).PublicKey;
                _keyPair = new KeyPair(publicKey, privateKey);
            }
        }

        public static void InitializeUser(string password, Context context)
        {
            _tDES_key = ComputeHash(password);
            if (CreateNewRSAKeyPair(ALIAS, context))
                File.WriteAllBytes(_tDES_key_path, EncryptKey(_tDES_key));
        }

        public static void EnrollNewPassword(string oldPassword, string newPassword, string confirmPassword)
        {
            if (ComputeHash(oldPassword).SequenceEqual(_tDES_key))
            {
                if (newPassword == confirmPassword)
                {
                    _tDES_key = ComputeHash(newPassword);
                    SaveNotesAsync();
                    File.WriteAllBytes(_tDES_key_path, EncryptKey(_tDES_key));
                }
                else throw new Exception("Passwords missmatch!");
            }
            else throw new Exception("Wrong password!");
        }

        private static bool CreateNewRSAKeyPair(string alias, Context context)
        {
            try
            {
                Calendar start = Calendar.GetInstance(Java.Util.TimeZone.Default);
                Calendar end = Calendar.GetInstance(Java.Util.TimeZone.Default);
                end.Add(CalendarField.Year, 100);
                KeyPairGeneratorSpec spec = new KeyPairGeneratorSpec.Builder(context)
                                                .SetAlias(alias)
                                                .SetSubject(new Javax.Security.Auth.X500.X500Principal("CN=CryptoTouch, O=Android Authority"))
                                                .SetSerialNumber(Java.Math.BigInteger.One)
                                                .SetStartDate(start.Time)
                                                .SetEndDate(end.Time)
                                                .Build();
                KeyPairGenerator generator = KeyPairGenerator.GetInstance("RSA", STORE_NAME);
                generator.Initialize(spec);
                _keyPair = generator.GenerateKeyPair();

                return true;
            }
            catch (Exception ex)
            { Toast.MakeText(context, ex.Message, ToastLength.Long).Show(); return false; }
        }

        public static byte[] ComputeHash(string data)
            => new MD5CryptoServiceProvider().ComputeHash(Encoding.UTF8.GetBytes(data));
    }
}