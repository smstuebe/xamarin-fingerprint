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
        /// En-/Disables the dialog. Disabling the dialog is only supported on Android.
        /// Default: true
        /// </summary>
        public bool UseDialog { get; set; } = true;

        public AuthenticationRequestConfiguration(string reason)
        {
            Reason = reason;
        }
    }
}