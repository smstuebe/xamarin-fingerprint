using System.Runtime.Versioning;

namespace Plugin.Fingerprint.Abstractions
{
    /// <summary>
    /// Configuration of the stuff presented to the user.
    /// </summary>
#if NET6_0_ANDROID
    [SupportedOSPlatform("android")]
#elif NET6_0_IOS
    [SupportedOSPlatform("ios")]
#elif NET6_0_MACCATALYST
    [SupportedOSPlatform("maccatalyst")]
#endif
    public class AuthenticationRequestConfiguration
    {
        /// <summary>
        /// Title of the authentication request.
        /// </summary>
        public string Title { get; }

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
        /// Shown when a recoverable error has been encountered during authentication. 
        /// The help strings are provided to give the user guidance for what went wrong.
        /// If a string is null or empty, the string provided by Android is shown.
        /// 
        /// Supported Platforms: Android
        /// </summary>
        public AuthenticationHelpTexts HelpTexts { get; }

        /// <summary>
        /// En-/Disables the use of the PIN / Password as fallback.
        /// 
        /// Supported Platforms: Android, iOS, Mac
        /// Default: false
        /// </summary>
        public bool AllowAlternativeAuthentication { get; set; } = false;

        /// <summary>
        /// Sets a hint to the system for whether to require user confirmation after authentication.
        /// For example, implicit modalities like face and iris are passive, meaning they don't require an explicit user action to complete authentication.
        /// If set to true, these modalities should require the user to take some action (e.g. press a button) before AuthenticateAsync() returns.
        ///
        /// Supported Platforms: Android
        /// Default: true
        /// </summary>
        public bool ConfirmationRequired { get; set; } = true;

        public AuthenticationRequestConfiguration(string title, string reason)
        {
            Reason = reason;
            Title = title;
            HelpTexts = new AuthenticationHelpTexts();
        }
    }

#if NET6_0_ANDROID
    [SupportedOSPlatform("android")]
#elif NET6_0_IOS
    [SupportedOSPlatform("ios")]
#elif NET6_0_MACCATALYST
    [SupportedOSPlatform("maccatalyst")]
#endif
    public class AuthenticationHelpTexts
    {
        /// <summary>
        /// The fingerprint image was incomplete due to quick motion.
        /// </summary>
        public string MovedTooFast { get; set; }

        /// <summary>
        /// The fingerprint image was unreadable due to lack of motion.
        /// </summary>
        public string MovedTooSlow { get; set; }

        /// <summary>
        /// Only a partial fingerprint image was detected.
        /// </summary>
        public string Partial { get; set; }

        /// <summary>
        /// The fingerprint image was too noisy to process due to a detected condition.
        /// </summary>
        public string Insufficient { get; set; }

        /// <summary>
        /// The fingerprint image was too noisy due to suspected or detected dirt on the sensor.
        /// </summary>
        public string Dirty { get; set; }
    }
}