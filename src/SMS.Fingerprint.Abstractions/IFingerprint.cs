using System.Threading;
using System.Threading.Tasks;

namespace SMS.Fingerprint.Abstractions
{
    public enum FingerprintAuthenticationResultStatus
    {
        Unknown,
        Succeeded,
        FallbackRequested,
        Failed,
        Canceled,
        UnknownError,
        NotAvailable
    }

    public class FingerprintAuthenticationResult
    {
        public bool Authorized { get { return Status == FingerprintAuthenticationResultStatus.Succeeded; } }
        public FingerprintAuthenticationResultStatus Status { get; set; }
        public string ErrorMessage { get; set; }
    }

    public interface IFingerprint
    {
        bool IsAvailable { get; }
        Task<FingerprintAuthenticationResult> AuthenticateAsync(string reason);
        Task<FingerprintAuthenticationResult> AuthenticateAsync(string reason, CancellationToken cancellationToken);
    }
}
