using System.Threading;
using System.Threading.Tasks;
using Plugin.Fingerprint.Abstractions;

namespace Plugin.Fingerprint
{
    internal class FingerprintImplementation : FingerprintImplementationBase
    {
        public override async Task<FingerprintAvailability> GetAvailabilityAsync(bool allowAlternativeAuthentication = false)
        {
            return FingerprintAvailability.NoImplementation;
        }

        protected override Task<FingerprintAuthenticationResult> NativeAuthenticateAsync(AuthenticationRequestConfiguration authRequestConfig, CancellationToken cancellationToken = new CancellationToken())
        {
            return Task.FromResult(new FingerprintAuthenticationResult
            {
                Status = FingerprintAuthenticationResultStatus.NotAvailable,
                ErrorMessage = "Not implemented for the current platform."
            });
        }
    }
}