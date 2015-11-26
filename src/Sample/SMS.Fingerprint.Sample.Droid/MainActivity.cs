using Android.App;
using Android.Content.PM;
using Android.OS;

namespace SMS.Fingerprint.Sample.Droid
{
    [Activity(Label = "SMS.Fingerprint.Sample", Icon = "@drawable/xamarin_fingerprint", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : Xamarin.Forms.Platform.Android.FormsApplicationActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            Xamarin.Forms.Forms.Init(this, bundle);
            LoadApplication(new App());
        }
    }
}

