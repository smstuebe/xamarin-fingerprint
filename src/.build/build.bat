@echo off
"../.nuget/nuget.exe" install FAKE -Version 4.61.2

"FAKE.4.61.2/tools/FAKE.exe" build.fsx %*