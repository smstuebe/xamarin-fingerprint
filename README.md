# Xamarin Fingerprint
Xamarin and MvvMCross plugin for accessing the fingerprint sensor

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
 * `telnet 127.0.0.1` *emulator-id* (`adb devices` prints "emulator-*emulator-id*")
 * `finger touch 1`
 * `finger touch 1`

**Note for Windows users:**
You have to enable telnet: Programs and Features > Add Windows Feature > Telnet Client


