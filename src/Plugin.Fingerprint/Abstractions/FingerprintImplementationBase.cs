using System;
using System.Threading;
using System.Threading.Tasks;

namespace Plugin.Fingerprint.Abstractions
{
    public abstract class FingerprintImplementationBase : IFingerprint
    {
        public async Task<FingerprintAuthenticationResult> AuthenticateAsync(AuthenticationRequestConfiguration authRequestConfig, CancellationToken cancellationToken = default)
        {
            if (authRequestConfig is null)
                throw new ArgumentNullException(nameof(authRequestConfig));

            var availability = await GetAvailabilityAsync(authRequestConfig.AllowAlternativeAuthentication);
            if (availability != FingerprintAvailability.Available)
            {
                var status = availability == FingerprintAvailability.Denied ?
                    FingerprintAuthenticationResultStatus.Denied :
                    FingerprintAuthenticationResultStatus.NotAvailable;

                return new FingerprintAuthenticationResult { Status = status };
            }

            return await NativeAuthenticateAsync(authRequestConfig, cancellationToken);
        }

        public async Task<bool> IsAvailableAsync(bool allowAlternativeAuthentication = false)
        {
            return await GetAvailabilityAsync(allowAlternativeAuthentication) == FingerprintAvailability.Available;
        }

        public abstract Task<FingerprintAvailability> GetAvailabilityAsync(bool allowAlternativeAuthentication = false);

        public abstract Task<AuthenticationType> GetAuthenticationTypeAsync();

        protected abstract Task<FingerprintAuthenticationResult> NativeAuthenticateAsync(AuthenticationRequestConfiguration authRequestConfig, CancellationToken cancellationToken);
    }
}