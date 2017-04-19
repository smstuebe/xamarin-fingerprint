using System;

namespace Plugin.Fingerprint.Abstractions
{
    public class SetSecureValueRequestConfiguration : SecureValueRequestConfiguration
    {
        public SetSecureValueRequestConfiguration(string key, string value, string serviceId, string reason)
            : base(key, serviceId, reason)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            Value = value;
        }
      
        public string Value { get; }         
    }
}