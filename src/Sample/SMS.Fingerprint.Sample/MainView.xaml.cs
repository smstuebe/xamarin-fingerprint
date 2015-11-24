using System;
using Xamarin.Forms;

namespace SMS.Fingerprint.Sample
{
    public partial class MainView : ContentPage
    {
        public MainView()
        {
            InitializeComponent();
        }

        private async void OnAuthorize(object sender, EventArgs e)
        {
            var authenticated = await Fingerprint.Current.AuthenticateAsync("Prove that you have fingers!");

            if (authenticated.Authorized)
            {
                await Navigation.PushAsync(new SecretView());
            }
        }
    }
}
