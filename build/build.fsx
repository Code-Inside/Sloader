// include Fake lib
#r "../packages/FAKE/tools/FakeLib.dll"
open Fake

RestorePackages()

// Properties
let artifactsDir = @".\artifacts\"
let artifactsBuildDir = "./artifacts/build/"
let artifactsTestsDir  = "./artifacts/tests/"
let artifactsResultsNugetSrcDir  = @".\artifacts\nuget-results-src\"
let artifactsResultsNugetPkgDir  = @".\artifacts\nuget-results-pkg\"
let toolNugetExe = @".nuget\nuget.exe"

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
let resultsNuGetPkg = @".\src\Sloader.Results\Sloader.Results.nuspec"

Target "CreateNuGetPackages" (fun _ ->

    trace "Create Assembly for NuGet Packages..."
    // The Path stuff is sooo wrong, but I have no idea how to do this elegant
    !! "src/Sloader.Results/*.csproj"
     |> MSBuildRelease artifactsResultsNugetSrcDir "Build"
     |> Log "TestBuild-Output: "

    trace "Create NuGet Packages..."
    CreateDir artifactsResultsNugetPkgDir
    let result =
        ExecProcess (fun info -> 
            info.FileName <- toolNugetExe
            info.Arguments <- "pack " + resultsNuGetPkg + " -OutputDirectory " + artifactsResultsNugetPkgDir + " -BasePath " + artifactsResultsNugetSrcDir
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