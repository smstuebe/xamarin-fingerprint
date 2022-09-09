using Android.App;

namespace Plugin.Fingerprint.Platforms.Android.Utils
{
    public class CryptoSettings
    {
        public string CryptoKeyName { get; }

        public byte[] CipherSecretBytes { get; }

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

            var r = new Java.Security.SecureRandom();
            var secret = new byte[128];
            r.NextBytes(secret);
            this.CipherSecretBytes = secret;
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
