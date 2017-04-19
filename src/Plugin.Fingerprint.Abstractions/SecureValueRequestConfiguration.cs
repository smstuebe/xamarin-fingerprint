using System;

namespace Plugin.Fingerprint.Abstractions
{
    /// <summary>
    /// Configuration of the stuff presented to the user.
    /// </summary>
    public class SecureValueRequestConfiguration : AuthenticationRequestConfiguration
    {
        public SecureValueRequestConfiguration(string key, string serviceId, string reason)
            : base(reason)
        {            
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            if (serviceId == null)
                throw new ArgumentNullException(nameof(serviceId));           

            ServiceId = serviceId;
            Key = key;
        }
      
        public string ServiceId { get; }

        public string Key { get; }            
    }
}