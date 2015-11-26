using System;
using Android.App;
using Android.Hardware.Fingerprints;
using Java.Lang;

namespace SMS.Fingerprint
{
    public partial class Fingerprint
    {
        private static Func<Activity> _activityResolver;

        public static Activity CurrentActivity => GetCurrentActivity();
        public static bool DialogEnabled { get; set; }
        
        public static void SetCurrentActivityResolver(Func<Activity> activityResolver)
        {
            _activityResolver = activityResolver;
        }

        internal static FingerprintManager GetService()
        {
            return (FingerprintManager)CurrentActivity.GetSystemService(Class.FromType(typeof(FingerprintManager)));
        }

        private static Activity GetCurrentActivity()
        {
            if (_activityResolver == null)
                throw new InvalidOperationException("Resolver for the current activity is not set. Call Fingerprint.SetCurrentActivityResolver somewhere in your startup code.");

            return _activityResolver();
        }
    }
}