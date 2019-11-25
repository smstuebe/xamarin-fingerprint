using MvvmCross.Plugin;
using Plugin.Fingerprint;

namespace MvvmCross.Plugins.Fingerprint
{
    [MvxPlugin]
    [Preserve(AllMembers = true)]
    public class Plugin : IMvxPlugin
    {
        public void Load()
        {
            Mvx.LazyConstructAndRegisterSingleton(() => CrossFingerprint.Current);
        }
    }
}