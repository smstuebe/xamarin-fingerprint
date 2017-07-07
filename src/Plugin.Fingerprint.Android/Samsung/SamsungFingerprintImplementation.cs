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
        private readonly bool _hasNoApi;
        private readonly bool _hasNoPermission;
        private readonly bool _hasNoFingerPrintSensor;
        private readonly SpassFingerprint _spassFingerprint;

        internal bool IsCompatible { get; }

        public SamsungFingerprintImplementation()
        {
            try
            {
                var spass = new Spass();
                spass.Initialize(Application.Context);
                _hasNoFingerPrintSensor = !spass.IsFeatureEnabled(Spass.DeviceFingerprint);
                _spassFingerprint = new SpassFingerprint(Application.Context);
                IsCompatible = true;
            }
            catch (SecurityException ex)
            {
                Log.Warn(nameof(SamsungFingerprintImplementation), ex);
                _hasNoPermission = true;
            }
            catch (Exception ex)
            {
                Log.Warn(nameof(SamsungFingerprintImplementation), ex);
                _hasNoApi = true;
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

        public override async Task<FingerprintAvailability> GetAvailabilityAsync(bool allowAlternativeAuthentication = false)
        {
            if (_hasNoApi)
                return FingerprintAvailability.NoApi;

            if(_hasNoPermission)
                return FingerprintAvailability.NoPermission;

            if (_hasNoFingerPrintSensor)
                return FingerprintAvailability.NoSensor;

            try
            {
                // On some devices, Samsung doesn't fulfill the API contract of IsFeatureEnabled.
                // This will cause a UnsupportedOperationException when calling HasRegisteredFinger see #53, #70
                if (!_spassFingerprint.HasRegisteredFinger)
                    return FingerprintAvailability.NoFingerprint;
            }
            catch (UnsupportedOperationException ex)
            {
                Log.Warn(nameof(SamsungFingerprintImplementation), ex);
                return FingerprintAvailability.NoSensor;
            }
            catch (Exception ex)
            {
                Log.Warn(nameof(SamsungFingerprintImplementation), ex);
                return FingerprintAvailability.Unknown;
            }

            return FingerprintAvailability.Available;
        }
    }
}