using System.Threading;
using System.Threading.Tasks;
using Plugin.Fingerprint.Abstractions;
using System.Collections.Generic;

namespace Plugin.Fingerprint
{
    internal class FingerprintImplementation : FingerprintImplementationBase
    {
        public override async Task<FingerprintAvailability> GetAvailabilityAsync()
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

        protected override Task<SecureValueResult> NativeSetSecureValue(SetSecureValueRequestConfiguration setSecureValueRequestConfig, CancellationToken cancellationToken)
        {
            return Task.FromResult(new SecureValueResult
            {
                Status = FingerprintAuthenticationResultStatus.NotAvailable,
                ErrorMessage = "Not implemented for the current platform."
            });
        }

        protected override Task<SecureValueResult> NativeRemoveSecureValue(SecureValueRequestConfiguration secureValueRequestConfig, CancellationToken cancellationToken)
        {
            return Task.FromResult(new SecureValueResult
            {
                Status = FingerprintAuthenticationResultStatus.NotAvailable,
                ErrorMessage = "Not implemented for the current platform."
            });
        }

        protected override Task<GetSecureValueResult> NativeGetSecureValue(SecureValueRequestConfiguration secureValueRequestConfig, CancellationToken cancellationToken)
        {
            return Task.FromResult(new GetSecureValueResult
            {
                Status = FingerprintAuthenticationResultStatus.NotAvailable,
                ErrorMessage = "Not implemented for the current platform."
            });
        }
    }
}