using System;
using Android.App;

namespace Plugin.Fingerprint
{
    public partial class CrossFingerprint
    {
        private static Func<Activity> _activityResolver;
        private static Type _dialogFragmentType;

        public static Activity CurrentActivity => GetCurrentActivity();
        
        public static void SetCurrentActivityResolver(Func<Activity> activityResolver)
        {
            _activityResolver = activityResolver;
        }

        //public static void SetDialogFragmentType<TFragment>() where TFragment : FingerprintDialogFragment
        //{
        //    _dialogFragmentType = typeof (TFragment);
        //}
        
        //internal static FingerprintDialogFragment CreateDialogFragment()
        //{
        //    _dialogFragmentType = _dialogFragmentType ?? typeof (FingerprintDialogFragment);
        //    return (FingerprintDialogFragment) Activator.CreateInstance(_dialogFragmentType);
        //}

        private static Activity GetCurrentActivity()
        {
            if (_activityResolver == null)
                throw new InvalidOperationException("Resolver for the current activity is not set. Call Fingerprint.SetCurrentActivityResolver somewhere in your startup code.");

            return _activityResolver();
        }
    }
}