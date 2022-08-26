using Android.App;

namespace Plugin.Fingerprint.Platforms.Android.Utils
{
    public class CryptoSettings
    {
        public string CryptoKeyName { get; }

        public byte[] CipherSecretBytes { get; }

        /// <summary>
        /// When disabled, manipulating the CryptoObject when verifying the result would valid and return a successful authorization 
        /// </summary>
        public bool EnforceCryptoObject { get; set; } = true;

        /// <summary>
        /// Validate the resulting Cipher to the provided one. CryptoObject can be changed as long as we result in the same valid cipher
        /// </summary>
        public bool ValidatedCipher { get; set; } = true;

        /// <summary>
        /// Data used for en-/decryption of authentication data
        /// </summary>
        /// <param name="cryptoKeyName">Key Name under which the key gets stored. Should be unique to the app</param>
        /// <param name="cipherSecretBytes">Cipher Secret Bytes used to validate cipher result. Should be unique to the app</param>
        public CryptoSettings(string cryptoKeyName, byte[] cipherSecretBytes)
        {
            this.CryptoKeyName = cryptoKeyName;
            this.CipherSecretBytes = cipherSecretBytes;
        }

        /// <summary>
        /// Data used for en-/decryption of authentication data
        /// No secret bytes will be used for cipher validation
        /// </summary>
        /// <param name="cryptoKeyName">Key Name under which the key gets stored. Should be unique to the app</param>
        public CryptoSettings(string cryptoKeyName)
        {
            this.CryptoKeyName = cryptoKeyName;
        }

        /// <summary>
        /// Data used for en-/decryption of authentication data
        /// Key Name will be automatically set to "{packagename}_plugin_fingerprint_authentication_key"
        /// </summary>
        /// <param name="cipherSecretBytes">Cipher Secret Bytes used to validate cipher result. Should be unique to the app</param>
        public CryptoSettings(byte[] cipherSecretBytes)
            : this()
        {
            this.CipherSecretBytes = cipherSecretBytes;
        }

        /// <summary>
        /// Data used for en-/decryption of authentication data
        /// Key Name will be automatically set to "{packagename}_plugin_fingerprint_authentication_key"
        /// No secret bytes will be used for cipher validation
        /// </summary>
        public CryptoSettings()
        {
            this.CryptoKeyName = Application.Context.PackageName.ToLower() + "_plugin_fingerprint_authentication_key";
        }
    }
}
