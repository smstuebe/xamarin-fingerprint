namespace Plugin.Fingerprint.Abstractions
{
    public class GetSecureValueResult
    {           
        /// <summary>
        /// Status of the operation.
        /// </summary>
        public FingerprintAuthenticationResultStatus Status { get; set; }

        /// <summary>
        /// The value read from the OS secure store after successful fingerprint authentication.
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Reason for the unsuccessful operation.
        /// </summary>
        public string ErrorMessage { get; set; }
    }
}