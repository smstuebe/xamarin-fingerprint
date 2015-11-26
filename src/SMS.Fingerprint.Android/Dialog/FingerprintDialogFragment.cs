using System.Threading;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Hardware.Fingerprints;
using Android.OS;
using Android.Views;
using SMS.Fingerprint.Abstractions;

namespace SMS.Fingerprint.Dialog
{
    public class FingerprintDialogFragment : DialogFragment
    {
        private TaskCompletionSource<FingerprintAuthenticationResult> _resultTaskCompletionSource;
        private FingerprintManager _fpService;

        public string Reason { get; private set; }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            _fpService = Fingerprint.GetService();
            RetainInstance = true;
            SetStyle(DialogFragmentStyle.Normal, 0);        
        }

        public async Task<FingerprintAuthenticationResult> ShowAsync(string reason, CancellationToken cancellationToken)
        {
            Reason = reason;

            var currentActivity = Fingerprint.CurrentActivity;
            Show(currentActivity.FragmentManager, "fingerprint-fragment");

            return await _resultTaskCompletionSource.Task;
        }

        public override void Show(FragmentManager manager, string tag)
        {
            _resultTaskCompletionSource = new TaskCompletionSource<FingerprintAuthenticationResult>();
            base.Show(manager, tag);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            Dialog.SetTitle(Reason);
            return new View(Context);
        }

        public override void OnDismiss(IDialogInterface dialog)
        {
            base.OnDismiss(dialog);
            _resultTaskCompletionSource.SetResult(new FingerprintAuthenticationResult
            {
                Status = FingerprintAuthenticationResultStatus.Canceled
            });
        }

        public override void OnResume()
        {
            base.OnResume();
            //if (mStage == Stage.FINGERPRINT) {
            //    mFingerprintUiHelper.startListening(mCryptoObject);
            //}
        }

        public override void OnPause()
        {
            base.OnPause();
            //mFingerprintUiHelper.stopListening();
        }

        //@Override
        //public void onAttach(Activity activity) {
        //    super.onAttach(activity);
        //    mActivity = (MainActivity) activity;
        //}
    }
}