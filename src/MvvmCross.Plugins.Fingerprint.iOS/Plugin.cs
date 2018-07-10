using MvvmCross.Plugin;
using Plugin.Fingerprint;

namespace MvvmCross.Plugins.Fingerprint.iOS
{
    [MvxPlugin]
    public class Plugin
        : IMvxPlugin
    {
        public void Load()
        {
            Mvx.LazyConstructAndRegisterSingleton(() => CrossFingerprint.Current);
        }
    }
}