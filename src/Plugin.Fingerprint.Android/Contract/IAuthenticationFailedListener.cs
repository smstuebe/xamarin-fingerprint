namespace Plugin.Fingerprint.Contract
{
    public interface IAuthenticationFailedListener
    {
        void OnFailedTry();
        void OnHelp(FingerprintAuthenticationHelp help, string nativeHelpText);
    }
}