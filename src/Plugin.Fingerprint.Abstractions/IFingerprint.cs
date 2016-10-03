using System.Threading;
using System.Threading.Tasks;

namespace Plugin.Fingerprint.Abstractions
{
    public interface IFingerprint
    {
        /// <summary>
        /// Checks the availability of fingerprint authentication.
        /// Checks are performed in this order:
        /// 1. API supports accessing the fingerprint sensor
        /// 2. Permission for accessint the fingerprint sensor granted
        /// 3. Device has sensor
        /// 4. Fingerprint has been enrolled
        /// <see cref="FingerprintAvailability.Unknown"/> will be returned if the check failed 
        /// with some other platform specific reason.
        /// </summary>
        Task<FingerprintAvailability> GetAvailabilityAsync();

        /// <summary>
        /// Checks if <see cref="GetAvailabilityAsync"/> returns <see cref="FingerprintAvailability.Available"/>.
        /// </summary>
        /// <returns><c>true</c> if Available, else <c>false</c></returns>
        Task<bool> IsAvailableAsync();

        /// <summary>
        /// Requests the authentication.
        /// </summary>
        /// <param name="reason">Reason for the fingerprint authentication request. Displayed to the user.</param>
        /// <param name="cancellationToken">Token used to cancel the operation.</param>
        /// <returns>Authentication result</returns>
        Task<FingerprintAuthenticationResult> AuthenticateAsync(string reason, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Requests the authentication.
        /// </summary>
        /// <param name="authRequestConfig">Configuration of the dialog that is displayed to the user.</param>
        /// <param name="cancellationToken">Token used to cancel the operation.</param>
        /// <returns>Authentication result</returns>
        Task<FingerprintAuthenticationResult> AuthenticateAsync(AuthenticationRequestConfiguration authRequestConfig, CancellationToken cancellationToken = default(CancellationToken));
    }
}