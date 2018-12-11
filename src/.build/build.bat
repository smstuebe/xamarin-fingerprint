@echo off
"../.nuget/nuget.exe" install FAKE -Version 5.8.4

"FAKE.5.8.4/tools/FAKE.exe" build.fsx %*