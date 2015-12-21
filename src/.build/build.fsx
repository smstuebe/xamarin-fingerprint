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
let NuspecFiles = ["SMS.Fingerprint.nuspec"; "SMS.Mvvmcross.Fingerprint.nuspec"] 

let Build (projectName:string, targetSubDir:string) =
    !! (".." +/ projectName +/ projectName + ".csproj")
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