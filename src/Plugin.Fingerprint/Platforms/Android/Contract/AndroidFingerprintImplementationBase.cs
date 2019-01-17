using System.Threading;
using System.Threading.Tasks;
using Android.OS;
using Plugin.Fingerprint.Abstractions;

namespace Plugin.Fingerprint.Contract
{


    //public class FingerprintAuthenticationCallback : BiometricPrompt.AuthenticationCallback
    //{
    //    private readonly IAuthenticationFailedListener _listener;
    //    private readonly TaskCompletionSource<FingerprintAuthenticationResult> _taskCompletionSource;

    //    public FingerprintAuthenticationCallback(IAuthenticationFailedListener listener)
    //    {
    //        _listener = listener;
    //        _taskCompletionSource = new TaskCompletionSource<FingerprintAuthenticationResult>();
    //    }

    //    public Task<FingerprintAuthenticationResult> GetTask()
    //    {
    //        return _taskCompletionSource.Task;
    //    }

    //    public override void OnAuthenticationSucceeded(BiometricPrompt.AuthenticationResult result)
    //    {
    //        base.OnAuthenticationSucceeded(result);

    //    }

    //    public override void OnAuthenticationFailed()
    //    {

    //    }


    //    public override void OnAuthenticationHelp(BiometricAcquiredStatus helpCode, ICharSequence helpString)
    //    {
    //        base.OnAuthenticationHelp(helpCode, helpString);
    //        _listener?.OnHelp(/* TODO: FingerprintAuthenticationHelp.MovedTooFast*/, helpString?.ToString());
    //    }

    //    //// https://developer.android.com/reference/android/hardware/fingerprint/FingerprintManager.AuthenticationCallback.html
    //    //public override void OnAuthenticationSucceeded(FingerprintManager.AuthenticationResult res)
    //    //{
    //    //    base.OnAuthenticationSucceeded(res);
    //    //    var result = new FingerprintAuthenticationResult { Status = FingerprintAuthenticationResultStatus.Succeeded };
    //    //    SetResultSafe(result);
    //    //}

    //    //public override void OnAuthenticationError(FingerprintState errorCode, ICharSequence errString)
    //    //{
    //    //    base.OnAuthenticationError(errorCode, errString);
    //    //    var message = errString != null ? errString.ToString() : string.Empty;
    //    //    var result = new FingerprintAuthenticationResult { Status = FingerprintAuthenticationResultStatus.Failed, ErrorMessage = message };

    //    //    if (errorCode == FingerprintState.ErrorLockout)
    //    //    {
    //    //        result.Status = FingerprintAuthenticationResultStatus.TooManyAttempts;
    //    //    }

    //    //    SetResultSafe(result);
    //    //}

    //    private void SetResultSafe(FingerprintAuthenticationResult result)
    //    {
    //        if (!(_taskCompletionSource.Task.IsCanceled || _taskCompletionSource.Task.IsCompleted || _taskCompletionSource.Task.IsFaulted))
    //        {
    //            _taskCompletionSource.SetResult(result);
    //        }
    //    }

    //    public override void OnAuthenticationFailed()
    //    {
    //        base.OnAuthenticationFailed();
    //        _listener?.OnFailedTry();
    //    }

    //    //public override void OnAuthenticationHelp(FingerprintState helpCode, ICharSequence helpString)
    //    //{
    //    //    base.OnAuthenticationHelp(helpCode, helpString);
    //    //    _listener?.OnHelp(FingerprintAuthenticationHelp.MovedTooFast, helpString?.ToString());
    //    //}
    //}


    /// <summary>
    /// Base implementation for the Android implementations.
    /// </summary>
    public abstract class AndroidFingerprintImplementationBase : FingerprintImplementationBase, IAndroidFingerprintImplementation
    {
        protected override async Task<FingerprintAuthenticationResult> NativeAuthenticateAsync(AuthenticationRequestConfiguration authRequestConfig, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (authRequestConfig.UseDialog)
            {
                //var fragment = CrossFingerprint.CreateDialogFragment();
                //return await fragment.ShowAsync(authRequestConfig, this, cancellationToken);
                using (var cancellationSignal = new CancellationSignal())
                using (cancellationToken.Register(() => cancellationSignal.Cancel()))
                {
                    //var builder = new BiometricPrompt.Builder(CrossFingerprint.CurrentActivity);
                    //builder.SetDescription(authRequestConfig.Reason);
                    //BiometricPrompt.AuthenticationCallback cb = new ;
                    //builder.Build().Authenticate(cancellationSignal, CrossFingerprint.CurrentActivity.MainExecutor, cb);
                }
            }

            return await AuthenticateNoDialogAsync(new DeafAuthenticationFailedListener(), cancellationToken);
        }

        public abstract Task<FingerprintAuthenticationResult> AuthenticateNoDialogAsync(IAuthenticationFailedListener failedListener, CancellationToken cancellationToken);

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
}