#r @"FAKE.4.8.0/tools/FakeLib.dll"
#r "System.Xml.Linq"
open Fake
open System.Xml.Linq
open System.IO

RestorePackages()

let (+/) path1 path2 = Path.Combine(path1, path2)
let NuGetTargetDir = @"./out/nuget/"
let BuildTargetDir = @"./out/lib/"

Target "clean" (fun _ ->
    trace "cleaning up..."
    CleanDir NuGetTargetDir
    CleanDir BuildTargetDir
)

Target "build" (fun _ ->
   trace "building libraries..."
   !! "../SMS.Fingerprint/*.csproj"
     |> MSBuildRelease (BuildTargetDir +/ "pcl") "Build"
     |> Log "Output: "

   !! "../SMS.Fingerprint.Android/*.csproj"
     |> MSBuildRelease (BuildTargetDir +/ "android") "Build"
     |> Log "Output: "

   !! "../SMS.Fingerprint.iOS/*.csproj"
     |> MSBuildRelease (BuildTargetDir +/ "ios") "Build"
     |> Log "Output: "
)

Target "nupack" (fun _ ->
   
    let doc = System.Xml.Linq.XDocument.Load("SMS.Fingerprint.nuspec")
    let vers = doc.Descendants(XName.Get("version", doc.Root.Name.NamespaceName)) 

    NuGet (fun p -> 
    {p with
        ToolPath = "../.nuget/NuGet.exe"
        Version = (Seq.head vers).Value
        OutputPath = NuGetTargetDir
        WorkingDir = BuildTargetDir
        }) "SMS.Fingerprint.nuspec"
)

// Dependencies
"clean"
  ==> "build"
  ==> "nupack"

// start build
RunTargetOrDefault "build"