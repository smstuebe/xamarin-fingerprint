using System;
using System.Threading;
using Plugin.Fingerprint.Abstractions;
using Plugin.Fingerprint.Samsung;
using Plugin.Fingerprint.Standard;

namespace Plugin.Fingerprint
{
    public partial class CrossFingerprint
    {
        private static Lazy<IFingerprint> _implementation = new Lazy<IFingerprint>(CreateFingerprint, LazyThreadSafetyMode.PublicationOnly);
        public static IFingerprint Current => _implementation.Value;

        private static IFingerprint CreateFingerprint()
        {
            if (Android.OS.Build.VERSION.SdkInt < Android.OS.BuildVersionCodes.P)
            {
                var samsungFp = new SamsungFingerprintImplementation();
                if (samsungFp.IsCompatible)
                    return samsungFp;
            }

            return new StandardFingerprintImplementation();
        }

        public static void Dispose()
        {
            if (_implementation != null && _implementation.IsValueCreated)
            {
                _implementation = new Lazy<IFingerprint>(CreateFingerprint, LazyThreadSafetyMode.PublicationOnly);
            }
        }
    }
}