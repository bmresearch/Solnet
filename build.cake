#module nuget:?package=Cake.DotNetTool.Module&version=0.4.0
#addin nuget:?package=Cake.Coverlet&version=2.5.4
#tool dotnet:?package=dotnet-reportgenerator-globaltool&version=4.8.7

var testProjectsRelativePaths = new string[]
{
    "./test/Solnet.Rpc.Test/Solnet.Rpc.Test.csproj",
    "./test/Solnet.Wallet.Test/Solnet.Wallet.Test.csproj"
};

var target = Argument("target", "Publish");
var configuration = Argument("configuration", "Release");
var solutionFolder = "./";
var outputFolder = "./artifacts";
var reportTypes = "HtmlInline_AzurePipelines";
var coverageFolder = "./code_coverage";

var coberturaFileName = "results";
var coverageFilePath = Directory(coverageFolder) + File(coberturaFileName + ".cobertura.xml");
var jsonFilePath = Directory(coverageFolder) + File(coberturaFileName + ".json");

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
            CoverletOutputDirectory = coverageFolder,
            CoverletOutputName = coberturaFileName
        };

        var testSettings = new DotNetCoreTestSettings
        {
            NoRestore = true,
            Configuration = configuration,
            NoBuild = true,
            ArgumentCustomization = args => args.Append($"--logger trx")
        };

        DotNetCoreTest(testProjectsRelativePaths[0], testSettings, coverletSettings);

        coverletSettings.MergeWithFile = jsonFilePath;
        for (int i = 1; i < testProjectsRelativePaths.Length; i++)
        {
            if (i == testProjectsRelativePaths.Length - 1)
            {
                coverletSettings.CoverletOutputFormat  = CoverletOutputFormat.cobertura;
            }
            DotNetCoreTest(testProjectsRelativePaths[i], testSettings, coverletSettings);
        }
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