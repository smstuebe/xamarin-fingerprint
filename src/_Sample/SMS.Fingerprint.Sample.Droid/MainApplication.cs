using System;
using Android.App;
using Android.Runtime;
using Plugin.CurrentActivity;
using Plugin.Fingerprint;

namespace SMS.Fingerprint.Sample.Droid
{

#if DEBUG
    [Application(Debuggable = true)]
#else
	[Application(Debuggable = false)]
#endif
    public class MainApplication : Application
    {
        public MainApplication(IntPtr handle, JniHandleOwnership transer)
            : base(handle, transer)
        {
        }

        public override void OnCreate()
        {
            base.OnCreate();
            CrossCurrentActivity.Current.Init(this);
            CrossFingerprint.SetCurrentActivityResolver(() => CrossCurrentActivity.Current.Activity);

            // uncomment this line to use custom dialog
            //CrossFingerprint.SetDialogFragmentType<MyCustomDialogFragment>();
        }
    }
}