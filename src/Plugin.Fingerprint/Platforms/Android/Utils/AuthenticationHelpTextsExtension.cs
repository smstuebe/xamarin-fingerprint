using Plugin.Fingerprint.Abstractions;
using Plugin.Fingerprint.Contract;

namespace Plugin.Fingerprint.Utils
{
    public static class AuthenticationHelpTextsExtension
    {
        public static string GetText(this AuthenticationHelpTexts texts, FingerprintAuthenticationHelp help,
            string nativeText)
        {
            switch (help)
            {
                case FingerprintAuthenticationHelp.MovedTooFast when !string.IsNullOrEmpty(texts.MovedTooFast):
                    return texts.MovedTooFast;
                case FingerprintAuthenticationHelp.MovedTooSlow when !string.IsNullOrEmpty(texts.MovedTooSlow):
                    return texts.MovedTooSlow;
                case FingerprintAuthenticationHelp.Partial when !string.IsNullOrEmpty(texts.Partial):
                    return texts.Partial;
                case FingerprintAuthenticationHelp.Insufficient when !string.IsNullOrEmpty(texts.Insufficient):
                    return texts.Insufficient;
                case FingerprintAuthenticationHelp.Dirty when !string.IsNullOrEmpty(texts.Dirty):
                    return texts.Dirty;
            }

            return nativeText;
        }
    }
}