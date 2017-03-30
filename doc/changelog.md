## Changelog


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
