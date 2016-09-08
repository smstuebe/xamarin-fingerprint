namespace Plugin.Fingerprint.Abstractions
{
    public class DialogConfiguration
    {
        public string Reason { get; }
        public string CancelTitle { get; set; }
        public string FallbackTitle { get; set; }

        public DialogConfiguration(string reason)
        {
            Reason = reason;
        }
    }
}