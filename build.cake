var target = Argument("Target", "Default");
var configuration =
    HasArgument("Configuration") ? Argument<string>("Configuration") :
    EnvironmentVariable("Configuration") is object ? EnvironmentVariable("Configuration") :
    "Release";

var artefactsDirectory = Directory("./Artefacts");

Task("Clean")
    .Description("Cleans the artefacts, bin and obj directories.")
    .Does(() =>
    {
        CleanDirectory(artefactsDirectory);
        DeleteDirectories(GetDirectories("**/bin"), new DeleteDirectorySettings() { Force = true, Recursive = true });
        DeleteDirectories(GetDirectories("**/obj"), new DeleteDirectorySettings() { Force = true, Recursive = true });
    });

Task("Restore")
    .Description("Restores NuGet packages.")
    .IsDependentOn("Clean")
    .Does(() =>
    {
        DotNetCoreRestore();
    });

Task("Build")
    .Description("Builds the solution.")
    .IsDependentOn("Restore")
    .Does(() =>
    {
        DotNetCoreBuild(
            ".",
            new DotNetCoreBuildSettings()
            {
                Configuration = configuration,
                NoRestore = true,
            });
    });

Task("Test")
    .Description("Runs unit tests and outputs test results to the artefacts directory.")
    .DoesForEach(GetFiles("./Tests/**/*.csproj"), project =>
    {
        DotNetCoreTest(
            project.ToString(),
            new DotNetCoreTestSettings()
            {
                Configuration = configuration,
                Logger = $"trx;LogFileName={project.GetFilenameWithoutExtension()}.trx",
                NoBuild = true,
                NoRestore = true,
                ResultsDirectory = artefactsDirectory,
                ArgumentCustomization = x => x
                    .Append("--blame")
                    .AppendSwitch("--logger", $"html;LogFileName={project.GetFilenameWithoutExtension()}.html")
                    .Append("--collect:\"XPlat Code Coverage\""),
            });
    });

Task("Publish")
    .Description("Publishes the solution.")
    .Does(() =>
    {
        Information(artefactsDirectory.GetType().Name);
        DotNetCorePublish(
            ".",
            new DotNetCorePublishSettings()
            {
                Configuration = configuration,
                NoBuild = true,
                NoRestore = true,
                OutputDirectory = artefactsDirectory + Directory("Publish"),
            });
    });

Task("DockerBuild")
    .Description("Builds a Docker image.")
    .DoesForEach(GetFiles("./**/Dockerfile"), dockerfile =>
    {
        var directoryBuildPropsFilePath = GetFiles("Directory.Build.props").Single().ToString();
        var directoryBuildPropsDocument = System.Xml.Linq.XDocument.Load(directoryBuildPropsFilePath);
        var preReleasePhase = directoryBuildPropsDocument.Descendants("MinVerDefaultPreReleasePhase").Single().Value;

        string version = null;
        StartProcess(
            "dotnet",
            new ProcessSettings()
                .WithArguments(x => x
                    .Append("minver")
                    .AppendSwitch("--default-pre-release-phase", preReleasePhase))
                .SetRedirectStandardOutput(true)
                .SetRedirectedStandardOutputHandler(
                    output =>
                    {
                        if (output != null)
                        {
                            version = output;
                        }
                        return output;
                    }));

        // Uncomment the following lines if using docker buildx.
        StartProcess(
            "docker",
            new ProcessArgumentBuilder()
                //.Append("buildx")
                .Append("build")
                //.AppendSwitch("--progress", "plain")
                .AppendSwitchQuoted("--tag", $"{dockerfile.GetDirectory().GetDirectoryName().ToLower()}:{version}")
                .AppendSwitchQuoted("--build-arg", $"Configuration={configuration}")
                .AppendSwitchQuoted("--label", $"org.opencontainers.image.created={DateTimeOffset.UtcNow:o}")
                .AppendSwitchQuoted("--label", $"org.opencontainers.image.version={version}")
                .AppendSwitchQuoted("--file", dockerfile.ToString())
                .Append(".")
                .RenderSafe());
    });

Task("Default")
    .Description("Cleans, restores NuGet packages, builds the solution, runs unit tests and then builds a Docker image.")
    .IsDependentOn("Build")
    .IsDependentOn("Test")
    .IsDependentOn("DockerBuild");

RunTarget(target);
