// include Fake lib
#r "../packages/FAKE/tools/FakeLib.dll"
open Fake
open System.IO;
open Fake.XUnit2Helper

RestorePackages()

// Properties
let artifactsDir = @".\artifacts\"
let artifactsBuildDir = "./artifacts/build/"
let artifactsTestsDir  = "./artifacts/tests/"

let artifactsNuGetPkgWorkingDir  = @".\artifacts\nuget-pkg-workingdir\"
let artifactsNuGetPkgDir  = @".\artifacts\nuget-pkg\"

// Targets
Target "Clean" (fun _ ->
    trace "Cleanup..."
    CleanDirs [artifactsDir]
)

Target "BuildApp" (fun _ ->
   trace "Building App..."
   !! "src/**/*.csproj"
     |> MSBuildRelease artifactsBuildDir "Build"
     |> Log "AppBuild-Output: "
)

Target "BuildTests" (fun _ ->
    trace "Building Tests..."
    !! "tests/**/*.csproj"
      |> Seq.collect (fun p -> MSBuildDebug (artifactsTestsDir @@ (Path.GetFileName(p))) "Build" [p])
      |> Log "TestBuild-Output: "
)

Target "RunTests" (fun _ ->
    trace "Running Tests..."
    !! (artifactsTestsDir + @"**\*Tests.dll") 
      |> xUnit2 (fun p -> {p with OutputDir = artifactsTestsDir })
)

Target "CreateNuGetArtifactsDirs" (fun _ ->
    CleanDir artifactsNuGetPkgDir
    CreateDir artifactsNuGetPkgDir
)

Target "Package:Sloader.Results" (fun _ ->

    CleanDir artifactsNuGetPkgWorkingDir

    trace "Create Assembly for NuGet Packages..."
    !! "src/Sloader.Results/*.csproj"
     |> MSBuildRelease artifactsNuGetPkgWorkingDir "Build"
     |> Log "NuGet Assembly Build-Output: "

    trace "Create Crawler Result NuGet Packages..."
    NuGet (fun p -> 
    {p with
        OutputPath = artifactsNuGetPkgDir
        WorkingDir = artifactsNuGetPkgWorkingDir
        })  "./src/Sloader.Results/Sloader.Results.nuspec"
)

Target "Package:Sloader.Crawler" (fun _ ->

    CleanDir artifactsNuGetPkgWorkingDir

    trace "Create Assembly for NuGet Packages..."
    !! "src/Sloader.Crawler/*.csproj"
     |> MSBuildRelease artifactsNuGetPkgWorkingDir "Build"
     |> Log "NuGet Assembly Build-Output: "

    trace "Create Crawler NuGet Packages..."
    NuGet (fun p -> 
    {p with
        OutputPath = artifactsNuGetPkgDir
        WorkingDir = artifactsNuGetPkgWorkingDir
        })  "./src/Sloader.Crawler/Sloader.Crawler.nuspec"
)

Target "Package:Sloader.Crawler.Config" (fun _ ->

    CleanDir artifactsNuGetPkgWorkingDir

    trace "Create Assembly for NuGet Packages..."
    !! "src/Sloader.Crawler.Config/*.csproj"
     |> MSBuildRelease artifactsNuGetPkgWorkingDir "Build"
     |> Log "NuGet Assembly Build-Output: "

    trace "Create Crawler Config NuGet Packages..."
    NuGet (fun p -> 
    {p with
        OutputPath = artifactsNuGetPkgDir
        WorkingDir = artifactsNuGetPkgWorkingDir
        })  "./src/Sloader.Crawler.Config/Sloader.Crawler.Config.nuspec"
)

Target "Default" (fun _ ->
    trace "Default Target invoked."
)

// Dependencies
"Clean"
  ==> "BuildApp"
  ==> "BuildTests"
  ==> "RunTests"
  ==> "CreateNuGetArtifactsDirs"
    ==> "Package:Sloader.Results"
    ==> "Package:Sloader.Crawler.Config"
    ==> "Package:Sloader.Crawler"
      ==> "Default"

// start build
RunTargetOrDefault "Default"
