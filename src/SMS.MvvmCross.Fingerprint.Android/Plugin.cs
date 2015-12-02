using Cirrious.CrossCore;
using Cirrious.CrossCore.Droid.Platform;
using Cirrious.CrossCore.Plugins;

namespace SMS.MvvmCross.Fingerprint.Android
{
    public class Plugin
        : IMvxPlugin
    {
        public void Load()
        {
            SMS.Fingerprint.Fingerprint.SetCurrentActivityResolver(() => Mvx.Resolve<IMvxAndroidCurrentTopActivity>().Activity);
            Mvx.LazyConstructAndRegisterSingleton(() => SMS.Fingerprint.Fingerprint.Current);
        }
    }   
}