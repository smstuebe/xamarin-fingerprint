namespace Plugin.Fingerprint.Abstractions
{
    public enum FingerprintAuthenticationResultStatus
    {
        Unknown,
        Succeeded,
        FallbackRequested,
        Failed,
        Cancelled,
        TooManyAttempts,
        UnknownError,
        NotAvailable
    }
}