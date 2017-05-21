namespace Plugin.Fingerprint.Abstractions
{
    /// <summary>
    /// Configuration of the stuff presented to the user.
    /// </summary>
    public class AuthenticationRequestConfiguration
    {
        /// <summary>
        /// Reason of the authentication request.
        /// </summary>
        public string Reason { get; }

        /// <summary>
        /// Title of the cancel button.
        /// </summary>
        public string CancelTitle { get; set; }

        /// <summary>
        /// Title of the fallback button.
        /// </summary>
        public string FallbackTitle { get; set; }

        /// <summary>
        /// En-/Disables the dialog. 
        /// Supported Platforms: Android
        /// Default: true
        /// </summary>
        public bool UseDialog { get; set; } = true;

        /// <summary>
        /// En-/Disables the use of the PIN / Passwort as fallback.
        /// Supported Platforms: iOS, Mac
        /// Default: false
        /// </summary>
        public bool AllowAlternativeAuthentication { get; set; } = false;

        public AuthenticationRequestConfiguration(string reason)
        {
            Reason = reason;
        }
    }
}