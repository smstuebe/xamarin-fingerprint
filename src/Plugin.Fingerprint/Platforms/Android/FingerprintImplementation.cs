using System;
using System.Threading;
using System.Threading.Tasks;
using Android.App;
using Android.Hardware.Fingerprints;
using Java.Lang;
using Plugin.Fingerprint.Abstractions;
using Plugin.Fingerprint.Dialog;

namespace Plugin.Fingerprint
{
    public class FingerprintImplementation : FingerprintImplementationBase
    {
        private static Func<Activity> _activityResolver;
        private static Type _dialogFragmentType;

        public Activity CurrentActivity => GetCurrentActivity();
        
        public void SetCurrentActivityResolver(Func<Activity> activityResolver)
        {
            _activityResolver = activityResolver;
        }

        public static void SetDialogFragmentType<TFragment>() where TFragment : FingerprintDialogFragment
        {
            _dialogFragmentType = typeof (TFragment);
        }
        
        internal static FingerprintDialogFragment CreateDialogFragment()
        {
            _dialogFragmentType = _dialogFragmentType ?? typeof (FingerprintDialogFragment);
            return (FingerprintDialogFragment) Activator.CreateInstance(_dialogFragmentType);
        }

        private static Activity GetCurrentActivity()
        {
            if (_activityResolver == null)
                throw new InvalidOperationException("Resolver for the current activity is not set. Call Fingerprint.SetCurrentActivityResolver somewhere in your startup code.");

            return _activityResolver();
        }

        public override Task<FingerprintAvailability> GetAvailabilityAsync(bool allowAlternativeAuthentication = false)
        {
            throw new NotImplementedException();
        }

        public override Task<AuthenticationType> GetAuthenticationTypeAsync()
        {
            throw new NotImplementedException();
        }

        protected override Task<FingerprintAuthenticationResult> NativeAuthenticateAsync(AuthenticationRequestConfiguration authRequestConfig, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}