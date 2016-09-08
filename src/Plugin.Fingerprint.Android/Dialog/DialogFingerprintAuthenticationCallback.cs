using System;
using Android.Runtime;

namespace Plugin.Fingerprint.Dialog
{
    public class DialogFingerprintAuthenticationCallback : FingerprintAuthenticationCallback
    {
        private readonly IDialogAuthenticationListener _listener;

        public DialogFingerprintAuthenticationCallback(IDialogAuthenticationListener listener)
        {
            _listener = listener;
        }

		public DialogFingerprintAuthenticationCallback(IntPtr a, JniHandleOwnership b) : base(a, b) { }

        public override void OnAuthenticationFailed()
        {
            base.OnAuthenticationFailed();
            _listener?.OnFailedTry();
        }
    }
}