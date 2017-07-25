namespace Plugin.Fingerprint.Contract
{
    internal class DeafAuthenticationFailedListener : IAuthenticationFailedListener
    {
        public void OnFailedTry()
        {
        }

        public void OnHelp(FingerprintAuthenticationHelp help, string nativeHelpText)
        {
        }
    }
}