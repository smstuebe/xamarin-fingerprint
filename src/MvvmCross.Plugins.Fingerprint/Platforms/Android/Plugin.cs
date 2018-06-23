using MvvmCross.Platforms.Android;
using MvvmCross.Plugin;
using Plugin.Fingerprint;

namespace MvvmCross.Plugins.Fingerprint.Platforms.Android
{
    [MvxPlugin]
    [Preserve(AllMembers = true)]
    public class Plugin : IMvxPlugin
    {
        public void Load()
        {
            CrossFingerprint.SetCurrentActivityResolver(() => Mvx.Resolve<IMvxAndroidCurrentTopActivity>().Activity);
            Mvx.LazyConstructAndRegisterSingleton(() => CrossFingerprint.Current);
        }
    }   
}