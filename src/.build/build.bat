@echo off
"../.nuget/nuget.exe" install FAKE -Version 4.48.0

"FAKE.4.48.0/tools/FAKE.exe" build.fsx %*