using System;
using Android;
using Android.Content.PM;
using Android.Hardware.Biometrics;
using Android.OS;
using Plugin.Fingerprint.Abstractions;
using System.Threading;
using System.Threading.Tasks;
using Android.App;

namespace Plugin.Fingerprint.Contract
{
    /// <summary>
    /// Base implementation for the Android implementations.
    /// </summary>
    public class FingerprintImplementation : FingerprintImplementationBase
    {
        public FingerprintImplementation()
        {
            // _manager = new BiometricManager(); // Android 10 -.-
        }

        public override async Task<AuthenticationType> GetAuthenticationTypeAsync()
        {
            var availability = await GetAvailabilityAsync(false);
            if (availability == FingerprintAvailability.NoFingerprint ||
                availability == FingerprintAvailability.NoPermission ||
                availability == FingerprintAvailability.Available)
            {
                return AuthenticationType.Fingerprint;
            }

            return AuthenticationType.None;
        }

        public override async Task<FingerprintAvailability> GetAvailabilityAsync(bool allowAlternativeAuthentication = false)
        {
            if (Build.VERSION.SdkInt < BuildVersionCodes.M)
                return FingerprintAvailability.NoApi;

            var context = Application.Context;
            if (context.CheckCallingOrSelfPermission(Manifest.Permission.UseBiometric) != Permission.Granted)
                return FingerprintAvailability.NoPermission;

            return FingerprintAvailability.Available;
        }

        protected override async Task<FingerprintAuthenticationResult> NativeAuthenticateAsync(AuthenticationRequestConfiguration authRequestConfig, CancellationToken cancellationToken = default)
        {
            if(string.IsNullOrWhiteSpace(authRequestConfig.Title))
                throw new ArgumentException("Title must not be null or empty on Android.", nameof(authRequestConfig.Title));

            using (var cancellationSignal = new CancellationSignal())
            using (cancellationToken.Register(() => cancellationSignal.Cancel()))
            {
                var cancel = string.IsNullOrWhiteSpace(authRequestConfig.CancelTitle) ? 
                    Application.Context.GetString(Android.Resource.String.Cancel) : 
                    authRequestConfig.CancelTitle;
                var handler = new AuthenticationHandler();
                var dialog = new BiometricPrompt.Builder(CrossFingerprint.CurrentActivity)
                  .SetTitle(authRequestConfig.Title)
                  .SetDescription(authRequestConfig.Reason)
                  .SetNegativeButton(cancel, CrossFingerprint.CurrentActivity.MainExecutor, handler)
                  .Build();

                dialog.Authenticate(cancellationSignal, CrossFingerprint.CurrentActivity.MainExecutor, handler);
                return await handler.GetTask();
            }
        }
    }
}