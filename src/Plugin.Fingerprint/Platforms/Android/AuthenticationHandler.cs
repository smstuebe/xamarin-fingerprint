using System;
using System.Threading.Tasks;
using Android.Content;
using AndroidX.Biometric;
using Java.Lang;
using Javax.Crypto;
using Plugin.Fingerprint.Abstractions;

namespace Plugin.Fingerprint
{
    public class AuthenticationHandler : BiometricPrompt.AuthenticationCallback, IDialogInterfaceOnClickListener
    {
        private readonly TaskCompletionSource<FingerprintAuthenticationResult> _taskCompletionSource;
        private readonly byte[] _cipherSecretBytes;

        public AuthenticationHandler(byte[] cipherSecretBytes)
        {
            _taskCompletionSource = new TaskCompletionSource<FingerprintAuthenticationResult>();
            _cipherSecretBytes = cipherSecretBytes;
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
            var cipher = result.CryptoObject.Cipher;
            if (cipher != null)
            {
                var errorMsg = string.Empty;
                try
                {
                    // Ensuring encryption
                    byte[] cipherFinalResult = _cipherSecretBytes == null
                                                    ? cipher.DoFinal()
                                                    : cipher.DoFinal(_cipherSecretBytes);

                    // Everything is fine
                }
                catch (BadPaddingException bpe)
                {
                    errorMsg = $"Failed to encrypt the data with the generated key.{Environment.NewLine}{bpe.Message}";
                }
                catch (IllegalBlockSizeException ibse)
                {
                    errorMsg = $"Failed to encrypt the data with the generated key.{Environment.NewLine}{ibse.Message}";
                }


                if (!string.IsNullOrEmpty(errorMsg))
                {
                    // Can't really trust the results.
                    var errorResult = new FingerprintAuthenticationResult { Status = FingerprintAuthenticationResultStatus.InvalidCipher, ErrorMessage = errorMsg };
                    SetResultSafe(errorResult);

                    return;
                }
            }
            else
            {
                base.OnAuthenticationSucceeded(result);
            }

            var faResult = new FingerprintAuthenticationResult { Status = FingerprintAuthenticationResultStatus.Succeeded };
            SetResultSafe(faResult);
        }

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