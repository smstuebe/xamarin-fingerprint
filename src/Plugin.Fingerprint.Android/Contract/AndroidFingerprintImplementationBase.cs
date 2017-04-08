using System.Threading;
using System.Threading.Tasks;
using Plugin.Fingerprint.Abstractions;
using System.Collections.Generic;
using System;

namespace Plugin.Fingerprint.Contract
{
    /// <summary>
    /// Base implementation for the Android implementations.
    /// </summary>
    public abstract class AndroidFingerprintImplementationBase : FingerprintImplementationBase, IAndroidFingerprintImplementation
    {
        protected override async Task<FingerprintAuthenticationResult> NativeAuthenticateAsync(AuthenticationRequestConfiguration authRequestConfig, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (authRequestConfig.UseDialog)
            {
                var fragment = CrossFingerprint.CreateDialogFragment();
                return await fragment.ShowAsync(authRequestConfig, this, cancellationToken);
            }

            return await AuthenticateNoDialogAsync(new DeafAuthenticationFailedListener(), cancellationToken);
        }

        public abstract Task<FingerprintAuthenticationResult> AuthenticateNoDialogAsync(IAuthenticationFailedListener failedListener, CancellationToken cancellationToken);

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