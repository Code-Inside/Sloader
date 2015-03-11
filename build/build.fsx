// include Fake lib
#r "../packages/FAKE/tools/FakeLib.dll"
open Fake
open System.IO;

RestorePackages()

// Properties
let artifactsDir = @".\artifacts\"
let artifactsBuildDir = "./artifacts/build/"
let artifactsTestsDir  = "./artifacts/tests/"

let toolNugetExe = @".nuget\nuget.exe"

let artifactsResultsNugetSrcDir  = @".\artifacts\nuget-results-src\"
let artifactsPkgDir  = @".\artifacts\nuget-pkg\"

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
      |> xUnit (fun p -> {p with OutputDir = artifactsTestsDir })
)

// Poor mans NuGet Pack Solution for FAKE...
Target "CreateNuGetCrawlerResultPackages" (fun _ ->

    trace "Create Assembly for NuGet Packages..."
    // The Path stuff is sooo wrong, but I have no idea how to do this elegant
    !! "src/Sloader.Results/*.csproj"
     |> MSBuildRelease artifactsResultsNugetSrcDir "Build"
     |> Log "NuGet Assembly Build-Output: "

    trace "Create Crawler Result NuGet Packages..."
    CreateDir artifactsPkgDir
    let result =
        ExecProcess (fun info -> 
            info.FileName <- toolNugetExe
            info.Arguments <- "pack .\src\Sloader.Results\Sloader.Results.nuspec -OutputDirectory " + artifactsPkgDir + " -BasePath " + artifactsResultsNugetSrcDir
        ) (System.TimeSpan.FromMinutes 1.)
 
    if result <> 0 then failwith "Failed result from NuGet"
)

// Poor mans NuGet Pack Solution for FAKE... (COPY :-/ Sad Panda)
Target "CreateNuGetCrawlerPackages" (fun _ ->

    trace "Create Assembly for NuGet Packages..."
    // The Path stuff is sooo wrong, but I have no idea how to do this elegant
    !! "src/Sloader.Crawler/*.csproj"
     |> MSBuildRelease artifactsResultsNugetSrcDir "Build"
     |> Log "NuGet Assembly Build-Output: "

    trace "Create Crawler NuGet Packages..."
    CreateDir artifactsPkgDir
    let result =
        ExecProcess (fun info -> 
            info.FileName <- toolNugetExe
            info.Arguments <- "pack .\src\Sloader.Crawler\Sloader.Crawler.nuspec -OutputDirectory " + artifactsPkgDir + " -BasePath " + artifactsResultsNugetSrcDir
        ) (System.TimeSpan.FromMinutes 1.)
 
    if result <> 0 then failwith "Failed result from NuGet"
)

Target "Default" (fun _ ->
    trace "Default Target invoked."
)

// Dependencies
"Clean"
  ==> "BuildApp"
  ==> "BuildTests"
  ==> "RunTests"
  ==> "CreateNuGetCrawlerResultPackages"
  ==> "CreateNuGetCrawlerPackages"
  ==> "Default"

// start build
RunTargetOrDefault "Default"
