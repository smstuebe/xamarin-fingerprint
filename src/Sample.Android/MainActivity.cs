using Android.App;
using Android.Content.PM;
using Android.OS;
using Plugin.Fingerprint;
using Plugin.Fingerprint.Platforms.Android.Utils;

namespace Sample.Droid
{
    [Activity(Label = "Sample", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            CrossFingerprint.SetCurrentActivityResolver(() => this);
            CrossFingerprint.CryptoSettings = new CryptoSettings(Application.Context.PackageName + "_biometric_authentication_key", new byte[] { 11, 15, 6, 8, 8, 242, 121, 21, 100, 4, 51, 83, 95, 45, 33, 122 });

            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            LoadApplication(new App());
        }
    }
}