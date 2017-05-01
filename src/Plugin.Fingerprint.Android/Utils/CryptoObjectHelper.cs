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

using Android.Security.Keystore;
using Android.Support.V4.Hardware.Fingerprint;
using Android.Util;
using Java.Security;
using Javax.Crypto;
using Javax.Crypto.Spec;

namespace Plugin.Fingerprint.Utils
{
    /// <summary>
    ///     This class encapsulates the creation of a CryptoObject based on a javax.crypto.Cipher.
    /// </summary>
    /// <remarks>Each invocation of BuildCryptoObject will instantiate a new CryptoObjet. 
    /// If necessary a key for the cipher will be created.</remarks>
    internal class CryptoObjectHelper
    {
        // ReSharper disable InconsistentNaming
        private static readonly string TAG = "X:" + typeof(CryptoObjectHelper).Name;
         
        private static readonly string KEY_NAME = "Plugin.Fingerprint.Android";
        private static readonly string KEYSTORE_NAME = "AndroidKeyStore";
         
        private static readonly string KEY_ALGORITHM = KeyProperties.KeyAlgorithmAes;
        private static readonly string BLOCK_MODE = KeyProperties.BlockModeCbc;
        private static readonly string ENCRYPTION_PADDING = KeyProperties.EncryptionPaddingPkcs7;
         
        private static readonly string TRANSFORMATION = KEY_ALGORITHM + "/" +
                                       BLOCK_MODE + "/" +
                                       ENCRYPTION_PADDING;
        // ReSharper restore InconsistentNaming

        private readonly KeyStore _keystore;

        public static CryptoObjectHelper Instance { get; private set; } = new CryptoObjectHelper();

        public CryptoObjectHelper()
        {
            _keystore = KeyStore.GetInstance(KEYSTORE_NAME);
            _keystore.Load(null);
        }

        public FingerprintManagerCompat.CryptoObject BuildCryptoObject(CipherMode cipherMode, byte[] iv = null)
        {
            if (cipherMode == CipherMode.UnwrapMode) throw new NotSupportedException();
            if (cipherMode == CipherMode.WrapMode) throw new NotSupportedException();
            if (cipherMode == CipherMode.DecryptMode && iv == null) throw new InvalidOperationException("iv must be supplied for CipherMode.DecryptMode");

            Cipher cipher = CreateCipher(cipherMode, iv);
            return new FingerprintManagerCompat.CryptoObject(cipher);
        }

        /// <summary>
        ///     Creates the cipher.
        /// </summary>
        /// <returns>The cipher.</returns>
        /// <param name="retry">If set to <c>true</c>, recreate the key and try again.</param>        
        Cipher CreateCipher(CipherMode cipherMode, byte[] iv = null, bool retry = true)
        {
            IKey key = GetKey();
            Cipher cipher = Cipher.GetInstance(TRANSFORMATION);

            if (cipherMode == CipherMode.EncryptMode)
            {
                try
                {
                    cipher.Init(cipherMode, key);
                }
                catch (KeyPermanentlyInvalidatedException e)
                {
                    Log.Debug(TAG, "The key was invalidated, creating a new key.");
                    _keystore.DeleteEntry(KEY_NAME);
                    if (retry)
                    {
                        CreateCipher(cipherMode, null, false);
                    }
                    else
                    {
                        throw new Exception("Could not create the cipher for fingerprint authentication.", e);
                    }
                }
            }
            else if (cipherMode == CipherMode.DecryptMode)
            {
                cipher.Init(cipherMode, key, new IvParameterSpec(iv));
            }

            return cipher;
        }

        /// <summary>
        ///     Will get the key from the Android keystore, creating it if necessary.
        /// </summary>
        /// <returns></returns>
        IKey GetKey()
        {
            if (!_keystore.IsKeyEntry(KEY_NAME))
            {
                CreateKey();
            }

            IKey secretKey = _keystore.GetKey(KEY_NAME, null);
            return secretKey;
        }

        /// <summary>
        ///     Creates the Key for fingerprint authentication.
        /// </summary>
        void CreateKey()
        {
            KeyGenerator keyGen = KeyGenerator.GetInstance(KeyProperties.KeyAlgorithmAes, KEYSTORE_NAME);
            KeyGenParameterSpec keyGenSpec =
                new KeyGenParameterSpec.Builder(KEY_NAME, KeyStorePurpose.Encrypt | KeyStorePurpose.Decrypt)
                    .SetBlockModes(BLOCK_MODE)
                    .SetEncryptionPaddings(ENCRYPTION_PADDING)
                    .SetUserAuthenticationRequired(true)
                    .Build();
            keyGen.Init(keyGenSpec);
            keyGen.GenerateKey();
            Log.Debug(TAG, "New key created for fingerprint authentication.");
        }
    }
}