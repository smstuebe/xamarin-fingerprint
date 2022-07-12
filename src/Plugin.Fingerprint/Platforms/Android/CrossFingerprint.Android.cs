using System;
using System.Runtime.Versioning;
using Android.App;

namespace Plugin.Fingerprint
{
    public partial class CrossFingerprint
    {
#if NET6_0_ANDROID
        [SupportedOSPlatform("android21.0")]
#endif
        private static Func<Activity> _activityResolver;

#if NET6_0_ANDROID
        [SupportedOSPlatform("android21.0")]
#endif
        public static Activity CurrentActivity => GetCurrentActivity();

#if NET6_0_ANDROID
        [SupportedOSPlatform("android21.0")]
#endif
        public static void SetCurrentActivityResolver(Func<Activity> activityResolver)
        {
            _activityResolver = activityResolver;
        }

#if NET6_0_ANDROID
        [SupportedOSPlatform("android21.0")]
#endif
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