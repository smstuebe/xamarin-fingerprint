using System.Threading;
using System.Threading.Tasks;
using Android;
using Android.App;
using Android.Content.PM;
using Android.Hardware.Fingerprints;
using Android.OS;
using Android.Util;
using Java.Lang;
using Plugin.Fingerprint.Abstractions;
using Plugin.Fingerprint.Contract;

namespace Plugin.Fingerprint.Standard
{
    //public class StandardFingerprintImplementation : AndroidFingerprintImplementationBase
    //{
    //    public async Task<FingerprintAuthenticationResult> AuthenticateNoDialogAsync(IAuthenticationFailedListener failedListener, CancellationToken cancellationToken)
    //    {
    //        using (var cancellationSignal = new CancellationSignal())
    //        using (cancellationToken.Register(() => cancellationSignal.Cancel()))
    //        {
    //            var callback = new FingerprintAuthenticationCallback(failedListener);
    //            GetService().Authenticate(null, cancellationSignal, FingerprintAuthenticationFlags.None, callback, null);
    //            return await callback.GetTask();
    //        }
    //    }

    //    private static FingerprintManager GetService()
    //    {
    //        return (FingerprintManager)Application.Context.GetSystemService(Class.FromType(typeof(FingerprintManager)));
    //    }

    //    public override async Task<FingerprintAvailability> GetAvailabilityAsync(bool allowAlternativeAuthentication = false)
    //    {
    //        if (Build.VERSION.SdkInt < BuildVersionCodes.M)
    //            return FingerprintAvailability.NoApi;

    //        var context = Application.Context;
    //        if (context.CheckCallingOrSelfPermission(Manifest.Permission.UseFingerprint) != Permission.Granted)
    //            return FingerprintAvailability.NoPermission;
            
    //        try
    //        {
    //            // service can be null certain devices #83
    //            var fpService = GetService();
    //            if (fpService == null)
    //                return FingerprintAvailability.NoApi;

    //            if (!fpService.IsHardwareDetected)
    //                return FingerprintAvailability.NoSensor;

    //            if (!fpService.HasEnrolledFingerprints)
    //                return FingerprintAvailability.NoFingerprint;

    //            return FingerprintAvailability.Available;
    //        }
    //        catch(Throwable e)
    //        {
    //            // ServiceNotFoundException can happen on certain devices #83
    //            Log.Error(nameof(StandardFingerprintImplementation), e, "Could not create Android service");
    //            return FingerprintAvailability.Unknown;
    //        }
    //    }

    //    protected override Task<FingerprintAuthenticationResult> NativeAuthenticateAsync(AuthenticationRequestConfiguration authRequestConfig, CancellationToken cancellationToken)
    //    {
    //        throw new System.NotImplementedException();
    //    }
    //}
}