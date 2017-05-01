using System;
using System.Text;

using Android.App;
using Android.Support.V4.Hardware.Fingerprint;

using Java.Lang;
using Javax.Crypto;
using CancellationSignal = Android.Support.V4.OS.CancellationSignal;
using Res = Android.Resource;
using Android.Content;
using System.Threading.Tasks;

namespace Plugin.Fingerprint.Dialog
{
    public class AuthenticationCallback : FingerprintManagerCompat.AuthenticationCallback
    {
        private readonly TaskCompletionSource<AuthenticationCallbackResult> _taskCompletionSource;
        private readonly byte[] _sourceValue;

        public AuthenticationCallback(
            byte[] sourceValue, 
            TaskCompletionSource<AuthenticationCallbackResult> taskCompletionSource)
        {
            _sourceValue = sourceValue;
            _taskCompletionSource = taskCompletionSource;
        }

        public override void OnAuthenticationSucceeded(FingerprintManagerCompat.AuthenticationResult result)
        {
            if (result.CryptoObject.Cipher != null)
            {
                try
                {                  
                    byte[] doFinalResult = result.CryptoObject.Cipher.DoFinal(_sourceValue);                   

                    _taskCompletionSource.TrySetResult(new AuthenticationCallbackResult
                    {
                        Status = Abstractions.FingerprintAuthenticationResultStatus.Succeeded,
                        IV = result.CryptoObject.Cipher.GetIV(),
                        Result = doFinalResult
                    });
                }
                catch (BadPaddingException bpe)
                {                   
                    ReportAuthenticationException(bpe);
                }
                catch (IllegalBlockSizeException ibse)
                {                   
                    ReportAuthenticationException(ibse);
                }
            }
            else
            {                              
                ReportAuthenticationException(new Java.Lang.Exception("No cipher"));
            }
        }

        private void ReportScanFailure(int errMsgId, string errorMessage)
        {
            _taskCompletionSource.TrySetException(new System.Exception(errorMessage));
        }

        void ReportAuthenticationException(Java.Lang.Exception exception)
        {
            _taskCompletionSource.TrySetException(new System.Exception(exception.Message));
        }

        public override void OnAuthenticationError(int errMsgId, ICharSequence errString)
        {
            // There are some situations where we don't care about the error. For example, 
            // if the user cancelled the scan, this will raise errorID #5. We don't want to
            // report that, we'll just ignore it as that event is a part of the workflow.
            ////bool reportError = (errMsgId == 5) 

            if (errMsgId == 7)
            {
                _taskCompletionSource.TrySetResult(new AuthenticationCallbackResult
                {
                    Status = Abstractions.FingerprintAuthenticationResultStatus.TooManyAttempts,
                });
            }
        }

        public override void OnAuthenticationFailed()
        {
            _taskCompletionSource.TrySetResult(new AuthenticationCallbackResult
            {
                Status = Abstractions.FingerprintAuthenticationResultStatus.Failed,                
            });
        }

        public override void OnAuthenticationHelp(int helpMsgId, ICharSequence helpString)
        {
            ReportScanFailure(helpMsgId, helpString.ToString());
        }
    }
}