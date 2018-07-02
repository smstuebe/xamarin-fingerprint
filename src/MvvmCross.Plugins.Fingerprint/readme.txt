---------------------------------
Xamarin and MvvmCross plugin for accessing the fingerprint sensor.
---------------------------------

var fpService = Mvx.Resolve<IFingerprint>(); // or use dependency injection and inject IFingerprint

var result = await fpService.AuthenticateAsync("Prove you have mvx fingers!");
if (result.Authenticated)
{
    // do secret stuff :)
}
else
{
    // not allowed to do secret stuff :(
}

Star on Github at: https://github.com/smstuebe/xamarin-fingerprint