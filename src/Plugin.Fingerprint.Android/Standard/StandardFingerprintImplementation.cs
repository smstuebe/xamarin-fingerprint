using System.Threading;
using System.Threading.Tasks;
using Android;
using Android.App;
using Android.Content.PM;
using Android.Hardware.Fingerprints;
using Android.OS;
using Java.Lang;
using Plugin.Fingerprint.Abstractions;
using Plugin.Fingerprint.Contract;

namespace Plugin.Fingerprint.Standard
{
    public class StandardFingerprintImplementation : AndroidFingerprintImplementationBase
    {
        public override async Task<FingerprintAuthenticationResult> AuthenticateNoDialogAsync(IAuthenticationFailedListener failedListener, CancellationToken cancellationToken)
        {
            using (var cancellationSignal = new CancellationSignal())
            using (cancellationToken.Register(() => cancellationSignal.Cancel()))
            {
                var callback = new FingerprintAuthenticationCallback(failedListener);
                GetService().Authenticate(null, cancellationSignal, FingerprintAuthenticationFlags.None, callback, null);
                return await callback.GetTask();
            }
        }

        private static FingerprintManager GetService()
        {
            return (FingerprintManager)Application.Context.GetSystemService(Class.FromType(typeof(FingerprintManager)));
        }

        public override FingerprintAvailability GetFingerprintAvailability()
        {
            if (Build.VERSION.SdkInt < BuildVersionCodes.M)
                return FingerprintAvailability.NoApi;

            var context = Application.Context;
            if (context.CheckCallingOrSelfPermission(Manifest.Permission.UseFingerprint) != Permission.Granted)
                return FingerprintAvailability.NoPermission;

            var fpService = (FingerprintManager)context.GetSystemService(Class.FromType(typeof(FingerprintManager)));
            if (!fpService.IsHardwareDetected)
                return FingerprintAvailability.NoSensor;

            if (!fpService.HasEnrolledFingerprints)
                return FingerprintAvailability.NoFingerprint;

            return FingerprintAvailability.Available;
        }
    }
}