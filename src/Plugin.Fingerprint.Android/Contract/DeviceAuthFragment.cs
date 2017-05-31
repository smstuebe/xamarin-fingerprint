using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Plugin.Fingerprint.Abstractions;

namespace Plugin.Fingerprint.Contract
{
    public class DeviceAuthFragment : Fragment
    {
        private const int ConfirmRequestId = 1;

        private readonly Intent _deviceAuthIntent;
        readonly TaskCompletionSource<FingerprintAuthenticationResultStatus> _deviceAuthTcs;

        public DeviceAuthFragment(Intent deviceAuthIntent, TaskCompletionSource<FingerprintAuthenticationResultStatus> deviceAuthTcs)
        {
            _deviceAuthIntent = deviceAuthIntent;
            _deviceAuthTcs = deviceAuthTcs;
        }

        public override void OnResume()
        {
            base.OnResume();

            if (_deviceAuthIntent != null && _deviceAuthTcs.Task.Status == TaskStatus.WaitingForActivation)
            {
                StartActivityForResult(_deviceAuthIntent, ConfirmRequestId);
            }
        }

        public override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            var status = FingerprintAuthenticationResultStatus.UnknownError;

            if (requestCode == ConfirmRequestId)
            {
                switch (resultCode)
                {
                    case Result.Ok:
                        status = FingerprintAuthenticationResultStatus.Succeeded;
                        break;
                    case Result.Canceled:
                        status = FingerprintAuthenticationResultStatus.Canceled;
                        break;
                }

            }

            _deviceAuthTcs.TrySetResult(status);

            base.OnActivityResult(requestCode, resultCode, data);
        }
    }
}