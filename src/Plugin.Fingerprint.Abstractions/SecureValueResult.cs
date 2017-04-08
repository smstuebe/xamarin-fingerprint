namespace Plugin.Fingerprint.Abstractions
{
    public class SecureValueResult
    {      
        /// <summary>
        /// Status of the operation.
        /// </summary>
        public SecureValueResultStatus Status { get; set; }

        /// <summary>
        /// Reason for the unsuccessful operation.
        /// </summary>
        public string ErrorMessage { get; set; }
    }
}