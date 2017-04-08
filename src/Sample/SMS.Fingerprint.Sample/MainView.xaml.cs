using System;
using System.Threading;
using System.Threading.Tasks;
using Plugin.Fingerprint.Abstractions;
using Xamarin.Forms;
using System.Collections.Generic;

namespace SMS.Fingerprint.Sample
{
    public partial class MainView : ContentPage
    {
        private CancellationTokenSource _cancel;
        private string _serviceId = Guid.NewGuid().ToString();
        private string _secureValueKey = "Password";

        public MainView()
        {
            InitializeComponent();
        }

        private async void OnAuthenticate(object sender, EventArgs e)
        {
            _cancel = swAutoCancel.IsToggled ? new CancellationTokenSource(TimeSpan.FromSeconds(10)) : new CancellationTokenSource();
            lblAuthenticateStatus.Text = "";
            var result = await Plugin.Fingerprint.CrossFingerprint.Current.AuthenticateAsync("Prove you have fingers!", _cancel.Token);

            await SetResultAsync(result);
        }

        private async void OnAuthenticateLocalized(object sender, EventArgs e)
        {
            _cancel = swAutoCancel.IsToggled ? new CancellationTokenSource(TimeSpan.FromSeconds(10)) : new CancellationTokenSource();
            lblAuthenticateStatus.Text = "";

            var dialogConfig = new AuthenticationRequestConfiguration("Beweise, dass du Finger hast!")
            {
                CancelTitle = "Abbrechen",
                FallbackTitle = "Anders!"
            };

            var result = await Plugin.Fingerprint.CrossFingerprint.Current.AuthenticateAsync(dialogConfig, _cancel.Token);

            await SetResultAsync(result);
        }

        private async Task SetResultAsync(FingerprintAuthenticationResult result)
        {
            if (result.Authenticated)
            {
                await Navigation.PushAsync(new SecretView());
            }
            else
            {
                lblAuthenticateStatus.Text = $"{result.Status}: {result.ErrorMessage}";
            }
        }

        private async void OnAddSecureValue(object sender, EventArgs e)
        {
            var result = await Plugin.Fingerprint.CrossFingerprint.Current.SetSecureValue(
                _serviceId, new KeyValuePair<string, string>(_secureValueKey, entSecureValue.Text));

            lblSecureValueStatus.Text = $"{result.Status}: {result.ErrorMessage}";
        }

        private async void OnRemoveSecureValue(object sender, EventArgs e)
        {
            var result = await Plugin.Fingerprint.CrossFingerprint.Current.RemoveSecureValue(
                _serviceId, _secureValueKey);

            lblSecureValueStatus.Text = $"{result.Status}: {result.ErrorMessage}";
        }

        private async void OnGetSecureValue(object sender, EventArgs e)
        {
            var result = await Plugin.Fingerprint.CrossFingerprint.Current.GetSecureValue(
               _serviceId, _secureValueKey, "Please authenticate with your fingerprint to continue.");

            lblSecureValueStatus.Text = $"{result.Status}: {result.ErrorMessage}";
        }
    }
}