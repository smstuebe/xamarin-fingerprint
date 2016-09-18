using System;
using System.Threading.Tasks;
using Com.Samsung.Android.Sdk.Pass;
using Plugin.Fingerprint.Abstractions;

namespace Plugin.Fingerprint.Samsung
{
    public class IdentifyListener : Java.Lang.Object, SpassFingerprint.IIdentifyListener
    {
        private readonly TaskCompletionSource<FingerprintAuthenticationResult> _taskCompletionSource;

        public IdentifyListener()
        {
            _taskCompletionSource = new TaskCompletionSource<FingerprintAuthenticationResult>();
        }

        public Task<FingerprintAuthenticationResult> GetTask()
        {
            return _taskCompletionSource.Task;
        }

        public void OnCompleted()
        {
        }

        public void OnFinished(SpassFingerprintStatus status)
        {
            FingerprintAuthenticationResultStatus resultStatus;
            switch (status)
            {
                case SpassFingerprintStatus.Success:
                    resultStatus = FingerprintAuthenticationResultStatus.Succeeded;
                    break;
                case SpassFingerprintStatus.TimeoutFailed:
                case SpassFingerprintStatus.SensorFailed:
                    resultStatus = FingerprintAuthenticationResultStatus.Failed;
                    break;
                case SpassFingerprintStatus.UserCancelled:
                    resultStatus = FingerprintAuthenticationResultStatus.Canceled;
                    break;
                case SpassFingerprintStatus.ButtonPressed:
                    resultStatus = FingerprintAuthenticationResultStatus.Canceled;
                    break;
                case SpassFingerprintStatus.QualityFailed:
                    resultStatus = FingerprintAuthenticationResultStatus.Failed;
                    break;
                case SpassFingerprintStatus.UserCancelledByTouchOutside:
                    resultStatus = FingerprintAuthenticationResultStatus.Canceled;
                    break;
                case SpassFingerprintStatus.Failed:
                    resultStatus = FingerprintAuthenticationResultStatus.Failed;
                    break;
                case SpassFingerprintStatus.OperationDenied:
                    resultStatus = FingerprintAuthenticationResultStatus.Failed;
                    break;
                case SpassFingerprintStatus.PasswordSuccess:
                    resultStatus = FingerprintAuthenticationResultStatus.Succeeded;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(status), status, null);
            }

            _taskCompletionSource.TrySetResult(new FingerprintAuthenticationResult
            {
                Status = resultStatus
            });
        }

        public void OnReady()
        {
        }

        public void OnStarted()
        {
        }
    }
}