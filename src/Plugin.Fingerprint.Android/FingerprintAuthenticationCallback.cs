using System;
using System.Threading.Tasks;
using Android.Hardware.Fingerprints;
using Android.Runtime;
using Java.Lang;
using Plugin.Fingerprint.Abstractions;

namespace Plugin.Fingerprint
{
    public class FingerprintAuthenticationCallback : FingerprintManager.AuthenticationCallback
    {
        private readonly TaskCompletionSource<FingerprintAuthenticationResult> _taskCompletionSource;

        public FingerprintAuthenticationCallback()
        {
            _taskCompletionSource = new TaskCompletionSource<FingerprintAuthenticationResult>();
        }

		public FingerprintAuthenticationCallback(IntPtr a, JniHandleOwnership b) : base(a, b)
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

		//public override void OnAuthenticationHelp(FingerprintState helpCode, ICharSequence helpString)
		//{
		//    base.OnAuthenticationHelp(helpCode, helpString);
		//    _taskCompletionSource.SetResult(null);
		//}
	}
}