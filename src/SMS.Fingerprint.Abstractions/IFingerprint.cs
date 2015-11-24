using System.Threading;
using System.Threading.Tasks;

namespace SMS.Fingerprint.Abstractions
{
    public interface IFingerprint
    {
        bool IsAvailable { get; }
        Task<FingerprintAuthenticationResult> AuthenticateAsync(string reason);
        Task<FingerprintAuthenticationResult> AuthenticateAsync(string reason, CancellationToken cancellationToken);
    }
}
