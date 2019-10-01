---------------------------------
Xamarin and MvvmCross plugin for accessing the fingerprint sensor.
---------------------------------

var dialogConfig = new AuthenticationRequestConfiguration("My App", "Prove you have fingers!")
var result = await CrossFingerprint.Current.AuthenticateAsync(dialogConfig);
if (result.Authenticated)
{
    // do secret stuff :)
}
else
{
    // not allowed to do secret stuff :(
}

Star on Github at: https://github.com/smstuebe/xamarin-fingerprint