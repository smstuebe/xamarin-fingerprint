using System;
using Android.Security.Keystore;
using AndroidX.Biometric;
using Java.Security;
using Javax.Crypto;

namespace Plugin.Fingerprint.Platforms.Android.Utils
{
    internal class CryptoObjectHelper
    {
        // Fixed Android KeyStore Name
        private static readonly string KeyStoreName = "AndroidKeyStore";

        /*
         * Algorithm Setup - Based on
         * https://developer.android.com/training/articles/keystore#java
         * https://developer.android.com/training/sign-in/biometric-auth
         * https://docs.microsoft.com/en-us/xamarin/android/platform/fingerprint-authentication/creating-a-cryptoobject
         */
        private static readonly string KeyAlgorithm = KeyProperties.KeyAlgorithmAes;
        private static readonly string BlockMode = KeyProperties.BlockModeCbc;
        private static readonly string EncryptionPadding = KeyProperties.EncryptionPaddingPkcs7;
        private static readonly string Transfomration = $"{KeyAlgorithm}/{BlockMode}/{EncryptionPadding}";

        private readonly KeyStore _keystore;

        public string KeyName { get; }

        public CryptoObjectHelper(string keyName)
        {
            if (string.IsNullOrEmpty(keyName))
            {
                throw new ArgumentException($"{nameof(keyName)} can't be empty or null");
            }

            KeyName = keyName;
            _keystore = KeyStore.GetInstance(KeyStoreName);
            _keystore.Load(null);
        }

        public BiometricPrompt.CryptoObject BuildCryptoObject()
        {
            var cipher = CreateCipher();
            return new BiometricPrompt.CryptoObject(cipher);
        }

        private Cipher CreateCipher(int retries = 3)
        {
            var key = GetKey();
            var cipher = Cipher.GetInstance(Transfomration);

            try
            {
                cipher.Init(CipherMode.EncryptMode, key);
            }
            catch (KeyPermanentlyInvalidatedException)
            {
                _keystore.DeleteEntry(KeyName);
                if (retries > 0)
                {
                    // Microsoft Docs doesn't overwrite the cipher.
                    // Without the implementation of GetInstance its hard to say if it doesn't need to be overwritten.
                    // So this is a just in case
                    cipher = CreateCipher(--retries);
                }
                else
                {
                    throw new KeyPermanentlyInvalidatedException($"Could not create the cipher for biometric authentication.");
                }
            }

            return cipher;
        }

        private IKey GetKey()
        {
            if (!_keystore.IsKeyEntry(KeyName))
            {
                CreateNewKey();
            }

            return _keystore.GetKey(KeyName, null);
        }

        private void CreateNewKey()
        {
            var keyGen = KeyGenerator.GetInstance(KeyAlgorithm, KeyStoreName);
            var keyGenSpec = new KeyGenParameterSpec.Builder(KeyName, KeyStorePurpose.Encrypt | KeyStorePurpose.Decrypt)
                                    .SetBlockModes(BlockMode)
                                    .SetEncryptionPaddings(EncryptionPadding)
                                    .SetUserAuthenticationRequired(true)
                                    .Build();
            keyGen.Init(keyGenSpec);
            keyGen.GenerateKey();
        }
    }
}
