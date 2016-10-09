@echo off
"../.nuget/nuget.exe" install FAKE -Version 4.41.3

"FAKE.4.41.3/tools/FAKE.exe" build.fsx %*