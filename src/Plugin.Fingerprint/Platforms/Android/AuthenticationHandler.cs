using System;
using System.Linq;
using System.Threading.Tasks;
using Android.Content;
using AndroidX.Biometric;
using Java.Lang;
using Javax.Crypto;
using Plugin.Fingerprint.Abstractions;
using Plugin.Fingerprint.Platforms.Android.Utils;
using static AndroidX.Biometric.BiometricPrompt;

namespace Plugin.Fingerprint
{
    public class AuthenticationHandler : BiometricPrompt.AuthenticationCallback, IDialogInterfaceOnClickListener
    {
        private readonly TaskCompletionSource<FingerprintAuthenticationResult> _taskCompletionSource;
        private readonly CryptoSettings _cryptoSettings;
        private readonly Func<CryptoObject, bool> _validatedCipherFunc;

        public AuthenticationHandler(CryptoSettings cryptoSettings, Func<CryptoObject, bool> validatedCipherFunc)
        {
            _taskCompletionSource = new TaskCompletionSource<FingerprintAuthenticationResult>();
            _cryptoSettings = cryptoSettings;
            _validatedCipherFunc = validatedCipherFunc;
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
            base.OnAuthenticationSucceeded(result);

            var faResult = new FingerprintAuthenticationResult { Status = FingerprintAuthenticationResultStatus.Succeeded };
            if (result.CryptoObject == null)
            {
                faResult = new FingerprintAuthenticationResult { Status = FingerprintAuthenticationResultStatus.MissingCryptoObject, ErrorMessage = $"CryptoObject was empty" };
            }
            else if (!_validatedCipherFunc(result.CryptoObject))
            {
                faResult = new FingerprintAuthenticationResult { Status = FingerprintAuthenticationResultStatus.InvalidCipher, ErrorMessage = $"Cipher changed since Authentication call. Maybe it was manipulated" };
            }
            else
            {
                var errorMsg = string.Empty;
                if (result.CryptoObject.Cipher != null)
                {
                    var cipher = result.CryptoObject.Cipher;
                    try
                    {
                        // Ensuring encryption works (Authenticated & Valid)
                        var cipherFinalResult = cipher.DoFinal(_cryptoSettings.CipherSecretBytes);

                        if (cipherFinalResult == null)
                        {
                            errorMsg = "DoFinal(..) was manipulated! Result was null";
                        }
                        else if (_cryptoSettings.CipherSecretBytes.SequenceEqual(cipherFinalResult))
                        {
                            errorMsg = "DoFinal(..) was manipulated! Result matches Secret";
                        }

                        try
                        {
                            cipherFinalResult = cipher.DoFinal(_cryptoSettings.CipherSecretBytes);
                            errorMsg = "Exceptions getting catched. This could mean manipulation";
                        }
                        catch (IllegalStateException)
                        {
                            // Everything looks valid and works as expected
                        }
                    }
                    catch (BadPaddingException bpe)
                    {
                        errorMsg = $"Failed to encrypt the data with the generated key.{Environment.NewLine}{bpe.Message}";
                    }
                    catch (IllegalBlockSizeException ibse)
                    {
                        errorMsg = $"Failed to encrypt the data with the generated key.{Environment.NewLine}{ibse.Message}";
                    }
                }
                else
                {
                    errorMsg = $"CryptoObject was given but Cipher was missing!";
                }

                if (!string.IsNullOrEmpty(errorMsg))
                {
                    // Can't really trust the results.
                    faResult = new FingerprintAuthenticationResult { Status = FingerprintAuthenticationResultStatus.InvalidCipher, ErrorMessage = errorMsg };
                }
            }

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