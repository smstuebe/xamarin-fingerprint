# <img src="doc/xamarin_fingerprint.png" width="80" height="71"/> Xamarin Fingerprint
Xamarin and MvvMCross plugin for accessing the fingerprint sensor

It is available via NuGet: TODO

## Usage
### API
The API is defined by the ```IFingerprint``` interface:

```csharp
/// <summary>
/// Checks the availability of fingerprint authentication.
/// Possible Reasons for unavailability:
/// - Device has no sensor
/// - OS does not support this functionality
/// - Fingerprint is not enrolled
/// </summary>
bool IsAvailable { get; }

/// <summary>
/// Requests the authentication.
/// </summary>
/// <param name="reason">Reason for the fingerprint authentication request. Displayed to the user.</param>
/// <returns>Authentication result</returns>
Task<FingerprintAuthenticationResult> AuthenticateAsync(string reason);

/// <summary>
/// Requests the authentication (cancelable).
/// </summary>
/// <see cref="AuthenticateAsync(string)"/>
Task<FingerprintAuthenticationResult> AuthenticateAsync(string reason, CancellationToken cancellationToken);
```

The returned ```FingerprintAuthenticationResult``` contains information about the authentication.
```csharp
/// <summary>
/// Indicatates whether the authentication was successful or not.
/// </summary>
public bool Authenticated { get { return Status == FingerprintAuthenticationResultStatus.Succeeded; } }

/// <summary>
/// Detailed information of the authentication.
/// </summary>
public FingerprintAuthenticationResultStatus Status { get; set; }

/// <summary>
/// Reason for the unsucessful authentication.
/// </summary>
public string ErrorMessage { get; set; }

```

### Example
#### vanilla
```csharp
var result = await Fingerprint.Current.AuthenticateAsync("Prove you have fingers!");
if (result.Authenticated)
{
    // do secret stuff :)
}
else
{
    // not allowed to do secret stuff :(
}
```

#### using MvvMCross
```csharp
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
```

### iOS
Nothing special on iOS. You can't configure anything and the standard iOS Dialog will be shown.

### Android
#### Setup
**Request the permission in AndroidManifest.xml**
```xml
<uses-permission android:name="android.permission.USE_FINGERPRINT" />
```
**Set the resolver of the current Activity**

Skip this, if you use the MvvMCross Plugin or don't use the dialog.

We need the current activity to display the dialog. You can use the [Current Activity Plugin](https://github.com/jamesmontemagno/Xamarin.Plugins/tree/master/CurrentActivity) from James Montemagno or implement your own functionality to retrieve the current activity. See Sample App for details.
```csharp
Fingerprint.SetCurrentActivityResolver(() => CrossCurrentActivity.Current.Activity);
```
#### Configuration
You can disable the dialog on Android (e.g. show fingerprint icon within your activity).
```csharp
Fingerprint.DialogEnabled = false;
```
## Testing on Simulators
### iOS
![Controlling the sensor on the iOS Simulator](doc/ios_simulator.png "Controlling the sensor on the iOS Simulator")

With the Hardware menu you can
* Toggle the enrollment status
* Trigger valid (<kbd>⌘</kbd> <kbd>⌥</kbd> <kbd>M</kbd>) and invalid (<kbd>⌘</kbd> <kbd>⌥</kbd> <kbd>N</kbd>) fingerprint sensor events

### Android
* start the emulator (Android >= 6.0)
* open the settings app
* go to Security > Fingerprint, then follow the enrollment instructions
* when it asks for touch
 * open command prompt
 * `telnet 127.0.0.1 <emulator-id>` (`adb devices` prints "emulator-&lt;emulator-id&gt;")
 * `finger touch 1`
 * `finger touch 1`

Sending fingerprint sensor events for testing the plugin can be done with the telnet commands, too.

**Note for Windows users:**
You have to enable telnet: Programs and Features > Add Windows Feature > Telnet Client

## Contribution
<img src="http://i.imgur.com/WFBeQuG.png" /> + <img src="http://i.imgur.com/P4Ay9tm.png" />
