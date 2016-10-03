using System.Threading;
using System.Threading.Tasks;

namespace Plugin.Fingerprint.Abstractions
{
    public abstract class FingerprintImplementationBase : IFingerprint
    {
        public Task<FingerprintAuthenticationResult> AuthenticateAsync(string reason, CancellationToken cancellationToken = default(CancellationToken))
        {
            return AuthenticateAsync(new AuthenticationRequestConfiguration(reason), cancellationToken);
        }

        public async Task<FingerprintAuthenticationResult> AuthenticateAsync(AuthenticationRequestConfiguration authRequestConfig, CancellationToken cancellationToken = new CancellationToken())
        {
            if(await IsAvailableAsync())
                return new FingerprintAuthenticationResult { Status = FingerprintAuthenticationResultStatus.NotAvailable };

            return await NativeAuthenticateAsync(authRequestConfig, cancellationToken);
        }

        public async Task<bool> IsAvailableAsync()
        {
            return await GetAvailabilityAsync() == FingerprintAvailability.Available;
        }

        public abstract Task<FingerprintAvailability> GetAvailabilityAsync();
        protected abstract Task<FingerprintAuthenticationResult> NativeAuthenticateAsync(AuthenticationRequestConfiguration authRequestConfig, CancellationToken cancellationToken);
    }
}