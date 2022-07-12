using System;
using System.Runtime.Versioning;
using System.Threading;
using Plugin.Fingerprint.Abstractions;
#if ANDROID
using Plugin.Fingerprint.Contract;
#endif

namespace Plugin.Fingerprint
{
    /// <summary>
    /// Cross Platform Fingerprint.
    /// </summary>
    public partial class CrossFingerprint
    {
#if NET6_0_ANDROID
        [SupportedOSPlatform("android23.0")]
#elif NET6_0_IOS
        [SupportedOSPlatform("ios10.0")]
#elif NET6_0_MACCATALYST
        [SupportedOSPlatform("maccatalyst10.0")]
#endif
        private static Lazy<IFingerprint> _implementation = new Lazy<IFingerprint>(CreateFingerprint, LazyThreadSafetyMode.PublicationOnly);

        /// <summary>
        /// Current plugin implementation to use
        /// </summary>
#if NET6_0_ANDROID
        [SupportedOSPlatform("android23.0")]
#elif NET6_0_IOS
        [SupportedOSPlatform("ios10.0")]
#elif NET6_0_MACCATALYST
        [SupportedOSPlatform("maccatalyst10.0")]
#endif
        public static IFingerprint Current
        {
            get => _implementation.Value;
            set
            {
                _implementation = new Lazy<IFingerprint>(() => value);
            }
        }

#if NET6_0_ANDROID
        [SupportedOSPlatform("android23.0")]
#elif NET6_0_IOS
        [SupportedOSPlatform("ios10.0")]
#elif NET6_0_MACCATALYST
        [SupportedOSPlatform("maccatalyst10.0")]
#endif
        static IFingerprint CreateFingerprint()
        {
#if NETSTANDARD2_0
            throw NotImplementedInReferenceAssembly();
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