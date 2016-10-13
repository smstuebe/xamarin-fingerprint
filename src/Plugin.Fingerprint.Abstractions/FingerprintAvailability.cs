namespace Plugin.Fingerprint.Abstractions
{
    /// <summary>
    /// Indicates if a fingerprint authentication can be performed.
    /// </summary>
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
        /// An unknown, platform specific error occurred. Availability status could not be 
        /// mapped to a <see cref="FingerprintAvailability"/>.
        /// </summary>
        Unknown
    }
}