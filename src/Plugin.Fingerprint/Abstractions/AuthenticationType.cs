namespace Plugin.Fingerprint.Abstractions
{
    /// <summary>
    /// The types of biometric authentication supported.
    /// </summary>
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