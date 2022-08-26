using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Android;
using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using AndroidX.Biometric;
using AndroidX.Fragment.App;
using AndroidX.Lifecycle;
using Java.Util.Concurrent;
using Plugin.Fingerprint.Abstractions;
using Plugin.Fingerprint.Platforms.Android.Utils;

namespace Plugin.Fingerprint
{
    /// <summary>
    /// Android fingerprint implementations.
    /// </summary>
    public class FingerprintImplementation : FingerprintImplementationBase
    {
        private readonly BiometricManager _manager;
        private readonly CryptoObjectHelper _cryptoObjectHelper;

        public FingerprintImplementation()
        {
            _manager = BiometricManager.From(Application.Context);

            if (CrossFingerprint.CryptoSettings == null)
            {
                CrossFingerprint.CryptoSettings = new CryptoSettings();
            }

            _cryptoObjectHelper = new CryptoObjectHelper(CrossFingerprint.CryptoSettings.CryptoKeyName);
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

                var cryptoObject = _cryptoObjectHelper.BuildCryptoObject();
                var handler = new AuthenticationHandler(CrossFingerprint.CryptoSettings, (resultCryptoObject) => resultCryptoObject.Cipher == cryptoObject.Cipher);
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
                    dialog.Authenticate(info, cryptoObject);
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