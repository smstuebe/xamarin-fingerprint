using MvvmCross.Platform;
using MvvmCross.Platform.Droid.Platform;
using MvvmCross.Platform.Plugins;
using Plugin.Fingerprint;

namespace MvvmCross.Plugins.Fingerprint.Android
{
    public class Plugin
        : IMvxPlugin
    {
        public void Load()
        {
            CrossFingerprint.SetCurrentActivityResolver(() => Mvx.Resolve<IMvxAndroidCurrentTopActivity>().Activity);
            Mvx.LazyConstructAndRegisterSingleton(() => CrossFingerprint.Current);
        }
    }   
}