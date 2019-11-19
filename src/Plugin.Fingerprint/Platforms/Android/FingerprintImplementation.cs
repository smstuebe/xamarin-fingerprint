using System;
using Android.OS;
using Plugin.Fingerprint.Abstractions;
using System.Threading;
using System.Threading.Tasks;
using Android;
using Android.App;
using Android.Content.PM;
using AndroidX.Biometric;
using AndroidX.Fragment.App;
using Java.Util.Concurrent;

namespace Plugin.Fingerprint.Contract
{
    /// <summary>
    /// Base implementation for the Android implementations.
    /// </summary>
    public class FingerprintImplementation : FingerprintImplementationBase
    {
        private readonly BiometricManager _manager;

        public FingerprintImplementation()
        {
            _manager = BiometricManager.From(Application.Context);
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
            
            if (context.CheckCallingOrSelfPermission(Manifest.Permission.UseBiometric) != Permission.Granted &&
                context.CheckCallingOrSelfPermission(Manifest.Permission.UseFingerprint) != Permission.Granted)
                return FingerprintAvailability.NoPermission;

            var result = _manager.CanAuthenticate();

            switch (result)
            {
                case BiometricManager.BiometricErrorNoHardware:
                    return FingerprintAvailability.NoSensor;
                case BiometricManager.BiometricErrorHwUnavailable:
                    return FingerprintAvailability.Unknown;
                case BiometricManager.BiometricErrorNoneEnrolled:
                    return FingerprintAvailability.NoFingerprint;
                case BiometricManager.BiometricSuccess:
                    return FingerprintAvailability.Available;
            }

            return FingerprintAvailability.Unknown;
        }

        protected override async Task<FingerprintAuthenticationResult> NativeAuthenticateAsync(AuthenticationRequestConfiguration authRequestConfig, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(authRequestConfig.Title))
                throw new ArgumentException("Title must not be null or empty on Android.", nameof(authRequestConfig.Title));

            if(!(CrossFingerprint.CurrentActivity is FragmentActivity)) 
                throw new InvalidOperationException($"Expected current activity to be '{typeof(FragmentActivity).FullName}' but was '{CrossFingerprint.CurrentActivity?.GetType().FullName}'. " +
                                                    "You need to use AndroidX. Have you installed Xamarin.AndroidX.Migration in your Android App project!?");

            try
            {
                using(var cancellationSignal = new CancellationSignal())
                using (cancellationToken.Register(() => cancellationSignal.Cancel()))
                {
                    var cancel = string.IsNullOrWhiteSpace(authRequestConfig.CancelTitle) ?
                        Application.Context.GetString(Android.Resource.String.Cancel) :
                        authRequestConfig.CancelTitle;

                    var handler = new AuthenticationHandler();
                    var builder = new BiometricPrompt.PromptInfo.Builder()
                        .SetTitle(authRequestConfig.Title)
                        .SetDescription(authRequestConfig.Reason);

                    if (authRequestConfig.AllowAlternativeAuthentication)
                    {
                        // It's not allowed to allow alternative auth & set the negative button
                        builder = builder.SetDeviceCredentialAllowed(authRequestConfig.AllowAlternativeAuthentication);
                    }
                    else
                    {
                        builder = builder.SetNegativeButtonText(cancel);
                    }

                    var info = builder.Build();
                    var executor = Executors.NewSingleThreadExecutor();
                    using var dialog = new BiometricPrompt((FragmentActivity)CrossFingerprint.CurrentActivity, executor, handler);

                    dialog.Authenticate(info);
                    return await handler.GetTask();
                }
            }
            catch (Exception e)
            {
                return new FingerprintAuthenticationResult
                {
                    Status = FingerprintAuthenticationResultStatus.UnknownError,
                    ErrorMessage = e.Message
                };
            }

        }
    }
}