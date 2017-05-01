using System;
using Android.App;
using Android.Hardware.Fingerprints;
using Java.Lang;
using Plugin.Fingerprint.Dialog;

namespace Plugin.Fingerprint
{
    public partial class CrossFingerprint
    {
        private static Func<Activity> _activityResolver;
        private static Type _dialogFragmentType;
        private static Type _secureValueFragmentType;

        public static Activity CurrentActivity => GetCurrentActivity();
        
        public static void SetCurrentActivityResolver(Func<Activity> activityResolver)
        {
            _activityResolver = activityResolver;
        }

        public static void SetDialogFragmentType<TFragment>() where TFragment : FingerprintDialogFragment
        {
            _dialogFragmentType = typeof (TFragment);
        }

        public static void SetSecureValueDialogFragmentType<TFragment>() where TFragment : FingerprintSecureValueDialogFragment
        {
            _secureValueFragmentType = typeof(TFragment);
        }

        internal static FingerprintDialogFragment CreateDialogFragment()
        {
            _dialogFragmentType = _dialogFragmentType ?? typeof (FingerprintDialogFragment);
            return (FingerprintDialogFragment) Activator.CreateInstance(_dialogFragmentType);
        }

        internal static FingerprintSecureValueDialogFragment CreateSecureValueDialogFragment()
        {
            _secureValueFragmentType = _secureValueFragmentType ?? typeof(FingerprintSecureValueDialogFragment);
            return (FingerprintSecureValueDialogFragment)Activator.CreateInstance(_secureValueFragmentType);
        }

        private static Activity GetCurrentActivity()
        {
            if (_activityResolver == null)
                throw new InvalidOperationException("Resolver for the current activity is not set. Call Fingerprint.SetCurrentActivityResolver somewhere in your startup code.");

            return _activityResolver();
        }
    }
}