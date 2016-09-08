using System.Threading;
using System.Threading.Tasks;

namespace Plugin.Fingerprint.Abstractions
{
    public interface IFingerprint
    {
        /// <summary>
        /// Checks the availability of fingerprint authentication.
        /// Possible Reasons for unavailability:
        /// - Device has no sensor
        /// - OS does not support this functionality
        /// - Fingerprint is not enrolled
        /// </summary>
        bool IsAvailable { get; }

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
        /// <param name="dialogConfig">Configuration of the dialog that is displayed to the user.</param>
        /// <param name="cancellationToken">Token used to cancel the operation.</param>
        /// <returns>Authentication result</returns>
        Task<FingerprintAuthenticationResult> AuthenticateAsync(DialogConfiguration dialogConfig, CancellationToken cancellationToken = default(CancellationToken));
    }
}
