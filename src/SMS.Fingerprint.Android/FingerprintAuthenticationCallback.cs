using System.Threading.Tasks;
using Android.Hardware.Fingerprints;
using Java.Lang;
using SMS.Fingerprint.Abstractions;

namespace SMS.Fingerprint
{
    internal class FingerprintAuthenticationCallback : FingerprintManager.AuthenticationCallback
    {
        private readonly TaskCompletionSource<FingerprintAuthenticationResult> _taskCompletionSource;

        public FingerprintAuthenticationCallback()
        {
            _taskCompletionSource = new TaskCompletionSource<FingerprintAuthenticationResult>();
        }

        public Task<FingerprintAuthenticationResult> GetTask()
        {
            return _taskCompletionSource.Task;
        }

        // https://developer.android.com/reference/android/hardware/fingerprint/FingerprintManager.AuthenticationCallback.html
        public override void OnAuthenticationSucceeded(FingerprintManager.AuthenticationResult res)
        {
            base.OnAuthenticationSucceeded(res);
            var result = new FingerprintAuthenticationResult { Status = FingerprintAuthenticationResultStatus.Succeeded };
            _taskCompletionSource.SetResult(result);
        }

        public override void OnAuthenticationFailed()
        {
            base.OnAuthenticationFailed();
            var result = new FingerprintAuthenticationResult { Status = FingerprintAuthenticationResultStatus.UnknownError };
            _taskCompletionSource.SetResult(result);
        }

        public override void OnAuthenticationError(FingerprintState errorCode, ICharSequence errString)
        {
            base.OnAuthenticationError(errorCode, errString);
            var message = errString != null ? errString.ToString() : string.Empty;
            var result = new FingerprintAuthenticationResult { Status = FingerprintAuthenticationResultStatus.Failed, ErrorMessage = message };
            _taskCompletionSource.SetResult(result);
        }

        //public override void OnAuthenticationHelp(FingerprintState helpCode, ICharSequence helpString)
        //{
        //    base.OnAuthenticationHelp(helpCode, helpString);
        //    _taskCompletionSource.SetResult(null);
        //}
    }
}