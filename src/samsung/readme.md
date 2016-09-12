# Samsung Pass Java Fake
The original Samsung pass library is obFUCKuscated. The lib2xml tool can't generate the api.xml properly because of that. This project builds an empty version of the pass-xxx.jar that is used as input jar in the binding project just to generate the binding automatically.

## Build
- copy android.jar from a android sdk platform into sdk folder
- open in IntelliJ
- Build -> Build Artifacts

## Update 
- Download pass sdk from samsung (http://developer.samsung.com/galaxy/pass)
- Decompile pass-xxx.jar with jd-gui (http://jd.benow.ca/)
- Save sources
- Move sources to src
- delete everything private and functions implementations
- build
- copy out/pass-fake.jar to binding project Jars
- update reference to pass-xxx.jar in binding project