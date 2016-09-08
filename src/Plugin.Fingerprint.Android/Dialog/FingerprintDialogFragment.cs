using System;
using System.Threading;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Views;
using Android.Views.Animations;
using Android.Widget;
using Plugin.Fingerprint.Abstractions;

namespace Plugin.Fingerprint.Dialog
{
    public class FingerprintDialogFragment : DialogFragment, IDialogAuthenticationListener, Animation.IAnimationListener
    {
        private TaskCompletionSource<FingerprintAuthenticationResult> _resultTaskCompletionSource;
        private Button _cancelButton;
        private Button _fallbackButton;
        private ImageView _icon;
        private bool _canceledByLifecycle;
        private CancellationTokenSource _cancelationTokenSource;
        private FingerprintAuthenticationCallback _callback;

        protected DialogConfiguration Configuration { get; private set; }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            RetainInstance = true;
            SetStyle(DialogFragmentStyle.NoTitle, 0);
            _callback = CreateAuthenticationCallback();
        }

        public async Task<FingerprintAuthenticationResult> ShowAsync(DialogConfiguration config, CancellationToken cancellationToken)
        {
            Configuration = config;

            var currentActivity = CrossFingerprint.CurrentActivity;
            Show(currentActivity.FragmentManager, "fingerprint-fragment");

            cancellationToken.Register(OnExternalCancel);

            return await _resultTaskCompletionSource.Task;
        }

        public override void Show(FragmentManager manager, string tag)
        {
            _resultTaskCompletionSource = new TaskCompletionSource<FingerprintAuthenticationResult>();
            base.Show(manager, tag);
        }

        private void SetManualResult(FingerprintAuthenticationResultStatus status)
        {
            _canceledByLifecycle = true;
            _cancelationTokenSource?.Cancel();

            Dismiss(new FingerprintAuthenticationResult
            {
                Status = status
            });
        }

        private void Dismiss(FingerprintAuthenticationResult result)
        {
            _resultTaskCompletionSource.SetResult(result);
            Dismiss();
        }

        private void OnExternalCancel()
        {
            SetManualResult(FingerprintAuthenticationResultStatus.Canceled);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.FingerprintDialog, container);
            view.FindViewById<TextView>(Resource.Id.fingerprint_txtReason).Text = Configuration.Reason;

            _cancelButton = view.FindViewById<Button>(Resource.Id.fingerprint_btnCancel);
            _fallbackButton = view.FindViewById<Button>(Resource.Id.fingerprint_btnFallback);
            _icon = view.FindViewById<ImageView>(Resource.Id.fingerprint_imgFingerprint);

            return view;
        }

        public override void OnDismiss(IDialogInterface dialog)
        {
            base.OnDismiss(dialog);
            _callback?.Dispose();
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
                _cancelButton.Text = string.IsNullOrWhiteSpace(Configuration.CancelTitle) 
                    ? Context.GetString(Android.Resource.String.Cancel)
                    : Configuration.CancelTitle;
                _cancelButton.Click += OnCancel;
            }

            if (_fallbackButton != null)
            {
                _fallbackButton.Text = string.IsNullOrWhiteSpace(Configuration.FallbackTitle)
                    ? "Use Fallback"
                    : Configuration.FallbackTitle;
                _fallbackButton.Click += OnFallback;
            }
            StartAuthenticationAsync();
        }

        private async void StartAuthenticationAsync()
        {
            _cancelationTokenSource = new CancellationTokenSource();
            _canceledByLifecycle = false;

            var result = await FingerprintImplementation.AuthenticateNoDialogAsync(_cancelationTokenSource.Token, _callback);

            if (!_canceledByLifecycle)
            {
                _cancelationTokenSource = null;
                Dismiss(result);
            }
        }

        private void OnCancel(object sender, EventArgs e)
        {
            SetManualResult(FingerprintAuthenticationResultStatus.Canceled);    
        }

        private void OnFallback(object sender, EventArgs e)
        {
            SetManualResult(FingerprintAuthenticationResultStatus.FallbackRequested);
        }

        public override void OnPause()
        {
            base.OnPause();

            if (_cancelButton != null)
            {
                _cancelButton.Click -= OnCancel;
            }

            if (_fallbackButton != null)
            {
                _fallbackButton.Click -= OnFallback;
            }

            _canceledByLifecycle = true;
            _cancelationTokenSource?.Cancel();
        }

        protected virtual FingerprintAuthenticationCallback CreateAuthenticationCallback()
        {
            return new DialogFingerprintAuthenticationCallback(this);
        }

        public virtual void OnFailedTry()
        {
            if (_icon != null)
            {
                var animation = new TranslateAnimation(-10, 10, 0, 0)
                {
                    Duration = 1000,
                    Interpolator = new CycleInterpolator(5)
                };

                animation.SetAnimationListener(this);
                _icon.StartAnimation(animation);
            }
        }

        void Animation.IAnimationListener.OnAnimationEnd(Animation animation)
        {
            _icon.ClearColorFilter();
        }

        void Animation.IAnimationListener.OnAnimationRepeat(Animation animation)
        {
        }

        void Animation.IAnimationListener.OnAnimationStart(Animation animation)
        {
            _icon.SetColorFilter(Color.Red);
        }
    }
}