/////////////////////////////////////////////////////////////////////
// ARGUMENTS
//////////////////////////////////////////////////////////////////////

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");

//////////////////////////////////////////////////////////////////////
// PREPARATION
//////////////////////////////////////////////////////////////////////

// Define directories.
var buildDir = Directory("./src/Example/bin") + Directory(configuration);
var buildResultDir = Directory("./build");
var testResultDir = buildResultDir + Directory("test-results");
var binDir = buildResultDir + Directory("bin");

// version
var version = "0.0.1";
var buildNumber = EnvironmentVariable("BUILD_NUMBER");
var semVersion = buildNumber == null ? version : (version + string.Concat(".", buildNumber));

// project specific
var solution = "./src/Example.sln";
var application = "Example.exe";
var solutionInfo = "./src/SolutionInfo.cs";
var nuspecFile = "./nuspec/Example.nuspec";

// meta data
var projectName = "Example";
var description = "";
var authors = new[] {"Yves Schmid"};
var copyright = "Copyright (c) Yves Schmid 2015";


//////////////////////////////////////////////////////////////////////
// TASKS
//////////////////////////////////////////////////////////////////////

Task("Clean")
    .Does(() =>
{
    CleanDirectories(new DirectoryPath[] {
        buildResultDir, testResultDir, binDir
    });
    CleanDirectories("src/**/bin/" + configuration);
});

Task("Restore-NuGet-Packages")
    .IsDependentOn("Clean")
    .Does(() =>
{
    NuGetRestore(solution);
});

Task("Patch-Assembly-Info")
    .IsDependentOn("Restore-NuGet-Packages")
    .Does(() =>
{
    CreateAssemblyInfo(solutionInfo, new AssemblyInfoSettings {
        Product = projectName,
        Description = description,
        Version = version,
        FileVersion = version,
        InformationalVersion = semVersion,
        Copyright = copyright
    });
});

Task("Build")
    .IsDependentOn("Patch-Assembly-Info")
    .Does(() =>
{
    MSBuild(solution, settings =>
        settings.SetConfiguration(configuration)
            .UseToolVersion(MSBuildToolVersion.NET45));
});

Task("Run-Unit-Tests")
    .IsDependentOn("Build")
    .Does(() =>
{
    NUnit("./src/**/bin/" + configuration + "/*.Test.dll", new NUnitSettings {
        ResultsFile = testResultDir + File("TestResult.xml")
    });
});

Task("Copy-Files")
    .IsDependentOn("Run-Unit-Tests")
    .Does(() =>
{
    CopyFileToDirectory(buildDir + File(application), binDir);
    CopyFileToDirectory(buildDir + File(application + ".config"), binDir);
});

Task("Zip-Files")
    .IsDependentOn("Copy-Files")
    .Does(() =>
{
    var packageFile = File(projectName + "-bin-v" + semVersion + ".zip");
    var packagePath = buildResultDir + packageFile;

    Zip(binDir, packagePath);
});

//////////////////////////////////////////////////////////////////////
// TASK TARGETS
//////////////////////////////////////////////////////////////////////

Task("Default")
    .IsDependentOn("Zip-Files");

//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////

RunTarget(target);
