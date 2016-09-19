using System.Threading;
using System.Threading.Tasks;
using Android.App;
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

        private bool StartIdentify(SpassFingerprint.IIdentifyListener listener)
        {
            try
            {
                _spassFingerprint.StartIdentify(listener);
            }
            catch (Exception ex)
            {
                Log.Warn(nameof(SamsungFingerprintImplementation), ex);
                return false;
            }

            return true;
        }

        protected override bool CheckAvailability()
        {
            if (!IsCompatible)
                return false;

            return _spassFingerprint.HasRegisteredFinger;
        }
    }
}