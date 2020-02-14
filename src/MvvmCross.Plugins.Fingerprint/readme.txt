---------------------------------
Xamarin and MvvmCross plugin for accessing the fingerprint sensor.
---------------------------------

var fpService = Mvx.Resolve<IFingerprint>(); // or use dependency injection and inject IFingerprint

var dialogConfig = new AuthenticationRequestConfiguration("My App", "Prove you have fingers!")
var result = await fpService.AuthenticateAsync(dialogConfig);
if (result.Authenticated)
{
    // do secret stuff :)
}
else
{
    // not allowed to do secret stuff :(
}

How to setup:
Read carefully (especially the Android X part)! https://github.com/smstuebe/xamarin-fingerprint

Star on Github at: https://github.com/smstuebe/xamarin-fingerprint

Feel free to buy me a bavarian beer: https://www.paypal.me/smstuebe