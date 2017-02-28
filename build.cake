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
    .IsDependentOn("Restore-NuGet-Packages")
	.IsDependentOn("RunTests")
    .Does(() =>
{
    var nuGetPackSettings = new NuGetPackSettings
	{
		OutputDirectory = rootAbsoluteDir + @"\artifacts\",
		IncludeReferencedProjects = true,
		Properties = new Dictionary<string, string>
		{
			{ "Configuration", "Release" }
		}
	};

	 MSBuild("./src/Sloader.Config/Sloader.Config.csproj", new MSBuildSettings().SetConfiguration("Release"));
     NuGetPack("./src/Sloader.Config/Sloader.Config.csproj", nuGetPackSettings);
	 MSBuild("./src/Sloader.Result/Sloader.Result.csproj", new MSBuildSettings().SetConfiguration("Release"));
     NuGetPack("./src/Sloader.Result/Sloader.Result.csproj", nuGetPackSettings);
	 MSBuild("./src/Sloader.Engine/Sloader.Engine.csproj", new MSBuildSettings().SetConfiguration("Release"));
	 NuGetPack("./src/Sloader.Engine/Sloader.Engine.csproj", nuGetPackSettings);
});

Task("BuildTests")
    .IsDependentOn("Restore-NuGet-Packages")
    .Does(() =>
{
	var parsedSolution = ParseSolution("./Sloader.sln");

	foreach(var project in parsedSolution.Projects)
	{
	
	if(project.Name.EndsWith(".Tests"))
		{
        Information("Start Building Test: " + project.Name);

        MSBuild(project.Path, new MSBuildSettings()
                .SetConfiguration("Debug")
                .SetMSBuildPlatform(MSBuildPlatform.Automatic)
                .SetVerbosity(Verbosity.Minimal)
                .WithProperty("SolutionDir", @".\")
                .WithProperty("OutDir", rootAbsoluteDir + @"\artifacts\_tests\" + project.Name + @"\"));
		}
	
	}    

});

Task("RunTests")
    .IsDependentOn("BuildTests")
    .Does(() =>
{
    Information("Start Running Tests");
    XUnit2("./artifacts/_tests/**/*.Tests.dll");
});

//////////////////////////////////////////////////////////////////////
// TASK TARGETS
//////////////////////////////////////////////////////////////////////

Task("Default")
    .IsDependentOn("RunTests")
	.IsDependentOn("BuildPackages");

//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////

RunTarget(target);