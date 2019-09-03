using Android;
using Android.Content;
using Android.Content.PM;
using Android.Hardware.Biometrics;
using Android.Hardware.Fingerprints;
using Android.OS;
using Android.Util;
using Java.Lang;
using Plugin.Fingerprint.Abstractions;
using System.Net.Mime;
using System.Threading;
using System.Threading.Tasks;
using Android.App;

namespace Plugin.Fingerprint.Contract
{
    public class BiometrisAuthenticationCallback : BiometricPrompt.AuthenticationCallback
    {
        private readonly TaskCompletionSource<FingerprintAuthenticationResult> _taskCompletionSource;

        public BiometrisAuthenticationCallback()
        {
            _taskCompletionSource = new TaskCompletionSource<FingerprintAuthenticationResult>();
        }

        public Task<FingerprintAuthenticationResult> GetTask()
        {
            return _taskCompletionSource.Task;
        }

        private void SetResultSafe(FingerprintAuthenticationResult result)
        {
            if (!(_taskCompletionSource.Task.IsCanceled || _taskCompletionSource.Task.IsCompleted || _taskCompletionSource.Task.IsFaulted))
            {
                _taskCompletionSource.SetResult(result);
            }
        }

        public override void OnAuthenticationSucceeded(BiometricPrompt.AuthenticationResult result)
        {
            base.OnAuthenticationSucceeded(result);
            var faResult = new FingerprintAuthenticationResult { Status = FingerprintAuthenticationResultStatus.Succeeded };
            SetResultSafe(faResult);
        }

        public override void OnAuthenticationError(BiometricErrorCode errorCode, ICharSequence errString)
        {
            base.OnAuthenticationError(errorCode, errString);
            var message = errString != null ? errString.ToString() : string.Empty;
            var result = new FingerprintAuthenticationResult { Status = FingerprintAuthenticationResultStatus.Failed, ErrorMessage = message };

            if (errorCode == BiometricErrorCode.Lockout)
            {
                result.Status = FingerprintAuthenticationResultStatus.TooManyAttempts;
            }

            SetResultSafe(result);
        }
    }


    /// <summary>
    /// Base implementation for the Android implementations.
    /// </summary>
    public abstract class AndroidFingerprintImplementationBase : FingerprintImplementationBase
    {
        // protected abstract Task<FingerprintAuthenticationResult> NativeAuthenticateAsync(AuthenticationRequestConfiguration authRequestConfig, CancellationToken cancellationToken = default);

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
    }

    /// <summary>
    /// Base implementation for the Android implementations.
    /// </summary>
    public class BiometricAndroidFingerprintImplementation : AndroidFingerprintImplementationBase
    {
        public override async Task<FingerprintAvailability> GetAvailabilityAsync(bool allowAlternativeAuthentication = false)
        {
            if (Build.VERSION.SdkInt < BuildVersionCodes.M)
                return FingerprintAvailability.NoApi;

            var context = Application.Context;
            if (context.CheckCallingOrSelfPermission(Manifest.Permission.UseFingerprint) != Permission.Granted)
                return FingerprintAvailability.NoPermission;

            try
            {
                // service can be null certain devices #83
                var fpService = GetService();
                if (fpService == null)
                    return FingerprintAvailability.NoApi;

                if (!fpService.IsHardwareDetected)
                    return FingerprintAvailability.NoSensor;

                if (!fpService.HasEnrolledFingerprints)
                    return FingerprintAvailability.NoFingerprint;

                return FingerprintAvailability.Available;
            }
            catch (Throwable e)
            {
                // ServiceNotFoundException can happen on certain devices #83
                Log.Error(nameof(BiometricAndroidFingerprintImplementation), e, "Could not create Android service");
                return FingerprintAvailability.Unknown;
            }
        }

        private static FingerprintManager GetService()
        {
            return (FingerprintManager)Application.Context.GetSystemService(Class.FromType(typeof(FingerprintManager)));
        }

        protected override async Task<FingerprintAuthenticationResult> NativeAuthenticateAsync(AuthenticationRequestConfiguration authRequestConfig, CancellationToken cancellationToken = default)
        {
            using (var cancellationSignal = new CancellationSignal())
            using (cancellationToken.Register(() => cancellationSignal.Cancel()))
            {
                var cancel = string.IsNullOrWhiteSpace(authRequestConfig.CancelTitle) ? 
                    Application.Context.GetString(Android.Resource.String.Cancel) : 
                    authRequestConfig.CancelTitle;
                var callback = new BiometrisAuthenticationCallback();
                var dialog = new BiometricPrompt.Builder(CrossFingerprint.CurrentActivity)
                  .SetTitle("Login")
                  .SetSubtitle("Do it!")
                  .SetDescription(authRequestConfig.Reason)
                  .SetNegativeButton(cancel, CrossFingerprint.CurrentActivity.MainExecutor, new Lis())
                  .Build();

                dialog.Authenticate(cancellationSignal, CrossFingerprint.CurrentActivity.MainExecutor, callback);
                return await callback.GetTask();
            }
        }
    }

    internal class Lis : Java.Lang.Object, IDialogInterfaceOnClickListener
    {
        public void OnClick(IDialogInterface dialog, int which)
        {
            throw new System.NotImplementedException();
        }
    }
}