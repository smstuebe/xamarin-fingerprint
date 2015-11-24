/*
* From Xamarin Plugin Template from James Montemagno @JamesMontemagno
*/

using System;
using SMS.Fingerprint.Abstractions;

namespace SMS.Fingerprint
{
    public class Fingerprint
    {
        private static Lazy<IFingerprint> _implementation = new Lazy<IFingerprint>(CreateFingerprint, System.Threading.LazyThreadSafetyMode.PublicationOnly);

        public static IFingerprint Current
        {
            get
            {
                var ret = _implementation.Value;
                if (ret == null)
                {
                    throw NotImplementedInReferenceAssembly();
                }
                return ret;
            }
        }

        static IFingerprint CreateFingerprint()
        {
#if PORTABLE
            return null;
#else
            return new FingerprintImplementation();
#endif
        }

        internal static Exception NotImplementedInReferenceAssembly()
        {
            return new NotImplementedException("This functionality is not implemented in the portable version of this assembly. You should reference the NuGet package from your main application project in order to reference the platform-specific implementation.");
        }

        public static void Dispose()
        {
            if (_implementation != null && _implementation.IsValueCreated)
            {
                //Implementation.Value.Dispose();
                _implementation = new Lazy<IFingerprint>(CreateFingerprint, System.Threading.LazyThreadSafetyMode.PublicationOnly);
            }
        }
    }

}