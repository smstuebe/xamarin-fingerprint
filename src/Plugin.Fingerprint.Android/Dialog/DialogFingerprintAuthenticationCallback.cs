namespace Plugin.Fingerprint.Dialog
{
    public class DialogFingerprintAuthenticationCallback : FingerprintAuthenticationCallback
    {
        private readonly IDialogAuthenticationListener _listener;

        public DialogFingerprintAuthenticationCallback(IDialogAuthenticationListener listener)
        {
            _listener = listener;
        }

        public override void OnAuthenticationFailed()
        {
            base.OnAuthenticationFailed();
            _listener?.OnFailedTry();
        }
    }
}