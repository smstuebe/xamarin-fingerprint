using System.Threading;
using System.Threading.Tasks;
using Android;
using Android.App;
using Android.Content.PM;
using Android.Hardware.Fingerprints;
using Android.OS;
using Java.Lang;
using Plugin.Fingerprint.Abstractions;

namespace Plugin.Fingerprint
{
    internal class FingerprintImplementation : IFingerprint
    {
        public bool IsAvailable => CheckAvailability();

        public Task<FingerprintAuthenticationResult> AuthenticateAsync(string reason)
        {
            return AuthenticateAsync(reason, new CancellationToken());
        }

        public async Task<FingerprintAuthenticationResult> AuthenticateAsync(string reason, CancellationToken cancellationToken)
        {
            if (!IsAvailable)
            {
                return new FingerprintAuthenticationResult { Status = FingerprintAuthenticationResultStatus.NotAvailable };
            }

            if (CrossFingerprint.DialogEnabled)
            {
                var fragment = CrossFingerprint.CreateDialogFragment();
                return await fragment.ShowAsync(reason, cancellationToken);
            }

            return await AuthenticateNoDialogAsync(cancellationToken, new FingerprintAuthenticationCallback());
        }

        internal static async Task<FingerprintAuthenticationResult> AuthenticateNoDialogAsync(CancellationToken cancellationToken, FingerprintAuthenticationCallback callback)
        {
            var cancellationSignal = new CancellationSignal();
            cancellationToken.Register(() => cancellationSignal.Cancel());

            CrossFingerprint.GetService().Authenticate(null, cancellationSignal, FingerprintAuthenticationFlags.None, callback, null);

            return await callback.GetTask();
        }

        private bool CheckAvailability()
        {
            if (Build.VERSION.SdkInt < BuildVersionCodes.M)
                return false;

            var context = Application.Context;
            if (context.CheckCallingOrSelfPermission(Manifest.Permission.UseFingerprint) != Permission.Granted)
                return false;

            var fpService = (FingerprintManager)context.GetSystemService(Class.FromType(typeof(FingerprintManager)));
            if (!fpService.IsHardwareDetected)
                return false;

            if (!fpService.HasEnrolledFingerprints)
                return false;

            return true;
        }
    }
}
