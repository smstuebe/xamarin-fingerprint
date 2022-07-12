using System;
using System.Collections.Generic;
using Android.OS;
using Plugin.Fingerprint.Abstractions;
using System.Threading;
using System.Threading.Tasks;
using Android;
using Android.App;
using Android.Content.PM;
using Android.Runtime;
using AndroidX.Biometric;
using AndroidX.Fragment.App;
using AndroidX.Lifecycle;
using Java.Util.Concurrent;
using System.Linq;
using System.Runtime.Versioning;

namespace Plugin.Fingerprint
{
    /// <summary>
    /// Android fingerprint implementations.
    /// </summary>
#if NET6_0_ANDROID
    [SupportedOSPlatform("android23.0")]
#endif
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

#if NET6_0_ANDROID
        [SupportedOSPlatform("android23.0")]
#endif
        public override async Task<FingerprintAvailability> GetAvailabilityAsync(bool allowAlternativeAuthentication = false)
        {
#if NET6_0_ANDROID
            if (!OperatingSystem.IsAndroidVersionAtLeast(6))
#else
            if (Build.VERSION.SdkInt < BuildVersionCodes.M)
#endif
                return FingerprintAvailability.NoApi;

            var biometricAvailability = GetBiometricAvailability();
            if (biometricAvailability == FingerprintAvailability.Available || !allowAlternativeAuthentication)
                return biometricAvailability;

            var context = Application.Context;

            try
            {
                var manager = (KeyguardManager)context.GetSystemService(Android.Content.Context.KeyguardService);
                if (manager.IsDeviceSecure)
                {
                    return FingerprintAvailability.Available;
                }

                return FingerprintAvailability.NoFallback;
            }
            catch
            {
                return FingerprintAvailability.NoFallback;
            }
        }

#if NET6_0_ANDROID
        [SupportedOSPlatform("android23.0")]
#endif
        private FingerprintAvailability GetBiometricAvailability()
        {
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

#if NET6_0_ANDROID
        [SupportedOSPlatform("android23.0")]
#endif
        protected override async Task<FingerprintAuthenticationResult> NativeAuthenticateAsync(AuthenticationRequestConfiguration authRequestConfig, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(authRequestConfig.Title))
                throw new ArgumentException("Title must not be null or empty on Android.", nameof(authRequestConfig.Title));

            if (!(CrossFingerprint.CurrentActivity is FragmentActivity))
                throw new InvalidOperationException($"Expected current activity to be '{typeof(FragmentActivity).FullName}' but was '{CrossFingerprint.CurrentActivity?.GetType().FullName}'. " +
                                                    "You need to use AndroidX. Have you installed Xamarin.AndroidX.Migration in your Android App project!?");

            try
            {
                var cancel = string.IsNullOrWhiteSpace(authRequestConfig.CancelTitle) ?
                    Application.Context.GetString(Android.Resource.String.Cancel) :
                    authRequestConfig.CancelTitle;

                var handler = new AuthenticationHandler();
                var builder = new BiometricPrompt.PromptInfo.Builder()
                    .SetTitle(authRequestConfig.Title)
                    .SetConfirmationRequired(authRequestConfig.ConfirmationRequired)
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


                var activity = (FragmentActivity)CrossFingerprint.CurrentActivity;
                using var dialog = new BiometricPrompt(activity, executor, handler);
                await using (cancellationToken.Register(() => dialog.CancelAuthentication()))
                {
                    dialog.Authenticate(info);
                    var result = await handler.GetTask();

                    TryReleaseLifecycleObserver(activity, dialog);

                    return result;
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

        /// <summary>
        /// Removes the lifecycle observer that is set by the BiometricPrompt from the lifecycleOwner.
        /// See: https://stackoverflow.com/a/59637670/1489968
        /// TODO: The new implementation of BiometricPrompt doesn't use this mechanism anymore. Recheck this code after Xamarin.AndroidX.Biometric has been updated.
        /// </summary>
        /// <param name="lifecycleOwner">Lifecycle owner where the observer was added.</param>
        /// <param name="dialog">Used BiometricPrompt</param>
#if NET6_0_ANDROID
        [SupportedOSPlatform("android23.0")]
#endif
        private static void TryReleaseLifecycleObserver(ILifecycleOwner lifecycleOwner, BiometricPrompt dialog)
        {
            var promptClass = Java.Lang.Class.FromType(dialog.GetType());
            var fields = promptClass.GetDeclaredFields();
            var lifecycleObserverField = fields?.FirstOrDefault(f => f.Name == "mLifecycleObserver");

            if (lifecycleObserverField is null)
                return;

            lifecycleObserverField.Accessible = true;
            var lastLifecycleObserver = lifecycleObserverField.Get(dialog).JavaCast<ILifecycleObserver>();
            var lifecycle = lifecycleOwner.Lifecycle;

            if (lastLifecycleObserver is null || lifecycle is null)
                return;

            lifecycle.RemoveObserver(lastLifecycleObserver);
        }
    }
}