using MvvmCross.Plugin;
using Plugin.Fingerprint;

namespace MvvmCross.Plugins.Fingerprint.WindowsUWP
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