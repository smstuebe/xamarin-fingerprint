﻿using System;
using System.Threading;
using System.Threading.Tasks;
using Foundation;
using LocalAuthentication;
using ObjCRuntime;
using Plugin.Fingerprint.Abstractions;
using UIKit;

namespace Plugin.Fingerprint
{
    internal class FingerprintImplementation : IFingerprint
    {
        private NSError _error;
        private LAContext _context;

        public bool IsAvailable
        {
            get
            {
				if (_context == null)
				{
					return false;
				}
				
                return _context.CanEvaluatePolicy(LAPolicy.DeviceOwnerAuthenticationWithBiometrics, out _error);
            }
        }

        public FingerprintImplementation()
        {
			CreateLaContext();
        }

        public Task<FingerprintAuthenticationResult> AuthenticateAsync(string reason)
        {
            return AuthenticateAsync(reason, new CancellationToken());
        }

        public async Task<FingerprintAuthenticationResult> AuthenticateAsync(string reason, CancellationToken cancellationToken)
        {
            var result = new FingerprintAuthenticationResult();
            cancellationToken.Register(CancelAuthentication);
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

            CreateNewContext();
            return result;
        }

        private void CancelAuthentication()
        {
            CreateNewContext();
        }

        private void CreateNewContext()
        {
            if (_context != null)
            {
                if (_context.RespondsToSelector(new Selector("invalidate")))
                {
                    _context.Invalidate();
                }
                _context.Dispose();
            }

			CreateLaContext();
        }

		private void CreateLaContext()
		{
			if (!UIDevice.CurrentDevice.CheckSystemVersion(8, 0))
			{
				// LAContext is not available before ios 8.
				return;
			}

			_context = new LAContext();
		}
    }
}
