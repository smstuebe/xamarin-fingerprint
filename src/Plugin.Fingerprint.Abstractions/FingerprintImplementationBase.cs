using System.Threading;
using System.Threading.Tasks;

namespace Plugin.Fingerprint.Abstractions
{
    public abstract class FingerprintImplementationBase : IFingerprint
    {
        public abstract bool IsAvailable { get; }

        public Task<FingerprintAuthenticationResult> AuthenticateAsync(string reason, CancellationToken cancellationToken = default(CancellationToken))
        {
            return AuthenticateAsync(new DialogConfiguration(reason), cancellationToken);
        }

        public abstract Task<FingerprintAuthenticationResult> AuthenticateAsync(DialogConfiguration dialogConfig, CancellationToken cancellationToken = new CancellationToken());
    }
}