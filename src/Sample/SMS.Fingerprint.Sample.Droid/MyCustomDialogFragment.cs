using System;
using System.Threading;
using System.Threading.Tasks;
using Android.Animation;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Views;
using Android.Views.Animations;
using Android.Widget;
using Plugin.Fingerprint;
using Plugin.Fingerprint.Abstractions;
using Plugin.Fingerprint.Dialog;
using Plugin.Fingerprint.Utils;
using Xamarin.Forms.Platform.Android;
using Resource = Xamarin.Forms.Platform.Android.Resource;

namespace SMS.Fingerprint.Sample.Droid
{
    public class MyCustomDialogFragment : FingerprintDialogFragment
    {
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = base.OnCreateView(inflater, container, savedInstanceState);
            view.Background = new ColorDrawable(Color.Magenta);
            return view;
        }
    }

    public class ZetagikCustomFingerprintDialogFragment : FingerprintDialogFragment
    {
        TaskCompletionSource<FingerprintAuthenticationResult> _resultTaskCompletionSource;
        TextView helpText;
        Button cancelButton;
        Button fallbackButton;
        ImageView icon;
        bool canceledByLifecycle;
        CancellationTokenSource cancelationTokenSource;

        protected new Color? DefaultColor = Xamarin.Forms.Color.FromHex("#43BBEA").ToAndroid();

        public ZetagikCustomFingerprintDialogFragment()
        {
        }

        public ZetagikCustomFingerprintDialogFragment(IntPtr ptr, JniHandleOwnership owner) : base(ptr, owner)
        {
        }

        public override Dialog OnCreateDialog(Bundle savedInstanceState)
        {
            var dialog = new BottomSheetDialog(Context);
            dialog.SetOnKeyListener(this);
            Cancelable = false;
            return dialog;
        }

        protected void DetachEventHandlers()
        {
            if (cancelButton != null)
                cancelButton.Click -= OnCancel;

            if (fallbackButton != null)
                fallbackButton.Click -= OnFallback;
        }

        void SetManualResult(FingerprintAuthenticationResultStatus status, bool animation = true)
        {
            canceledByLifecycle = true;
            cancelationTokenSource?.Cancel();

            Dismiss(new FingerprintAuthenticationResult
            {
                Status = status
            }, animation);
        }

        async void Dismiss(FingerprintAuthenticationResult result, bool animation = true)
        {
            DetachEventHandlers();
            if (animation)
            {
                ShowHelpText();
                await AnimateResultAsync(result.Status);
            }

            _resultTaskCompletionSource.TrySetResult(result);
            DismissAllowingStateLoss();
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.FingerprintDialogContent, null);

            icon = view.FindViewById<ImageView>(Resource.Id.biometricIcon);
            icon.SetImageDrawable(Context.GetDrawable(Resource.Drawable.fingerprint_white));

            var reason = view.FindViewById<TextView>(Resource.Id.reasonText);
            reason.Text = "some dynamic reason";

            helpText = view.FindViewById<TextView>(Resource.Id.helpText);

            if (DefaultColor.HasValue)
                icon?.SetColorFilter(DefaultColor.Value);

            return view;
        }

        public void OnCancel(object sender, EventArgs e) => SetManualResult(FingerprintAuthenticationResultStatus.Canceled);

        void OnFallback(object sender, EventArgs e) => SetManualResult(FingerprintAuthenticationResultStatus.FallbackRequested);

        public override async void OnFailedTry()
        {
            ShowHelpText();
            await AnimateFailedTryAsync();
        }

        void ShowHelpText()
        {
            if (helpText != null)
            {
                helpText.Text = "bla bla";
                helpText.Visibility = ViewStates.Visible;
            }
        }

        async Task AnimateResultAsync(FingerprintAuthenticationResultStatus status)
        {
            switch (status)
            {
                case FingerprintAuthenticationResultStatus.Succeeded:
                    await FinalAnimationAsync(PositiveColor);
                    break;
                case FingerprintAuthenticationResultStatus.FallbackRequested:
                    await FallbackAnimationAsync();
                    break;
                case FingerprintAuthenticationResultStatus.Canceled:
                case FingerprintAuthenticationResultStatus.Unknown:
                case FingerprintAuthenticationResultStatus.Failed:
                case FingerprintAuthenticationResultStatus.UnknownError:
                case FingerprintAuthenticationResultStatus.NotAvailable:
                case FingerprintAuthenticationResultStatus.Denied:
                    await FinalAnimationAsync(NegativeColor);
                    break;
                case FingerprintAuthenticationResultStatus.TooManyAttempts:
                    break;
            }
        }

        async Task FinalAnimationAsync(Color color)
        {
            if (icon == null)
                return;

            icon.SetColorFilter(color);
            var press = ObjectAnimator.OfPropertyValuesHolder(icon, PropertyValuesHolder.OfFloat("scaleX", 0.7f), PropertyValuesHolder.OfFloat("scaleY", 0.7f));
            press.SetDuration(300);
            press.RepeatMode = ValueAnimatorRepeatMode.Reverse;
            press.RepeatCount = 1;
            await press.StartAsync();
        }

        async Task AnimateFailedTryAsync()
        {
            if (icon == null)
                return;

            icon.SetColorFilter(NegativeColor);
            var shake = ObjectAnimator.OfFloat(icon, "translationX", -10f, 10f);
            shake.SetDuration(500);
            shake.SetInterpolator(new CycleInterpolator(5));
            await shake.StartAsync();

            if (DefaultColor.HasValue)
                icon?.SetColorFilter(DefaultColor.Value);
            else
                icon?.ClearColorFilter();
        }

        async Task FallbackAnimationAsync()
        {
            if (icon == null)
                return;

            var rotate = ObjectAnimator.OfFloat(icon, "rotationY", 0f, 180f);
            var fade = ObjectAnimator.OfFloat(icon, "alpha", 1f, 0f);
            var scale = ObjectAnimator.OfPropertyValuesHolder(icon, PropertyValuesHolder.OfFloat("scaleX", 0.4f), PropertyValuesHolder.OfFloat("scaleY", 0.4f));
            rotate.SetDuration(200);
            fade.SetDuration(600);
            scale.SetDuration(600);

            var animation = new AnimatorSet();
            animation.Play(rotate).Before(scale);
            animation.Play(fade).With(scale);
            await animation.StartAsync();
        }
    }
}