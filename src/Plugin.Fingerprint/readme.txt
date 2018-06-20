---------------------------------
Xamarin and MvvmCross plugin for accessing the fingerprint sensor.
---------------------------------

var result = await CrossFingerprint.Current.AuthenticateAsync("Prove you have fingers!");
if (result.Authenticated)
{
    // do secret stuff :)
}
else
{
    // not allowed to do secret stuff :(
}

Star on Github at: https://github.com/smstuebe/xamarin-fingerprint