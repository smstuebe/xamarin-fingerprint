using System.Threading;
using System.Threading.Tasks;
using Android.App;
using Android.Util;
using Com.Samsung.Android.Sdk.Pass;
using Java.Lang;
using Plugin.Fingerprint.Abstractions;

namespace Plugin.Fingerprint.Samsung
{
    public class SamsungFingerprintImplementation : FingerprintImplementationBase
    {
        private readonly bool _couldInitialize;
        private readonly bool _hasFingerPrintSensor;
        private readonly SpassFingerprint _spassFingerprint;

        public override bool IsAvailable => CheckAvailability();
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

        public override async Task<FingerprintAuthenticationResult> AuthenticateAsync(DialogConfiguration dialogConfig, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (!IsAvailable)
            {
                return new FingerprintAuthenticationResult { Status = FingerprintAuthenticationResultStatus.NotAvailable };
            }

            using (cancellationToken.Register(() => _spassFingerprint.CancelIdentify()))
            {
                var identifyListener = new IdentifyListener();
                try
                {
                    _spassFingerprint.StartIdentify(identifyListener);
                }
                catch (Exception ex)
                {
                    Log.Warn(nameof(SamsungFingerprintImplementation), ex);
                    return new FingerprintAuthenticationResult { Status = FingerprintAuthenticationResultStatus.Failed };
                }

                return await identifyListener.GetTask();
            }
        }

        private bool CheckAvailability()
        {
            if (!IsCompatible)
                return false;

            return _spassFingerprint.HasRegisteredFinger;
        }
    }
}