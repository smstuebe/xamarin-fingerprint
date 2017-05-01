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
            if(!await IsAvailableAsync())
                return new FingerprintAuthenticationResult { Status = FingerprintAuthenticationResultStatus.NotAvailable };

            return await NativeAuthenticateAsync(authRequestConfig, cancellationToken);
        }

        public async Task<bool> IsAvailableAsync()
        {
            return await GetAvailabilityAsync() == FingerprintAvailability.Available;
        }

        public abstract Task<FingerprintAvailability> GetAvailabilityAsync();
        protected abstract Task<FingerprintAuthenticationResult> NativeAuthenticateAsync(AuthenticationRequestConfiguration authRequestConfig, CancellationToken cancellationToken);
        protected abstract Task<SecureValueResult> NativeSetSecureValue(SetSecureValueRequestConfiguration setSecureValueRequestConfig, CancellationToken cancellationToken);
        protected abstract Task<SecureValueResult> NativeRemoveSecureValue(SecureValueRequestConfiguration secureValueRequestConfig, CancellationToken cancellationToken);
        protected abstract Task<GetSecureValueResult> NativeGetSecureValue(SecureValueRequestConfiguration secureValueRequestConfig, CancellationToken cancellationToken);

        public async Task<SecureValueResult> SetSecureValue(SetSecureValueRequestConfiguration setSecureValueRequestConfig, CancellationToken cancellationToken = default(CancellationToken))
        {          
            if (!await IsAvailableAsync())
                return new SecureValueResult { Status = FingerprintAuthenticationResultStatus.NotAvailable };

            return await NativeSetSecureValue(setSecureValueRequestConfig, cancellationToken);
        }

        public async Task<SecureValueResult> RemoveSecureValue(SecureValueRequestConfiguration secureValueRequestConfig, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (!await IsAvailableAsync())
                return new SecureValueResult { Status = FingerprintAuthenticationResultStatus.NotAvailable };

            return await NativeRemoveSecureValue(secureValueRequestConfig, cancellationToken);
        }

        public async Task<GetSecureValueResult> GetSecureValue(SecureValueRequestConfiguration secureValueRequestConfig, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (!await IsAvailableAsync())
                return new GetSecureValueResult { Status = FingerprintAuthenticationResultStatus.NotAvailable };

            return await NativeGetSecureValue(secureValueRequestConfig, cancellationToken);
        }
    }
}