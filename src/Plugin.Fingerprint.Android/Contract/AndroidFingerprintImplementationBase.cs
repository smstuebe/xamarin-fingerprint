using System.Threading;
using System.Threading.Tasks;
using Plugin.Fingerprint.Abstractions;

namespace Plugin.Fingerprint.Contract
{
    /// <summary>
    /// Base implementation for the Android implementations.
    /// </summary>
    public abstract class AndroidFingerprintImplementationBase : FingerprintImplementationBase, IAndroidFingerprintImplementation
    {
        private readonly DeviceAuthImplementation _deviceAuth;

        public AndroidFingerprintImplementationBase()
        {
            _deviceAuth = new DeviceAuthImplementation();
        }

        protected override async Task<FingerprintAuthenticationResult> NativeAuthenticateAsync(AuthenticationRequestConfiguration authRequestConfig, CancellationToken cancellationToken = default(CancellationToken))
        {
            FingerprintAuthenticationResult result = new FingerprintAuthenticationResult { Status = FingerprintAuthenticationResultStatus.NotAvailable };

            if (await GetAvailabilityAsync() == FingerprintAvailability.Available)
            {
				if (authRequestConfig.UseDialog)
				{
					var fragment = CrossFingerprint.CreateDialogFragment();
					result = await fragment.ShowAsync(authRequestConfig, this, cancellationToken);
				}
				else
				{
					result = await AuthenticateNoDialogAsync(new DeafAuthenticationFailedListener(), cancellationToken);
				}
            }

            if(authRequestConfig.AllowAlternativeAuthentication && result.Status != FingerprintAuthenticationResultStatus.Succeeded)
            {
                if(_deviceAuth.IsDeviceAuthSetup())
                {
                    result = await _deviceAuth.AuthenticateAsync();
                }
                else
                {
                    result = new FingerprintAuthenticationResult
                    {
                        Status = FingerprintAuthenticationResultStatus.NotAvailable,
                        ErrorMessage = "Fallback authentication not available"
                    };
                }
            }

            return result;
        }

        public abstract Task<FingerprintAuthenticationResult> AuthenticateNoDialogAsync(IAuthenticationFailedListener failedListener, CancellationToken cancellationToken);
    }
}