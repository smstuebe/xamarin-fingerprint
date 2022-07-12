using System.Threading.Tasks;
using Android.Content;
using Java.Lang;
using Plugin.Fingerprint.Abstractions;
using AndroidX.Biometric;
using System.Runtime.Versioning;

namespace Plugin.Fingerprint
{
#if NET6_0_ANDROID
    [SupportedOSPlatform("android")]
#endif
    public class AuthenticationHandler : BiometricPrompt.AuthenticationCallback, IDialogInterfaceOnClickListener
    {
        private readonly TaskCompletionSource<FingerprintAuthenticationResult> _taskCompletionSource;

        public AuthenticationHandler()
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

#if NET6_0_ANDROID
        [SupportedOSPlatform("android21.0")]
#endif
        public override void OnAuthenticationSucceeded(BiometricPrompt.AuthenticationResult result)
        {
            base.OnAuthenticationSucceeded(result);
            var faResult = new FingerprintAuthenticationResult { Status = FingerprintAuthenticationResultStatus.Succeeded };
            SetResultSafe(faResult);
        }

#if NET6_0_ANDROID
        [SupportedOSPlatform("android21.0")]
#endif
        public override void OnAuthenticationError(int errorCode, ICharSequence errString)
        {
            base.OnAuthenticationError(errorCode, errString);

            var message = errString != null ? errString.ToString() : string.Empty;
            var result = new FingerprintAuthenticationResult { Status = FingerprintAuthenticationResultStatus.Failed, ErrorMessage = message };

            result.Status = errorCode switch
            {
                BiometricPrompt.ErrorLockout => FingerprintAuthenticationResultStatus.TooManyAttempts,
                BiometricPrompt.ErrorUserCanceled => FingerprintAuthenticationResultStatus.Canceled,
                BiometricPrompt.ErrorNegativeButton => FingerprintAuthenticationResultStatus.Canceled,
                _ => FingerprintAuthenticationResultStatus.Failed
            };

            SetResultSafe(result);
        }

#if NET6_0_ANDROID
        [SupportedOSPlatform("android21.0")]
#endif
        public override void OnAuthenticationFailed()
        {
            base.OnAuthenticationFailed();
        }

        public void OnClick(IDialogInterface dialog, int which)
        {
            var faResult = new FingerprintAuthenticationResult { Status = FingerprintAuthenticationResultStatus.Canceled };
            SetResultSafe(faResult);
        }

        //public override void OnAuthenticationHelp(BiometricAcquiredStatus helpCode, ICharSequence helpString)
        //{
        //    base.OnAuthenticationHelp(helpCode, helpString);
        //    _listener?.OnHelp(FingerprintAuthenticationHelp.MovedTooFast, helpString?.ToString());
        //}
    }
}