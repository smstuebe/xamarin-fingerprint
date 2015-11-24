namespace SMS.Fingerprint.Abstractions
{
    public class FingerprintAuthenticationResult
    {
        public bool Authenticated { get { return Status == FingerprintAuthenticationResultStatus.Succeeded; } }
        public FingerprintAuthenticationResultStatus Status { get; set; }
        public string ErrorMessage { get; set; }
    }
}