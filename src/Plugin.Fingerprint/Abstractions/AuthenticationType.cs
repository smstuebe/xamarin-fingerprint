using System.Runtime.Versioning;

namespace Plugin.Fingerprint.Abstractions
{
    /// <summary>
    /// The types of biometric authentication supported.
    /// </summary>
#if NET6_0_ANDROID
    [SupportedOSPlatform("android")]
#elif NET6_0_IOS
    [SupportedOSPlatform("ios")]
#elif NET6_0_MACCATALYST
    [SupportedOSPlatform("maccatalyst")]
#endif
    public enum AuthenticationType
    {
        /// <summary>
        /// None.
        /// </summary>
        None,

        /// <summary>
        /// Device supports fingerprint.
        /// </summary>
        Fingerprint,

        /// <summary>
        /// Device supports face detection.
        /// </summary>
        Face
    }
}