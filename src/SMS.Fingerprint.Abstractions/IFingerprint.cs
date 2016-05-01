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
        /// <returns>Authentication result</returns>
        Task<FingerprintAuthenticationResult> AuthenticateAsync(string reason);

        /// <summary>
        /// Requests the authentication (cancelable).
        /// </summary>
        /// <see cref="AuthenticateAsync(string)"/>
        Task<FingerprintAuthenticationResult> AuthenticateAsync(string reason, CancellationToken cancellationToken);
    }
}
