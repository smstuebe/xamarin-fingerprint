// don't forget nuget setapikey <key> before publish ;)
#r @"FAKE.4.8.0/tools/FakeLib.dll"
#r "System.Xml.Linq"

open System.Xml.Linq
open System.Xml;
open System.IO
open System.Text.RegularExpressions
open Fake
open Fake.XMLHelper;
open Fake.Git;


let (+/) path1 path2 = Path.Combine(path1, path2)
let RepositoryRootDir = "../../"
let NuGetTargetDir = @"./out/nuget/"
let BuildTargetDir = @"./out/lib/"
let BootstrapFile = "FingerprintPluginBootstrap.cs.pp"
let NugetPath = "../.nuget/NuGet.exe"
let NuspecFiles = ["Plugin.Fingerprint.nuspec"; "MvvmCross.Plugins.Fingerprint.nuspec"] 

let Build (projectName:string, targetSubDir:string) =
    [".." +/ projectName +/ projectName + ".csproj"]
     |> MSBuildRelease (BuildTargetDir +/ targetSubDir) "Build"
     |> Log "Output: "

let NuVersionGet (specFile:string) =
    let doc = System.Xml.Linq.XDocument.Load(specFile)
    let versionElements = doc.Descendants(XName.Get("version", doc.Root.Name.NamespaceName))
    (Seq.head versionElements).Value

let NuVersionSet (specFile:string, version:string) = 
    let xmlDocument = new XmlDocument()
    xmlDocument.Load specFile
    let nsmgr = XmlNamespaceManager(xmlDocument.NameTable)
    nsmgr.AddNamespace("ns", "http://schemas.microsoft.com/packaging/2010/07/nuspec.xsd")
    let node = xmlDocument.DocumentElement.SelectSingleNode("//ns:version", nsmgr)
    node.InnerText <- version
    xmlDocument.Save specFile

let NuPack (specFile:string, publish:bool) = 
    let version = NuVersionGet(specFile)
    let project = specFile.Replace(".nuspec", "")

    NuGet (fun p -> 
    {p with
        ToolPath = NugetPath
        Version = version
        OutputPath = NuGetTargetDir
        WorkingDir = BuildTargetDir
        Project = project
        Publish = publish
        PublishUrl = "https://www.nuget.org/api/v2/package"
        }) specFile

let NuPackAll (publish:bool) = 
    NuspecFiles |> List.iter (fun file -> NuPack(file, publish))

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
    Build("Plugin.Fingerprint", "pcl")
    Build("Plugin.Fingerprint.NotImplemented", "null")
    Build("Plugin.Fingerprint.Android", "android")
    Build("Plugin.Fingerprint.iOS", "ios")
    Build("Plugin.Fingerprint.Mac", "mac")
    Build("Plugin.Fingerprint.UWP", "uwp")
    Build("MvvmCross.Plugins.Fingerprint", "mvx" +/ "pcl")
    Build("MvvmCross.Plugins.Fingerprint.Android", "mvx" +/ "android")
    Build("MvvmCross.Plugins.Fingerprint.iOS", "mvx" +/ "ios")
    Build("MvvmCross.Plugins.Fingerprint.WindowsUWP", "mvx" +/ "uwp")
    
    trace "copy mvvm cross bootstrap files..."
    File.Copy("../MvvmCross.Plugins.Fingerprint.Android" +/ BootstrapFile, BuildTargetDir +/ "mvx" +/ "android" +/ BootstrapFile)
    File.Copy("../MvvmCross.Plugins.Fingerprint.iOS" +/ BootstrapFile, BuildTargetDir +/ "mvx" +/ "ios" +/ BootstrapFile)
    File.Copy("../MvvmCross.Plugins.Fingerprint.WindowsUWP" +/ BootstrapFile, BuildTargetDir +/ "mvx" +/ "uwp" +/ BootstrapFile)
)

Target "nupack" (fun _ ->
    NuPackAll false
)

//call: build version v=1.0.0
Target "version" (fun _ ->
    let version = getBuildParam "v"
    let cleanVersion = Regex.Replace(version, @"[^\d\.].*$", "")

    BulkReplaceAssemblyInfoVersions ".." (fun info ->
    {info with
        AssemblyVersion = cleanVersion  
        AssemblyFileVersion = cleanVersion
        AssemblyInformationalVersion = version
    })

    List.iter (fun f -> NuVersionSet(f, version)) NuspecFiles
)

Target "publish" (fun _ ->    
    if not (Fake.Git.Information.isCleanWorkingCopy RepositoryRootDir) then
        failwith "Uncommited changes. Please commit everything!"
    
    NuPackAll true

    let version = NuVersionGet(Seq.head NuspecFiles)    
    let branchName = Fake.Git.Information.getBranchName RepositoryRootDir
    trace ("Current GIT Branch: " + branchName)
    
    let tagName = ("v" + version)
    trace ("Creating Tag: " + tagName)
    tag RepositoryRootDir tagName
    pushTag RepositoryRootDir  "origin" tagName
)

// Dependencies
"clean"
  ==> "build"
  ==> "nupack"

"build"
  ==> "publish"

// start build
RunTargetOrDefault "build"