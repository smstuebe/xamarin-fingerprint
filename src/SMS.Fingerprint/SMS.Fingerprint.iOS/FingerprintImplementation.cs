using System.Threading;
using System.Threading.Tasks;
using Foundation;
using LocalAuthentication;

namespace SMS.Fingerprint
{
    internal class FingerprintImplementation : IFingerprint
    {
        private NSError _error;
        private readonly LAContext _context;

        public bool IsAvailable
        {
            get
            {
                return _context.CanEvaluatePolicy(LAPolicy.DeviceOwnerAuthenticationWithBiometrics, out _error);
            }
        }

        public FingerprintImplementation()
        {
            _context = new LAContext();
        }

        public Task<FingerprintAuthenticationResult> AuthenticateAsync(string reason)
        {
            return AuthenticateAsync(reason, new CancellationToken());
        }

        public async Task<FingerprintAuthenticationResult> AuthenticateAsync(string reason, CancellationToken cancellationToken)
        {
            var result = new FingerprintAuthenticationResult();

            if (!IsAvailable)
            {
                result.Status = FingerprintAuthenticationResultStatus.NotAvailable;
                return result;
            }

            var resTuple = await _context.EvaluatePolicyAsync(LAPolicy.DeviceOwnerAuthenticationWithBiometrics, reason);

            if (resTuple.Item1)
            {
                result.Status = FingerprintAuthenticationResultStatus.Succeeded;
            }
            else
            {
                switch ((LAStatus)(int)resTuple.Item2.Code)
                {
                    case LAStatus.AuthenticationFailed:
                        result.Status = FingerprintAuthenticationResultStatus.Failed;
                        break;

                    case LAStatus.UserCancel:
                        result.Status = FingerprintAuthenticationResultStatus.Canceled;
                        break;

                    case LAStatus.UserFallback:
                        result.Status = FingerprintAuthenticationResultStatus.FallbackRequested;
                        break;

                    default:
                        result.Status = FingerprintAuthenticationResultStatus.UnknownError;
                        break;
                }

                result.ErrorMessage = resTuple.Item2.LocalizedDescription;
            }

            return result;
        }
    }
}
