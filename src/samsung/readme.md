== Build ==
- copy android.jar from a android sdk platform into sdk folder
- open in IntelliJ
- Build -> Build Artifacts

== Update ==
- Download pass sdk from samsung (http://developer.samsung.com/galaxy/pass)
- Decompile pass-xxx.jar with jd-gui (http://jd.benow.ca/)
- Save sources
- Move sources to src
- delete everything private and functions implementations
- build
- copy out/pass-fake.jar to binding project Jars
- update reference to pass-xxx.jar in binding project