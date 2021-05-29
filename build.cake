#module nuget:?package=Cake.DotNetTool.Module&version=0.4.0
#addin nuget:?package=Cake.Coverlet&version=2.5.4
#tool dotnet:?package=dotnet-reportgenerator-globaltool&version=4.8.7

var target = Argument("target", "Publish");
var configuration = Argument("configuration", "Release");
var solutionFolder = "./";
var outputFolder = "./artifacts";
var reportTypes = "Html";
var coverageFolder = "./code_coverage";


var coverageFile = "results";
var coverageFileExtension = ".coverage.xml";
var coverageFilePath = Directory(coverageFolder) + File(coverageFile + coverageFileExtension);

Task("Clean")
    .Does(() => {
        CleanDirectory(outputFolder);
        CleanDirectory(coverageFolder);
    });

Task("Restore")
    .Does(() => {
    DotNetCoreRestore(solutionFolder);
    });

Task("Build")
    .IsDependentOn("Clean")
    .IsDependentOn("Restore")
    .Does(() => {
        DotNetCoreBuild(solutionFolder, new DotNetCoreBuildSettings
        {
            NoRestore = true,
            Configuration = configuration
        });
    });

    
Task("Test")
    .IsDependentOn("Build")
    .Does(() => {    
        var coverletSettings = new CoverletSettings {
            CollectCoverage = true,
            CoverletOutputFormat = CoverletOutputFormat.cobertura,
            CoverletOutputDirectory = coverageFolder,
            CoverletOutputName = coverageFile
        };

        var testSettings = new DotNetCoreTestSettings
        {
            NoRestore = true,
            Configuration = configuration,
            NoBuild = true
        };

        DotNetCoreTest("./test/Solnet.Rpc.Test/Solnet.Rpc.Test.csproj", testSettings, coverletSettings);
    });


Task("Report")
    .IsDependentOn("Test")
    .Does(() =>
{
    var reportSettings = new ReportGeneratorSettings
    {
        ArgumentCustomization = args => args.Append($"-reportTypes:{reportTypes}")
    };
    ReportGenerator(coverageFilePath, Directory(coverageFolder), reportSettings);
});


Task("Publish")
    .IsDependentOn("Report")
    .Does(() => {
        DotNetCorePublish(solutionFolder, new DotNetCorePublishSettings
        {
            NoRestore = true,
            Configuration = configuration,
            NoBuild = true,
            OutputDirectory = outputFolder
        });
    });

RunTarget(target);