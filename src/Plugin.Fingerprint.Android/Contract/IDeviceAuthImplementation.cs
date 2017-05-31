using System.Threading.Tasks;
using Plugin.Fingerprint.Abstractions;

namespace Plugin.Fingerprint.Contract
{
	public interface IDeviceAuthImplementation
	{
		bool IsDeviceAuthSetup();
		Task<FingerprintAuthenticationResult> AuthenticateAsync();
	}
}
