using System;
using System.Threading.Tasks;
using Com.Samsung.Android.Sdk.Pass;
using Plugin.Fingerprint.Abstractions;
using Plugin.Fingerprint.Contract;

namespace Plugin.Fingerprint.Samsung
{
    public class IdentifyListener : Java.Lang.Object, SpassFingerprint.IIdentifyListener
    {
        private readonly Func<SpassFingerprint.IIdentifyListener, Task<bool>> _startIdentify;
        private readonly IAuthenticationFailedListener _failedListener;
        private readonly TaskCompletionSource<FingerprintAuthenticationResult> _taskCompletionSource;
        private int _retriesLeft;
        //private TaskCompletionSource<int> _completedSource;

        public IdentifyListener(Func<SpassFingerprint.IIdentifyListener, Task<bool>> startIdentify, IAuthenticationFailedListener failedListener)
        {
            _retriesLeft = 2;
            _startIdentify = startIdentify;
            _failedListener = failedListener;
            _taskCompletionSource = new TaskCompletionSource<FingerprintAuthenticationResult>();
        }

        public async Task<FingerprintAuthenticationResult> GetTask()
        {
            if (!await StartIdentify())
            {
                return new FingerprintAuthenticationResult
                {
                    Status = FingerprintAuthenticationResultStatus.UnknownError
                };
            }

            return await _taskCompletionSource.Task;
        }

        public void CancelManually()
        {
            _taskCompletionSource.TrySetResult(new FingerprintAuthenticationResult
            {
                Status = FingerprintAuthenticationResultStatus.Canceled
            });
        }

        private async Task<bool> StartIdentify()
        {
            // TODO: use task completion source instead of retries in SamsungFingerprintImplementation, if samsung fixes the library 
            //_completedSource = new TaskCompletionSource<int>();
            return await _startIdentify(this);
        }

        public void OnCompleted()
        {
            //_completedSource?.TrySetResult(0);
        }

        public async void OnFinished(SpassFingerprintStatus status)
        {
            //_completedSource = new TaskCompletionSource<int>();
            var resultStatus = GetResultStatus(status);
            
            if (resultStatus == FingerprintAuthenticationResultStatus.Failed && _retriesLeft > 0)
            {
                _failedListener?.OnFailedTry();

                if (_retriesLeft > 0)
                {
                    _retriesLeft--;

                    //await _completedSource.Task;
                
                    if (await StartIdentify())
                        return;
                }
            }
            else if (resultStatus == FingerprintAuthenticationResultStatus.Failed && _retriesLeft <= 0)
            {
                resultStatus = FingerprintAuthenticationResultStatus.TooManyAttempts;
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
            // TODO: OnStarted -> OnCompleted = failed (to short touched) OMG 
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