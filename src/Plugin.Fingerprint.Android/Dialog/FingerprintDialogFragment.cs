using System;
using System.Threading;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using Plugin.Fingerprint;
using Plugin.Fingerprint.Abstractions;

namespace SMS.Fingerprint.Dialog
{
    public class FingerprintDialogFragment : DialogFragment
    {
        private TaskCompletionSource<FingerprintAuthenticationResult> _resultTaskCompletionSource;
        private Button _cancelButton;
        private bool _canceledByLifecycle;
        private CancellationTokenSource _cancelationTokenSource;
        private bool _hasResult;

        public string Reason { get; private set; }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            RetainInstance = true;
            SetStyle(DialogFragmentStyle.NoTitle, 0);
        }

        public async Task<FingerprintAuthenticationResult> ShowAsync(string reason, CancellationToken cancellationToken)
        {
            Reason = reason;

            var currentActivity = CrossFingerprint.CurrentActivity;
            Show(currentActivity.FragmentManager, "fingerprint-fragment");

            cancellationToken.Register(Dismiss);

            return await _resultTaskCompletionSource.Task;
        }

        public override void Show(FragmentManager manager, string tag)
        {
            _resultTaskCompletionSource = new TaskCompletionSource<FingerprintAuthenticationResult>();
            base.Show(manager, tag);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.FingerprintDialog, container);
            view.FindViewById<TextView>(Resource.Id.fingerprint_txtReason).Text = Reason;

            _cancelButton = view.FindViewById<Button>(Resource.Id.fingerprint_btnCancel);

            return view;
        }
        
        public override void OnDismiss(IDialogInterface dialog)
        {
            base.OnDismiss(dialog);

            if (!_hasResult)
            {
                _resultTaskCompletionSource.SetResult(new FingerprintAuthenticationResult
                {
                    Status = FingerprintAuthenticationResultStatus.Canceled
                });
            }
        }

        public override void OnResume()
        {
            base.OnResume();

            var param = Dialog.Window.Attributes;
            param.Width = ViewGroup.LayoutParams.MatchParent;
            param.Height = ViewGroup.LayoutParams.WrapContent;
            Dialog.Window.Attributes = param;

            if (_cancelButton != null)
            {
                _cancelButton.Click += OnCancel;
            }

            StartAuthenticationAsync();
        }

        private async void StartAuthenticationAsync()
        {
            _cancelationTokenSource = new CancellationTokenSource();
            _canceledByLifecycle = false;
            _hasResult = false;

            var result = await FingerprintImplementation.AuthenticateNoDialogAsync(_cancelationTokenSource.Token);

            if (!_canceledByLifecycle)
            {
                _cancelationTokenSource = null;
                _hasResult = true;
                Dismiss();
                _resultTaskCompletionSource.SetResult(result);    
            }
        }

        private void OnCancel(object sender, EventArgs e)
        {
            Dismiss();    
        }

        public override void OnPause()
        {
            base.OnPause();

            if (_cancelButton != null)
            {
                _cancelButton.Click -= OnCancel;
            }

            _canceledByLifecycle = true;
            _cancelationTokenSource?.Cancel();
        }
    }
}