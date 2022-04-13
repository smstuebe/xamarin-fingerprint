## Changelog

### 3.0.0

- Support for MAUI

### 2.1.5

- #212 Android authentication result is now `Cancelled` instead of `Failed` if the user presses a button to dismiss the dialog.

### 2.1.4

- #203, #205 fixed mLifecycleObserver related crash on some devices (thx @Seuleuzeuh)
- updated Xamarin.AndroidX.Biometric to 1.1.0

### 2.1.3

- #200 fixed hang on second consecutive fallback authentication (thx @albertruff)
- updated Xamarin.AndroidX.Biometric to 1.0.1.7

### 2.1.2

- sourcelink support
- #192 Added ConfirmationRequired option to AuthenticationRequestConfiguration (Android only)
- updated Xamarin.AndroidX.Biometric to 1.0.1.6

### 2.1.1

- new enum member FingerprintAvailability.NoFallback for CheckAvailabilityAsync()
- Android: CheckAvailabilityAsync(true) returns Available if Biometric is not available, but fallback is available.
- iOS: CheckAvailabilityAsync(true) FingerprintAvailability.NoFallback instead of FingerprintAvailability.NoFingerprint if no passcode is set.
- IsAvailableAsync changed accordingly

### 2.0.0

- .netstandard 2.0
- Android X
- replaced custom dialog and Samsung pass with BiometricPrompt
- Fallback authentication for Android
- NoDialog option nolonger supported

### 1.4.9

- #116 Use Android API instead of pass SDK on Samsung devices with Android 9+. (thanks @Nedlinin) 

### 1.4.7

- #101 FingerprintAuthenticationResultStatus and FingerprintAvailability now can express that the user has denied the access to the authentication method

### 1.4.6

- #51 allows setting the `DefaultColor` to change the color of the fingerprint icon
- #86 android fallback button is now hidden if AllowAlternativeAuthentication is false (thanks @fedemkr)
- #91 new API `GetAuthenticationTypeAsync` to retrieve the biometric auth type
- CrossFingerprint is now setable for mocking during unit tests (thanks @ArtjomP)

#### 1.4.6-beta4

- #84 missing resource ids should be available again

#### 1.4.6-beta3

- #83 prevent NullRefereneceException and catch others if service is not available on Android

#### 1.4.6-beta2

- #75 fix crash at onPause on Samsung devices
- #73 display help string for recoverable errors

#### 1.4.6-beta1

- #29 fix crash on device lock during authentication

### 1.4.5

- #53, #70 fixed possible crash on some samsung devices without fingerprint sensor

### 1.4.4

- #60 allow alternative authentication via PIN / password (iOS only)
- allow custon fallback title on Mac

### 1.4.3

- #45: fixed UWP nuget packages

### 1.4.2

- #43: fixed crash when fast tapping on cancel or fallback on Android

### 1.4.1

- #40: fixed crash on iOS &lt; 8.0

### 1.4.0

- Xamarin.Mac support

### 1.3.0

- #28 fixed wrong authentication status on too many attempts

#### 1.3.0-beta5

- Breaking: IsAvaialable is now `IsAvailableAsync()`. `GetAvailabilityAsync()` can be used for a more detailed handling.
- #15 Support for .NET standard
- #21 Switched to fail early approach if PCL is referenced
- #14 Introduced new authentication result for too many attempts

#### 1.3.0-beta4

- Android: handling of backbutton, disabled touch to cancel

#### 1.3.0-beta3

- #18 fixed crash on iOS < 7
- #2 support for Samsung devices
- animations on the Android dialog

#### 1.3.0-beta2

- #13 Fallback button on Android
- #12 possibility to set localized fallback title
- #6 fixed crash on Android

#### 1.3.0-beta1

- #5 Implemented feedback for failed authentication try
