using System.Threading.Tasks;
using Android.Hardware.Fingerprints;
using Java.Lang;
using Plugin.Fingerprint.Abstractions;
using Plugin.Fingerprint.Contract;

namespace Plugin.Fingerprint.Standard
{
    public class FingerprintAuthenticationCallback : FingerprintManager.AuthenticationCallback, IAuthenticationCallback
    {
        private readonly IAuthenticationFailedListener _listener;
        private readonly TaskCompletionSource<FingerprintAuthenticationResult> _taskCompletionSource;

        public FingerprintAuthenticationCallback(IAuthenticationFailedListener listener)
        {
            _listener = listener;
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
            SetResultSafe(result);
        }

        public override void OnAuthenticationError(FingerprintState errorCode, ICharSequence errString)
        {
            base.OnAuthenticationError(errorCode, errString);
            var message = errString != null ? errString.ToString() : string.Empty;
            var result = new FingerprintAuthenticationResult { Status = FingerprintAuthenticationResultStatus.Failed, ErrorMessage = message };
            SetResultSafe(result);
        }

        private void SetResultSafe(FingerprintAuthenticationResult result)
        {
            if (!(_taskCompletionSource.Task.IsCanceled || _taskCompletionSource.Task.IsCompleted ||
                  _taskCompletionSource.Task.IsFaulted))
            {
                _taskCompletionSource.SetResult(result);
            }
        }

        public override void OnAuthenticationFailed()
        {
            base.OnAuthenticationFailed();
            _listener?.OnFailedTry();
        }

        //public override void OnAuthenticationHelp(FingerprintState helpCode, ICharSequence helpString)
        //{
        //    base.OnAuthenticationHelp(helpCode, helpString);
        //    _taskCompletionSource.SetResult(null);
        //}
    }
}