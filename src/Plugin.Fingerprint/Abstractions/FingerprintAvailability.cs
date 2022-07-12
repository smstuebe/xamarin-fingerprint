using System.Runtime.Versioning;

namespace Plugin.Fingerprint.Abstractions
{
    /// <summary>
    /// Indicates if a fingerprint authentication can be performed.
    /// </summary>
#if NET6_0_ANDROID || NET6_0_IOS || NET6_0_MACCATALYST
    [SupportedOSPlatform("android")]
    [SupportedOSPlatform("ios")]
    [SupportedOSPlatform("maccatalyst")]
#endif
    public enum FingerprintAvailability
    {
        /// <summary>
        /// Fingerprint authentication can be used.
        /// </summary>
        Available,
        /// <summary>
        /// This plugin has no implementation for the current platform.
        /// </summary>
        NoImplementation,
        /// <summary>
        /// Operating system has no supported fingerprint API.
        /// </summary>
        NoApi,
        /// <summary>
        /// App is not allowed to access the fingerprint sensor.
        /// </summary>
        NoPermission,
        /// <summary>
        /// Device has no fingerprint sensor.
        /// </summary>
        NoSensor,
        /// <summary>
        /// Fingerprint has not been set up.
        /// </summary>
        NoFingerprint,
        /// <summary>
        /// Fallback has not been set up.
        /// </summary>
        NoFallback,
        /// <summary>
        /// An unknown, platform specific error occurred. Availability status could not be 
        /// mapped to a <see cref="FingerprintAvailability"/>.
        /// </summary>
        Unknown,
        /// <summary>
        /// User has denied the usage of the biometric authentication.
        /// </summary>
        Denied
    }
}