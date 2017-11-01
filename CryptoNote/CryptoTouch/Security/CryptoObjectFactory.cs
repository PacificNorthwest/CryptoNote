using System;
using Android.Hardware.Fingerprints;
using Android.Security.Keystore;
using Java.Security;
using Javax.Crypto;

namespace CryptoNote.Security
{
    class CryptoObjectFactory
    {
        private static readonly string KEY_NAME = "com.xamarin.android.cryptonote.fingerprint_authentication_key";
        private static readonly string KEYSTORE_NAME = "AndroidKeyStore";

        private static readonly string KEY_ALGORITHM = KeyProperties.KeyAlgorithmAes;
        private static readonly string BLOCK_MODE = KeyProperties.BlockModeCbc;
        private static readonly string ENCRYPTION_PADDING = KeyProperties.EncryptionPaddingPkcs7;
        private static readonly string TRANSFORMATION = KEY_ALGORITHM + "/" +
                                                BLOCK_MODE + "/" +
                                                ENCRYPTION_PADDING;
        private readonly KeyStore _keystore;

        public CryptoObjectFactory()
        {
            _keystore = KeyStore.GetInstance(KEYSTORE_NAME);
            _keystore.Load(null);
        }

        public FingerprintManager.CryptoObject BuildCryptoObject()
        {
            Cipher cipher = CreateCipher();
            return new FingerprintManager.CryptoObject(cipher);
        }

        private Cipher CreateCipher(bool retry = true)
        {
            IKey key = GetKey();
            Cipher cipher = Cipher.GetInstance(TRANSFORMATION);

            try
            {
                cipher.Init(CipherMode.EncryptMode, key);
            }
            catch (KeyPermanentlyInvalidatedException e)
            {
                _keystore.DeleteEntry(KEY_NAME);
                if (retry)
                {
                    CreateCipher(false);
                }
                else
                {
                    throw new Exception("Could not create the cipher for fingerprint authentication.", e);
                }
            }
            return cipher;
        }

        private IKey GetKey()
        {
            IKey secretKey;
            if (!_keystore.IsKeyEntry(KEY_NAME))
            {
                CreateKey();
            }

            secretKey = _keystore.GetKey(KEY_NAME, null);
            return secretKey;
        }

        private void CreateKey()
        {
            KeyGenerator keyGen = KeyGenerator.GetInstance(KEY_ALGORITHM, KEYSTORE_NAME);
            KeyGenParameterSpec keyGenSpec =
                new KeyGenParameterSpec.Builder(KEY_NAME, KeyStorePurpose.Encrypt | KeyStorePurpose.Decrypt)
                    .SetBlockModes(BLOCK_MODE)
                    .SetEncryptionPaddings(ENCRYPTION_PADDING)
                    .SetUserAuthenticationRequired(true)
                    .Build();
            keyGen.Init(keyGenSpec);
            keyGen.GenerateKey();
        }
    }
}