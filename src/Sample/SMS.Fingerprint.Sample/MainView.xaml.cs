using System;
using System.Threading;
using Xamarin.Forms;

namespace SMS.Fingerprint.Sample
{
    public partial class MainView : ContentPage
    {
        private CancellationTokenSource _cancel;

        public MainView()
        {
            InitializeComponent();
        }

        private async void OnAuthenticate(object sender, EventArgs e)
        {
            _cancel = swAutoCancel.IsToggled ? new CancellationTokenSource(TimeSpan.FromSeconds(10)) : new CancellationTokenSource();
            lblStatus.Text = "";
            var result = await Fingerprint.Current.AuthenticateAsync("Prove you have fingers!", _cancel.Token);

            if (result.Authenticated)
            {
                await Navigation.PushAsync(new SecretView());
            }
            else
            {
                lblStatus.Text = $"{result.Status}: {result.ErrorMessage}";
            }
        }
    }
}
