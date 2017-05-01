using System.Threading;
using System.Threading.Tasks;
using Plugin.Fingerprint.Abstractions;
using System.Collections.Generic;
using System;
using Plugin.Fingerprint.Utils;
using Javax.Crypto;
using Android.Hardware.Fingerprints;

using Android.Support.V4.Hardware.Fingerprint;
using Plugin.Fingerprint.Dialog;
using Android.Support.V4.OS;
using Android.App;
using Android.Content;

namespace Plugin.Fingerprint.Contract
{
    /// <summary>
    /// Base implementation for the Android implementations.
    /// </summary>
    public abstract class AndroidFingerprintImplementationBase : FingerprintImplementationBase, IAndroidFingerprintImplementation
    {
        private const string prefsName = "Plugin.Fingerprint.Android";

        protected override async Task<FingerprintAuthenticationResult> NativeAuthenticateAsync(AuthenticationRequestConfiguration authRequestConfig, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (authRequestConfig.UseDialog)
            {
                var fragment = CrossFingerprint.CreateDialogFragment();
                return await fragment.ShowAsync(authRequestConfig, this, cancellationToken);
            }

            return await AuthenticateNoDialogAsync(new DeafAuthenticationFailedListener(), cancellationToken);
        }

        public abstract Task<FingerprintAuthenticationResult> AuthenticateNoDialogAsync(IAuthenticationFailedListener failedListener, CancellationToken cancellationToken);

        protected override async Task<SecureValueResult> NativeSetSecureValue(SetSecureValueRequestConfiguration setSecureValueRequestConfig, CancellationToken cancellationToken)
        {                    
            var currentActivity = CrossFingerprint.CurrentActivity;
            var _fingerprintManager = FingerprintManagerCompat.From(currentActivity);

            var taskCompletionSource = new TaskCompletionSource<AuthenticationCallbackResult>();

            try
            {
                var sourceValue = System.Text.Encoding.UTF8.GetBytes(setSecureValueRequestConfig.Value);

                _fingerprintManager.Authenticate(CryptoObjectHelper.Instance.BuildCryptoObject(CipherMode.EncryptMode),
                                                 (int)FingerprintAuthenticationFlags.None,
                                                 new CancellationSignal(),
                                                 new AuthenticationCallback(sourceValue, taskCompletionSource),
                                                 null);

                var fragment = CrossFingerprint.CreateSecureValueDialogFragment();
                var result = await fragment.ShowAsync(setSecureValueRequestConfig, taskCompletionSource, cancellationToken);

                if (result.Status == FingerprintAuthenticationResultStatus.Succeeded)
                {
                    var prefs = Application.Context.GetSharedPreferences(prefsName, FileCreationMode.Private);
                    var edit = prefs.Edit();
                    edit.PutString($"{setSecureValueRequestConfig.Key}:iv", Convert.ToBase64String(result.IV));
                    edit.PutString($"{setSecureValueRequestConfig.Key}:value", Convert.ToBase64String(result.Result));
                    edit.Commit();
                }

                return new SecureValueResult
                {
                    Status = result.Status,
                    // Pass through error info?
                };
            }
            catch (Exception ex)
            {
                return new SecureValueResult
                {
                    Status = FingerprintAuthenticationResultStatus.UnknownError,
                    ErrorMessage = ex.Message
                };
            }
        }

        protected override Task<SecureValueResult> NativeRemoveSecureValue(SecureValueRequestConfiguration secureValueRequestConfig, CancellationToken cancellationToken)
        {
            try
            {
                var prefs = Application.Context.GetSharedPreferences(prefsName, FileCreationMode.Private);
                var edit = prefs.Edit();
                edit.Remove($"{secureValueRequestConfig.Key}:iv");
                edit.Remove($"{secureValueRequestConfig.Key}:value");
                edit.Commit();

                return Task.FromResult(new SecureValueResult
                {
                    Status = FingerprintAuthenticationResultStatus.Succeeded,
                });
            }
            catch (Exception ex)
            {
                return Task.FromResult(new SecureValueResult
                {
                    Status = FingerprintAuthenticationResultStatus.UnknownError,
                    ErrorMessage = ex.Message,
                });
            }           
        }

        protected override async Task<GetSecureValueResult> NativeGetSecureValue(SecureValueRequestConfiguration secureValueRequestConfig, CancellationToken cancellationToken)
        {
            var currentActivity = CrossFingerprint.CurrentActivity;
            var _fingerprintManager = FingerprintManagerCompat.From(currentActivity);

            var taskCompletionSource = new TaskCompletionSource<AuthenticationCallbackResult>();

            byte[] iv;
            byte[] value;
            try
            {
                var prefs = Application.Context.GetSharedPreferences(prefsName, FileCreationMode.Private);
                iv = Convert.FromBase64String(prefs.GetString($"{secureValueRequestConfig.Key}:iv", null));
                value = Convert.FromBase64String(prefs.GetString($"{secureValueRequestConfig.Key}:value", null));
            }
            catch (Exception ex)
            {
                return new GetSecureValueResult
                {
                    Status = FingerprintAuthenticationResultStatus.UnknownError,
                    ErrorMessage = $"No values for {secureValueRequestConfig.Key}"
                };
            }

            try
            {            
                _fingerprintManager.Authenticate(CryptoObjectHelper.Instance.BuildCryptoObject(CipherMode.DecryptMode, iv),
                                                 (int)FingerprintAuthenticationFlags.None,
                                                 new CancellationSignal(),
                                                 new AuthenticationCallback(value, taskCompletionSource),
                                                 null);

                var fragment = CrossFingerprint.CreateSecureValueDialogFragment();
                var result = await fragment.ShowAsync(secureValueRequestConfig, taskCompletionSource, cancellationToken);

                if (result.Status == FingerprintAuthenticationResultStatus.Succeeded)
                {
                    return new GetSecureValueResult
                    {
                        Status = result.Status,
                        Value = System.Text.Encoding.UTF8.GetString(result.Result),
                    };
                }

                return new GetSecureValueResult
                {
                    Status = result.Status,
                    // Pass through error info?
                };
            }
            catch (Exception ex)
            {
                return new GetSecureValueResult
                {
                    Status = FingerprintAuthenticationResultStatus.UnknownError,
                    ErrorMessage = ex.Message
                };
            }
        }
    }
}