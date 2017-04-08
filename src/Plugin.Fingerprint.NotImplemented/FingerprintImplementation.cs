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

        protected override Task<SecureValueResult> NativeSetSecureValue(string serviceId, KeyValuePair<string, string> value)
        {
            return Task.FromResult(new SecureValueResult
            {
                Status = SecureValueResultStatus.NotAvailable,
                ErrorMessage = "Not implemented for the current platform."
            });
        }

        protected override Task<SecureValueResult> NativeRemoveSecureValue(string serviceId, string key)
        {
            return Task.FromResult(new SecureValueResult
            {
                Status = SecureValueResultStatus.NotAvailable,
                ErrorMessage = "Not implemented for the current platform."
            });
        }

        protected override Task<GetSecureValueResult> NativeGetSecureValue(string serviceId, string key, string reason)
        {
            return Task.FromResult(new GetSecureValueResult
            {
                Status = SecureValueResultStatus.NotAvailable,
                ErrorMessage = "Not implemented for the current platform."
            });
        }
    }
}