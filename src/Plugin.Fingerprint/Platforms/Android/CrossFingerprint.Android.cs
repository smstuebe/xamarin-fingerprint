using System;
using Android.App;
using Plugin.Fingerprint.Platforms.Android.Utils;

namespace Plugin.Fingerprint
{
    public partial class CrossFingerprint
    {
        private static Func<Activity> _activityResolver;

        public static Activity CurrentActivity => GetCurrentActivity();

        public static CryptoSettings CryptoSettings { get; set; }

        public static void SetCurrentActivityResolver(Func<Activity> activityResolver)
        {
            _activityResolver = activityResolver;
        }

        private static Activity GetCurrentActivity()
        {
            if (_activityResolver is null)
                throw new InvalidOperationException("Resolver for the current activity is not set. Call Fingerprint.SetCurrentActivityResolver somewhere in your startup code.");

            var activity = _activityResolver();
            if (activity is null)
                throw new InvalidOperationException("The configured CurrentActivityResolver returned null. " +
                                                    "You need to setup the Android implementation via CrossFingerprint.SetCurrentActivityResolver(). " +
                                                    "If you are using CrossCurrentActivity don't forget to initialize it, too!");

            return activity;
        }
    }
}