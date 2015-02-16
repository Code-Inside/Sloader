// include Fake lib
#r "../packages/FAKE/tools/FakeLib.dll"
open Fake

RestorePackages()

// Properties
let artifactsDir = @".\artifacts\"
let artifactsBuildDir = "./artifacts/build/"
let artifactsTestsDir  = "./artifacts/tests/"
let artifactsNugetDir  = @".\artifacts\nuget\"
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



let nuspec = @".\src\Sloader.Results\Sloader.Results.nuspec"
// Poor mans NuGet Pack Solution for FAKE...  
Target "CreateNuGetPackages" (fun _ ->
    trace "Create NuGet Packages..."
    CreateDir artifactsNugetDir
    let result =
        ExecProcess (fun info -> 
            info.FileName <- toolNugetExe
            info.Arguments <- "pack " + nuspec + " -OutputDirectory " + artifactsNugetDir
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