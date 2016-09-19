using System;
using System.Threading.Tasks;
using Com.Samsung.Android.Sdk.Pass;
using Plugin.Fingerprint.Abstractions;
using Plugin.Fingerprint.Contract;

namespace Plugin.Fingerprint.Samsung
{
    public class IdentifyListener : Java.Lang.Object, SpassFingerprint.IIdentifyListener
    {
        private readonly Func<SpassFingerprint.IIdentifyListener, bool> _startIdentify;
        private readonly IAuthenticationFailedListener _failedListener;
        private readonly TaskCompletionSource<FingerprintAuthenticationResult> _taskCompletionSource;
        private int _retriesLeft;
        private TaskCompletionSource<int> _completedSource;

        public IdentifyListener(Func<SpassFingerprint.IIdentifyListener, bool> startIdentify, IAuthenticationFailedListener failedListener)
        {
            _retriesLeft = 2;
            _startIdentify = startIdentify;
            _failedListener = failedListener;
            _taskCompletionSource = new TaskCompletionSource<FingerprintAuthenticationResult>();
        }

        public Task<FingerprintAuthenticationResult> GetTask()
        {
            if (!StartIdentify())
            {
                return Task.FromResult(new FingerprintAuthenticationResult
                {
                    Status = FingerprintAuthenticationResultStatus.UnknownError
                });
            }

            return _taskCompletionSource.Task;
        }

        private bool StartIdentify()
        {
            _completedSource = new TaskCompletionSource<int>();
            return _startIdentify(this);
        }

        public void OnCompleted()
        {
            _completedSource?.TrySetResult(0);
        }

        public async void OnFinished(SpassFingerprintStatus status)
        {
            var resultStatus = GetResultStatus(status);

            if (resultStatus == FingerprintAuthenticationResultStatus.Failed && _retriesLeft > 0)
            {
                _failedListener?.OnFailedTry();

                if (_retriesLeft > 0)
                {
                    _retriesLeft--;

                    await _completedSource.Task;

                    if (StartIdentify())
                        return;
                }
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

        private static FingerprintAuthenticationResultStatus GetResultStatus(SpassFingerprintStatus status)
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
                    resultStatus = FingerprintAuthenticationResultStatus.UnknownError;
                    break;
                case SpassFingerprintStatus.PasswordSuccess:
                    resultStatus = FingerprintAuthenticationResultStatus.Succeeded;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(status), status, null);
            }
            return resultStatus;
        }
    }
}