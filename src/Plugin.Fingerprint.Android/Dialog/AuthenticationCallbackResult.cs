using Plugin.Fingerprint.Abstractions;

namespace Plugin.Fingerprint.Dialog
{
    public class AuthenticationCallbackResult
    {
        public byte[] IV { get; set; }
        public byte[] Result { get; set; }

        public FingerprintAuthenticationResultStatus Status { get; set; }

        public bool Animate { get; set; } = true;
    }
}