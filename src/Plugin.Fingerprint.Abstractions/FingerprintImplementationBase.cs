using System;
using System.Collections.Generic;
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
        protected abstract Task<SecureValueResult> NativeSetSecureValue(string serviceId, KeyValuePair<string, string> value);
        protected abstract Task<SecureValueResult> NativeRemoveSecureValue(string serviceId, string key);
        protected abstract Task<GetSecureValueResult> NativeGetSecureValue(string serviceId, string key, string reason);

        public async Task<SecureValueResult> SetSecureValue(string serviceId, KeyValuePair<string, string> value)
        {
            if (serviceId == null)
                throw new ArgumentNullException(nameof(serviceId));

            if (string.IsNullOrEmpty(value.Key) || string.IsNullOrEmpty(value.Value))
                throw new ArgumentNullException(nameof(value), "Key or value cannot be empty");            

            if (!await IsAvailableAsync())
                return new SecureValueResult { Status = SecureValueResultStatus.NotAvailable };

            return await NativeSetSecureValue(serviceId, value);
        }

        public async Task<SecureValueResult> RemoveSecureValue(string serviceId, string key)
        {
            if (serviceId == null)
                throw new ArgumentNullException(nameof(serviceId));

            if (key == null)
                throw new ArgumentNullException(nameof(key));            

            if (!await IsAvailableAsync())
                return new SecureValueResult { Status = SecureValueResultStatus.NotAvailable };

            return await NativeRemoveSecureValue(serviceId, key);
        }

        public async Task<GetSecureValueResult> GetSecureValue(string serviceId, string key, string reason)
        {
            if (serviceId == null)
                throw new ArgumentNullException(nameof(serviceId));

            if (key == null)
                throw new ArgumentNullException(nameof(key));

            if (reason == null)
                throw new ArgumentNullException(nameof(reason));

            if (!await IsAvailableAsync())
                return new GetSecureValueResult { Status = SecureValueResultStatus.NotAvailable };

            return await NativeGetSecureValue(serviceId, key, reason);
        }
    }
}