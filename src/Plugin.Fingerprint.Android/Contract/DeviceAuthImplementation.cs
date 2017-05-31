using System;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Security.Keystore;
using Java.Security;
using Javax.Crypto;
using Plugin.Fingerprint.Abstractions;

namespace Plugin.Fingerprint.Contract
{
    public class DeviceAuthImplementation : IDeviceAuthImplementation
    {
        private const string KeystoreName = "AndroidKeyStore";
		
        private const string KeyAlgorithm = KeyProperties.KeyAlgorithmAes;
        private const string BlockMode = KeyProperties.BlockModeCbc;
        private const string EncryptionPadding = KeyProperties.EncryptionPaddingPkcs7;
        private const string Transformation = KeyAlgorithm + "/" + BlockMode + "/" + EncryptionPadding;
		
        private const string KeyNameSuffix = "deviceAuthKey3";
		
        private readonly string KeyName;
        private KeyStore _keyStore;
        private KeyguardManager _keyguardManager => (KeyguardManager)Activity.GetSystemService(Context.KeyguardService);

        private Activity Activity => CrossFingerprint.CurrentActivity;

        public DeviceAuthImplementation()
        {
            var packageName = Activity.PackageName;
            KeyName = string.Join(".", new string[] { packageName, KeyNameSuffix });
			
            _keyStore = KeyStore.GetInstance(KeystoreName);
			_keyStore.Load(null);
			
            GetKey(); // Setup device key
        }

        public bool IsDeviceAuthSetup()
        {
            return _keyguardManager.IsKeyguardSecure;
        }

        IKey GetKey()
        {
            IKey secretKey;
            if (!_keyStore.IsKeyEntry(KeyName))
            {
                CreateKey();
            }

            secretKey = _keyStore.GetKey(KeyName, null);
            return secretKey;
        }

        private void CreateKey()
        {
            var keyGenerator = KeyGenerator.GetInstance(KeyProperties.KeyAlgorithmAes, KeystoreName);

            keyGenerator.Init(new KeyGenParameterSpec.Builder(KeyName, KeyStorePurpose.Decrypt | KeyStorePurpose.Encrypt)
                                 .SetBlockModes(KeyProperties.BlockModeCbc)
                                 .SetUserAuthenticationRequired(true)
                                 .SetEncryptionPaddings(KeyProperties.EncryptionPaddingPkcs7)
                                 .Build());
            keyGenerator.GenerateKey();
        }

        public async Task<FingerprintAuthenticationResult> AuthenticateAsync()
        {
            // TODO KS: Cancellation token

            var result = new FingerprintAuthenticationResult();

            try
            {
                var secretKey = GetKey();
                var cipher = Cipher.GetInstance(Transformation);
                cipher.Init(CipherMode.EncryptMode, secretKey);
                Random rnd = new Random();
                Byte[] nounceBytes = new Byte[10];
                rnd.NextBytes(nounceBytes);
                // attempt encrypting data
                cipher.DoFinal(nounceBytes);
            }
            catch (GeneralSecurityException ex) when (ex is UserNotAuthenticatedException || ex.InnerException?.Message == "Key user not authenticated")
            {
                // User is not authenticated, let's authenticate with device credentials.
                return await ShowAuthenticationScreenAsync();
            }
            catch(KeyPermanentlyInvalidatedException ex)
            {
                // TODO KS: Fix by generating random
                // User has changed their fingerprint and/or device auth
                // We need a new key
            }
            catch (GeneralSecurityException ex)
            {
                result.Status = FingerprintAuthenticationResultStatus.UnknownError;
                result.ErrorMessage = ex.ToString();
                return result;
            }

            result.Status = FingerprintAuthenticationResultStatus.Unknown;
            return result;
        }

        private async Task<FingerprintAuthenticationResult> ShowAuthenticationScreenAsync()
        {
            var deviceAuthTcs = new TaskCompletionSource<FingerprintAuthenticationResultStatus>();
            var intent = _keyguardManager.CreateConfirmDeviceCredentialIntent((string)null, (string)null);
            var deviceAuthFragment = new DeviceAuthFragment(intent, deviceAuthTcs);

            FragmentTransaction addFragTx = Activity.FragmentManager.BeginTransaction();
            addFragTx.Add(deviceAuthFragment, "deviceAuth-fragment");
            addFragTx.Commit();

            var status = await deviceAuthTcs.Task;

            FragmentTransaction removeFragTx = Activity.FragmentManager.BeginTransaction();
            removeFragTx.Remove(deviceAuthFragment);
            removeFragTx.Commit();

            var result = new FingerprintAuthenticationResult 
            { 
                Status = status
            };

            return result;
        }

    }

    public class DeviceAuthFragment : Fragment
    {
        static readonly int ConfirmRequestId = 1;

        private Intent _deviceAuthIntent;
        TaskCompletionSource<FingerprintAuthenticationResultStatus> _deviceAuthTcs;

        public DeviceAuthFragment(Intent deviceAuthIntent, TaskCompletionSource<FingerprintAuthenticationResultStatus> deviceAuthTcs)
        {
            _deviceAuthIntent = deviceAuthIntent;
            _deviceAuthTcs = deviceAuthTcs;
        }

        public override void OnResume()
        {
            base.OnResume();

            if (_deviceAuthIntent != null && _deviceAuthTcs.Task.Status == TaskStatus.WaitingForActivation)
            {
                StartActivityForResult(_deviceAuthIntent, ConfirmRequestId);
            }
        }

        public override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            var status = FingerprintAuthenticationResultStatus.UnknownError;

            if (requestCode == ConfirmRequestId)
            {
                switch (resultCode)
                {
                    case Result.Ok:
                        status = FingerprintAuthenticationResultStatus.Succeeded;
                        break;
                    case Result.Canceled:
                        status = FingerprintAuthenticationResultStatus.Canceled;
                        break;
                }

            }

            _deviceAuthTcs.TrySetResult(status);

            base.OnActivityResult(requestCode, resultCode, data);
        }
    }
}

