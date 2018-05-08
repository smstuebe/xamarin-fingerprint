using System;
using System.Threading.Tasks;
using Plugin.Fingerprint.Abstractions;

namespace Plugin.Fingerprint.Contract
{
    public interface IAuthenticationCallback : IDisposable
    {
        Task<FingerprintAuthenticationResult> GetTask();
    }
}