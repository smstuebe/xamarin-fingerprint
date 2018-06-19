using System;
using System.Threading;
using Plugin.Fingerprint.Abstractions;
#if ANDROID
using Plugin.Fingerprint.Samsung;
using Plugin.Fingerprint.Standard;
#endif

namespace Plugin.Fingerprint
{
    /// <summary>
    /// Cross Platform Fingerprint.
    /// </summary>
    public partial class CrossFingerprint
    {
        private static Lazy<IFingerprint> _implementation = new Lazy<IFingerprint>(CreateFingerprint, LazyThreadSafetyMode.PublicationOnly);

        /// <summary>
        /// Current plugin implementation to use
        /// </summary>
        public static IFingerprint Current
        {
            get => _implementation.Value;
            set
            {
                _implementation = new Lazy<IFingerprint>(() => value);
            }
        }

        static IFingerprint CreateFingerprint()
        {
#if NETSTANDARD2_0
            throw NotImplementedInReferenceAssembly();
#elif ANDROID
            var samsungFp = new SamsungFingerprintImplementation();

            if (samsungFp.IsCompatible)
                return samsungFp;

            return new StandardFingerprintImplementation();
#else
            return new FingerprintImplementation();
#endif
        }

        /// <summary>
        /// Cleans up implementation reference.
        /// </summary>
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