using Cirrious.CrossCore;
using Cirrious.CrossCore.Plugins;

namespace SMS.MvvmCross.Fingerprint.iOS
{
    public class Plugin
        : IMvxPlugin
    {
        public void Load()
        {
            Mvx.LazyConstructAndRegisterSingleton(() => SMS.Fingerprint.Fingerprint.Current);
        }
    }
}