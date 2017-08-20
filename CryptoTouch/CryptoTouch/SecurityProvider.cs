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
using System.Security.Cryptography;
using Java.Security;
using System.IO;
using Android.Security;
using Java.Util;
using Android.Hardware.Fingerprints;
using Javax.Crypto;
using Java.IO;
using Android.Util;
using Newtonsoft.Json;

namespace CryptoTouch
{
    class SecurityProvider
    {
        private const string STORE_NAME = "AndroidKeyStore";
        private static KeyStore _keyStore;
        private static KeyPair _keyPair;
        private static string _aliasPath;
        private static string _notesPath;
        private static string _alias;
        
        public static bool AliasExists() => System.IO.File.Exists(_aliasPath);

        public static void InitializeSecuritySystem()
        {
            _keyStore = KeyStore.GetInstance(STORE_NAME);
            _keyStore.Load(null);

            _notesPath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "notes.dat");
            _aliasPath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "alias.dat");
            
            if (System.IO.File.Exists(_aliasPath))
                using (StreamReader reader = new StreamReader(_aliasPath))
                    _alias = reader.ReadLine();
            
        }

        public static void LoadNotes()
        {
            if (_keyPair != null && System.IO.File.Exists(_notesPath))
            {
                
                string buffer = System.IO.File.ReadAllText(_notesPath);
                Cipher cipher = Cipher.GetInstance("RSA/ECB/PKCS1Padding", "AndroidOpenSSL");
                cipher.Init(Javax.Crypto.CipherMode.DecryptMode, _keyPair.Private);
                CipherInputStream cipherInputStream = new CipherInputStream(
                    new MemoryStream(Base64.Decode(buffer, Base64Flags.Default)), cipher);
                List<Byte> values = new List<Byte>();
                int nextByte;
                while ((nextByte = cipherInputStream.Read()) != -1)
                    values.Add((byte)nextByte);

                string json = Encoding.UTF8.GetString(values.ToArray());
                NoteStorage.Notes = JsonConvert.DeserializeObject<List<Note>>(json);
            }

        }

        public static void SaveNotes()
        {
            string json = JsonConvert.SerializeObject(NoteStorage.Notes);
            Cipher cipher = Cipher.GetInstance("RSA/ECB/PKCS1Padding", "AndroidOpenSSL");
            cipher.Init(Javax.Crypto.CipherMode.EncryptMode, _keyPair.Public);
            MemoryStream memoryStream = new MemoryStream();
            CipherOutputStream cipherOutputStream = new CipherOutputStream(memoryStream, cipher);
            cipherOutputStream.Write(Encoding.UTF8.GetBytes(json));
            cipherOutputStream.Close();
            System.IO.File.WriteAllText( _notesPath ,Base64.EncodeToString(memoryStream.ToArray(), Base64Flags.Default));
        }

        public static bool PasswordAuthenticate(string password)
        {
            if (ComputeHash(password) == _alias)
            {
                AccessKeyStore();
                return true;
            }
            return false;
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
        }

        private static void AccessKeyStore()
        {
            if (_keyStore.ContainsAlias(_alias))
            {
                IPrivateKey privateKey = (_keyStore.GetEntry(_alias, null) as KeyStore.PrivateKeyEntry).PrivateKey;
                IPublicKey publicKey = _keyStore.GetCertificate(_alias).PublicKey;
                _keyPair = new KeyPair(publicKey, privateKey);
            }
        }

        public static void InitializeUser(string password, Context context)
        {
            string alias = ComputeHash(password);
            if (alias != _alias)
                if (CreateNewKeyPair(alias, context))
                {
                    SaveAlias(alias);
                    InitializeSecuritySystem();
                }
        }

        private static bool CreateNewKeyPair(string alias, Context context)
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

        private static void SaveAlias(string alias)
        {
            using (FileStream stream = new FileStream(_aliasPath, FileMode.Create, FileAccess.Write))
            using (StreamWriter writer = new StreamWriter(stream))
                writer.WriteLine(alias);
        }

        public static string ComputeHash(string data)
        {
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            byte[] hash = md5.ComputeHash(Encoding.UTF8.GetBytes(data));
            return BitConverter.ToString(hash).Replace("-", string.Empty);
        }
    }
}