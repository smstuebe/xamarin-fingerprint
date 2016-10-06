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
#if PORTABLE
            throw NotImplementedInReferenceAssembly();
#else
            return new FingerprintImplementation();
#endif
        }

        public static void Dispose()
        {
            if (_implementation != null && _implementation.IsValueCreated)
            {
                _implementation = new Lazy<IFingerprint>(CreateFingerprint, LazyThreadSafetyMode.PublicationOnly);
            }
        }

        private static Exception NotImplementedInReferenceAssembly()
        {
            return new NotImplementedException("This functionality is not implemented in the portable version of this assembly. You should reference the NuGet package from your main application project in order to reference the platform-specific implementation.");
        }
    }
}