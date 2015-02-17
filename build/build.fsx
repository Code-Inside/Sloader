// include Fake lib
#r "../packages/FAKE/tools/FakeLib.dll"
open Fake

RestorePackages()

// Properties
let artifactsDir = @".\artifacts\"
let artifactsBuildDir = "./artifacts/build/"
let artifactsTestsDir  = "./artifacts/tests/"

let toolNugetExe = @".nuget\nuget.exe"

let resultsNuGetPkg = @".\src\Sloader.Results\Sloader.Results.nuspec"
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
      |> MSBuildDebug artifactsTestsDir "Build"
      |> Log "TestBuild-Output: "
)

Target "RunTests" (fun _ ->
    trace "Running Tests..."
    !! (artifactsTestsDir + @"\*Tests.dll") 
      |> xUnit (fun p -> {p with OutputDir = artifactsTestsDir })
)

// Poor mans NuGet Pack Solution for FAKE...
Target "CreateNuGetPackages" (fun _ ->

    trace "Create Assembly for NuGet Packages..."
    // The Path stuff is sooo wrong, but I have no idea how to do this elegant
    !! "src/Sloader.Results/*.csproj"
     |> MSBuildRelease artifactsResultsNugetSrcDir "Build"
     |> Log "NuGet Assembly Build-Output: "

    trace "Create NuGet Packages..."
    CreateDir artifactsPkgDir
    let result =
        ExecProcess (fun info -> 
            info.FileName <- toolNugetExe
            info.Arguments <- "pack " + resultsNuGetPkg + " -OutputDirectory " + artifactsPkgDir + " -BasePath " + artifactsResultsNugetSrcDir
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
  ==> "CreateNuGetPackages"
  ==> "Default"

// start build
RunTargetOrDefault "Default"