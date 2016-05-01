using System.Threading;
using System.Threading.Tasks;
using Plugin.Fingerprint.Abstractions;

namespace Plugin.Fingerprint
{
    internal class FingerprintImplementation : IFingerprint
    {
        public bool IsAvailable { get; } = false;
        public Task<FingerprintAuthenticationResult> AuthenticateAsync(string reason)
        {
            return AuthenticateAsync(reason, CancellationToken.None);
        }

        public Task<FingerprintAuthenticationResult> AuthenticateAsync(string reason, CancellationToken cancellationToken)
        {
            return Task.FromResult(new FingerprintAuthenticationResult
            {
                Status = FingerprintAuthenticationResultStatus.NotAvailable,
                ErrorMessage = "Not implemented for the current platform"
            });
        }
    }
}