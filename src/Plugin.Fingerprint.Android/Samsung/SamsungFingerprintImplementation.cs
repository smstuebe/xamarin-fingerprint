using System.Threading;
using System.Threading.Tasks;
using Android.App;
using Android.Support.V4.Content;
using Android.Util;
using Com.Samsung.Android.Sdk.Pass;
using Java.Lang;
using Plugin.Fingerprint.Abstractions;
using Plugin.Fingerprint.Contract;

namespace Plugin.Fingerprint.Samsung
{
    public class SamsungFingerprintImplementation : AndroidFingerprintImplementationBase
    {
        private readonly bool _couldInitialize;
        private readonly bool _hasFingerPrintSensor;
        private readonly SpassFingerprint _spassFingerprint;

        internal bool IsCompatible => _couldInitialize && _hasFingerPrintSensor;

        public SamsungFingerprintImplementation()
        {
            try
            {
                var spass = new Spass();
                spass.Initialize(Application.Context);
                _couldInitialize = true;
                _hasFingerPrintSensor = spass.IsFeatureEnabled(Spass.DeviceFingerprint);
                _spassFingerprint = new SpassFingerprint(Application.Context);
            }
            catch (Exception ex)
            {
                Log.Warn(nameof(SamsungFingerprintImplementation), ex);
            }
        }

        public override async Task<FingerprintAuthenticationResult> AuthenticateNoDialogAsync(IAuthenticationFailedListener failedListener, CancellationToken cancellationToken)
        {
            using (cancellationToken.Register(() => _spassFingerprint.CancelIdentify()))
            {
                var identifyListener = new IdentifyListener(StartIdentify, failedListener);
                return await identifyListener.GetTask();
            }
        }

        private async Task<bool> StartIdentify(SpassFingerprint.IIdentifyListener listener)
        {
            // TODO: remove retry and delay, if samsung fixes the library 
            for (var i = 0; i < 5; i++)
            {
                try
                {
                    _spassFingerprint.StartIdentify(listener);
                    return true;
                }
                catch (IllegalStateException ex)
                {
                    Log.Warn(nameof(SamsungFingerprintImplementation), ex);
                    await Task.Delay(100);
                }
                catch (Exception ex)
                {
                    Log.Warn(nameof(SamsungFingerprintImplementation), ex);
                    return false;
                }
            }

            return false;
        }

        protected override bool CheckAvailability()
        {
            if (!IsCompatible)
                return false;

            return _spassFingerprint.HasRegisteredFinger;
        }
    }
}