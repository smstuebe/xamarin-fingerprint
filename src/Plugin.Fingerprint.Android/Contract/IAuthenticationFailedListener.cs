namespace Plugin.Fingerprint.Contract
{
    public interface IAuthenticationFailedListener
    {
        void OnFailedTry();
    }
}