#r @"FAKE.4.8.0/tools/FakeLib.dll"
#r "System.Xml.Linq"
open Fake
open System.Xml.Linq
open System.IO

let (+/) path1 path2 = Path.Combine(path1, path2)
let NuGetTargetDir = @"./out/nuget/"
let BuildTargetDir = @"./out/lib/"
let BootstrapFile = "FingerprintPluginBootstrap.cs.pp"
let NugetPath = "../.nuget/NuGet.exe"

let Build (projectName:string, targetSubDir:string) =
    !! (".." +/ projectName +/ projectName + ".csproj")
     |> MSBuildRelease (BuildTargetDir +/ targetSubDir) "Build"
     |> Log "Output: "

let NuPack (specFile:string) = 
    let doc = System.Xml.Linq.XDocument.Load(specFile)
    let vers = doc.Descendants(XName.Get("version", doc.Root.Name.NamespaceName)) 

    NuGet (fun p -> 
    {p with
        ToolPath = NugetPath
        Version = (Seq.head vers).Value
        OutputPath = NuGetTargetDir
        WorkingDir = BuildTargetDir
        }) specFile

let RestorePackages() = 
    !! "../**/packages.config"
    |> Seq.iter (RestorePackage (fun p ->
        { p with
            ToolPath = NugetPath
            OutputPath = "../packages"
        }))

// Targets
Target "clean" (fun _ ->
    trace "cleaning up..."
    CleanDir NuGetTargetDir
    CleanDir BuildTargetDir
)

Target "build" (fun _ ->
    trace "restoring packages..."
    RestorePackages()

    trace "building libraries..."
    Build("SMS.Fingerprint", "pcl")
    Build("SMS.Fingerprint.Android", "android")
    Build("SMS.Fingerprint.iOS", "ios")
    Build("SMS.MvvmCross.Fingerprint.Android", "mvx" +/ "android")
    Build("SMS.MvvmCross.Fingerprint.iOS", "mvx" +/ "ios")
    
    trace "copy mvvm cross bootstrap files..."
    File.Copy("../SMS.MvvmCross.Fingerprint.Android" +/ BootstrapFile, BuildTargetDir +/ "mvx" +/ "android" +/ BootstrapFile)
    File.Copy("../SMS.MvvmCross.Fingerprint.iOS" +/ BootstrapFile, BuildTargetDir +/ "mvx" +/ "ios" +/ BootstrapFile)
)

Target "nupack" (fun _ ->
    NuPack "SMS.Fingerprint.nuspec"
    NuPack "SMS.Mvvmcross.Fingerprint.nuspec"
)

// Dependencies
"clean"
  ==> "build"
  ==> "nupack"

// start build
RunTargetOrDefault "build"