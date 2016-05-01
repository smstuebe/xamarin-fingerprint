using System;
using System.Threading;
using System.Threading.Tasks;
using Windows.Security.Credentials.UI;
using Plugin.Fingerprint.Abstractions;

namespace Plugin.Fingerprint
{
    internal class FingerprintImplementation : IFingerprint
    {
        public bool IsAvailable => CheckAvailability();

        public Task<FingerprintAuthenticationResult> AuthenticateAsync(string reason)
        {
            return AuthenticateAsync(reason, new CancellationToken());
        }

        public async Task<FingerprintAuthenticationResult> AuthenticateAsync(string reason, CancellationToken cancellationToken)
        {
            var result = new FingerprintAuthenticationResult();

            try
            {
                var verificationResult = await UserConsentVerifier.RequestVerificationAsync(reason);

                switch (verificationResult)
                {
                    case UserConsentVerificationResult.Verified:
                        result.Status = FingerprintAuthenticationResultStatus.Succeeded;
                        break;

                    case UserConsentVerificationResult.DeviceBusy:
                    case UserConsentVerificationResult.DeviceNotPresent:
                    case UserConsentVerificationResult.DisabledByPolicy:
                    case UserConsentVerificationResult.NotConfiguredForUser:
                        result.Status = FingerprintAuthenticationResultStatus.NotAvailable;
                        break;
                    
                    case UserConsentVerificationResult.RetriesExhausted:
                        result.Status = FingerprintAuthenticationResultStatus.Failed;
                        break;
                    case UserConsentVerificationResult.Canceled:
                        result.Status = FingerprintAuthenticationResultStatus.Canceled;
                        break;
                    default:
                        result.Status = FingerprintAuthenticationResultStatus.NotAvailable;
                        break;
                }
            }
            catch (Exception ex)
            {
                result.Status = FingerprintAuthenticationResultStatus.UnknownError;
                result.ErrorMessage = ex.Message;
            }

            return result;
        }

        private bool CheckAvailability()
        {
            var availability = UserConsentVerifier.CheckAvailabilityAsync().AsTask().Result;
            return availability == UserConsentVerifierAvailability.Available;
        }
    }
}
