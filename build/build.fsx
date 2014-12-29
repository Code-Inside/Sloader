// include Fake lib
#r "../packages/FAKE/tools/FakeLib.dll"
open Fake

RestorePackages()

// Properties
let buildDir = "./artifacts/build/"
let testsDir  = "./artifacts/tests/"

// Targets
Target "Clean" (fun _ ->
    trace "Cleanup..."
    CleanDirs [buildDir; testsDir]
)

Target "BuildApp" (fun _ ->
   trace "Building App..."
   !! "src/**/*.csproj"
     |> MSBuildRelease buildDir "Build"
     |> Log "AppBuild-Output: "
)

Target "BuildTests" (fun _ ->
    trace "Building Tests..."
    !! "tests/**/*.csproj"
      |> MSBuildDebug testsDir "Build"
      |> Log "TestBuild-Output: "
)

Target "RunTests" (fun _ ->
    trace "Running Tests..."
    !! (testsDir + @"\*Tests.dll") 
      |> xUnit (fun p -> {p with OutputDir = testsDir })
)

Target "Default" (fun _ ->
    trace "Default Target invoked."
)

// Dependencies
"Clean"
  ==> "BuildApp"
  ==> "BuildTests"
  ==> "RunTests"
  ==> "Default"

// start build
RunTargetOrDefault "Default"