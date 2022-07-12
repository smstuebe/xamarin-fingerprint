using System.Runtime.Versioning;
using System.Threading;
using System.Threading.Tasks;

namespace Plugin.Fingerprint.Abstractions
{
#if NET6_0_ANDROID
    [SupportedOSPlatform("android")]
#elif NET6_0_IOS
    [SupportedOSPlatform("ios")]
#elif NET6_0_MACCATALYST
    [SupportedOSPlatform("maccatalyst")]
#endif
    public interface IFingerprint
    {
        /// <summary>
        /// Checks the availability of fingerprint authentication.
        /// Checks are performed in this order:
        /// 1. API supports accessing the fingerprint sensor
        /// 2. Permission for accessing the fingerprint sensor granted
        /// 3. Device has sensor
        /// 4. Fingerprint has been enrolled
        /// <see cref="FingerprintAvailability.Unknown"/> will be returned if the check failed 
        /// with some other platform specific reason.
        /// </summary>
        /// <param name="allowAlternativeAuthentication">
        /// En-/Disables the use of the PIN / Passwort as fallback.
        /// Supported Platforms: iOS, Mac
        /// Default: false
        /// </param>
        Task<FingerprintAvailability> GetAvailabilityAsync(bool allowAlternativeAuthentication = false);

        /// <summary>
        /// Checks if <see cref="GetAvailabilityAsync"/> returns <see cref="FingerprintAvailability.Available"/>.
        /// </summary>
        /// <param name="allowAlternativeAuthentication">
        /// En-/Disables the use of the PIN / Passwort as fallback.
        /// Supported Platforms: iOS, Mac
        /// Default: false
        /// </param>
        /// <returns><c>true</c> if Available, else <c>false</c></returns>
        Task<bool> IsAvailableAsync(bool allowAlternativeAuthentication = false);

        /// <summary>
        /// Requests the authentication.
        /// </summary>
        /// <param name="authRequestConfig">Configuration of the dialog that is displayed to the user.</param>
        /// <param name="cancellationToken">Token used to cancel the operation.</param>
        /// <returns>Authentication result</returns>
        Task<FingerprintAuthenticationResult> AuthenticateAsync(AuthenticationRequestConfiguration authRequestConfig, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets the available authentication type.
        /// </summary>
        /// <returns>Authentication type</returns>
        Task<AuthenticationType> GetAuthenticationTypeAsync();
    }
}