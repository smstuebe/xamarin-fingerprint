using System.Runtime.Versioning;

namespace Plugin.Fingerprint.Abstractions
{
#if NET6_0_ANDROID
    [SupportedOSPlatform("android")]
#elif NET6_0_IOS
    [SupportedOSPlatform("ios")]
#elif NET6_0_MACCATALYST
    [SupportedOSPlatform("maccatalyst")]
#endif
    public class FingerprintAuthenticationResult
    {
        /// <summary>
        /// Indicatates whether the authentication was successful or not.
        /// </summary>
        public bool Authenticated { get { return Status == FingerprintAuthenticationResultStatus.Succeeded; } }

        /// <summary>
        /// Detailed information of the authentication.
        /// </summary>
        public FingerprintAuthenticationResultStatus Status { get; set; }

        /// <summary>
        /// Reason for the unsucessful authentication.
        /// </summary>
        public string ErrorMessage { get; set; }
    }
}