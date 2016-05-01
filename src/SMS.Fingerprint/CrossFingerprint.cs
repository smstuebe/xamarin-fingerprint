using System;
using System.Threading;
using Plugin.Fingerprint.Abstractions;

namespace Plugin.Fingerprint
{
    public partial class CrossFingerprint
    {
        private static Lazy<IFingerprint> _implementation = new Lazy<IFingerprint>(CreateFingerprint, LazyThreadSafetyMode.PublicationOnly);
        public static IFingerprint Current => _implementation.Value;

        static IFingerprint CreateFingerprint()
        {
            return new FingerprintImplementation();
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