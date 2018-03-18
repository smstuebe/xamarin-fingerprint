using Android.App;
using Android.Widget;
using Android.OS;
using Android.Views;

namespace Plugin.Fingerprint.Test.UI.Android
{
    [Activity(Label = "Fingerprint Test", MainLauncher = true, Icon = "@drawable/xamarin_fingerprint")]
    public class MainActivity : Activity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.Main);
        }

        protected override async void OnResume()
        {
            base.OnResume();

            CrossFingerprint.SetCurrentActivityResolver(() => this);
            var availability = await CrossFingerprint.Current.GetAvailabilityAsync(false);
            var result = await CrossFingerprint.Current.AuthenticateAsync("Test");

            FindViewById<TextView>(Resource.Id.txtAvailability).Text = availability.ToString();
            FindViewById<TextView>(Resource.Id.txtResult).Text = result.Status.ToString();
            FindViewById<TextView>(Resource.Id.txtError).Text = result.ErrorMessage;
            FindViewById<TextView>(Resource.Id.txtSuccess).Visibility = ViewStates.Visible;
        }
    }
}