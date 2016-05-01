using MvvmCross.Platform;
using MvvmCross.Platform.Plugins;
using Plugin.Fingerprint;

namespace MvvmCross.Plugins.Fingerprint.iOS
{
    public class Plugin
        : IMvxPlugin
    {
        public void Load()
        {
            Mvx.LazyConstructAndRegisterSingleton(() => CrossFingerprint.Current);
        }
    }
}