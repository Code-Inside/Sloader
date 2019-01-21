#tool "nuget:?package=xunit.runner.console"
//////////////////////////////////////////////////////////////////////
// ARGUMENTS
//////////////////////////////////////////////////////////////////////

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");

//////////////////////////////////////////////////////////////////////
// PREPARATION
//////////////////////////////////////////////////////////////////////

// Define directories.
var artifactsDir  = Directory("./artifacts/");
var rootAbsoluteDir = MakeAbsolute(Directory("./")).FullPath;

//////////////////////////////////////////////////////////////////////
// TASKS
//////////////////////////////////////////////////////////////////////

Task("Clean")
    .Does(() =>
{
    CleanDirectory(artifactsDir);
});

Task("Restore-NuGet-Packages")
    .IsDependentOn("Clean")
    .Does(() =>
{
    NuGetRestore("./Sloader.sln");
});

Task("BuildPackages")
	.IsDependentOn("Test")
    .Does(() =>
{
	 var settings = new DotNetCorePackSettings
     {
         Configuration = "Release",
		 NoBuild = false,
         OutputDirectory =  rootAbsoluteDir + @"\artifacts\",
		 ArgumentCustomization = args=> args.Append("--include-symbols").Append("-p:SymbolPackageFormat=snupkg")
     };

     DotNetCorePack("./src/Sloader.Config/Sloader.Config.csproj", settings);
	 DotNetCorePack("./src/Sloader.Result/Sloader.Result.csproj", settings);
	 DotNetCorePack("./src/Sloader.Engine/Sloader.Engine.csproj", settings);
});

Task("Test")
	.IsDependentOn("Restore-NuGet-Packages")
    .Does(() =>
{
    Information("Start Building and Running Tests");
    var parsedSolution = ParseSolution("./Sloader.sln");

	foreach(var project in parsedSolution.Projects)
	{
	
	if(project.Name.EndsWith(".Tests"))
		{
        Information("Start Building Test: " + project.Name);

		DotNetCoreTest(
                project.Path.ToString(),
                new DotNetCoreTestSettings()
                {
                    Configuration = configuration,
                    NoBuild = false
                });
		}
	
	}    
});

//////////////////////////////////////////////////////////////////////
// TASK TARGETS
//////////////////////////////////////////////////////////////////////

Task("Default")
    .IsDependentOn("Test")
	.IsDependentOn("BuildPackages");

//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////

RunTarget(target);