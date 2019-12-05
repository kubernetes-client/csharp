var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");

// Repository-specific variables for build tasks
var buildNumber = EnvironmentVariable("build_number") ?? "0.0.5";
var artifactsDir = "./artifacts";
var projectToPublish = "./src/KubernetesClient/KubernetesClient.csproj";
var feedzKey = EnvironmentVariable("NUGET_FEEDZ_API_KEY");

// Shared build tasks that hopefully should be copy-pastable
Information($"Running on TeamCity: {TeamCity.IsRunningOnTeamCity}");
Information($"Building: {buildNumber}");

Task("Clean")
    .Does(() =>
{
    CleanDirectory(artifactsDir);
});

Task("Restore")
    .IsDependentOn("Clean")
    .Does(() => {
        DotNetCoreRestore(projectToPublish);
    });

Task("Pack-NuGet")
    .IsDependentOn("Restore")
    .Does(() => {
        DotNetCorePack(projectToPublish, new DotNetCorePackSettings {
			Configuration = configuration,
            OutputDirectory = artifactsDir,
			ArgumentCustomization = args => args.Append($"/p:Version={buildNumber}")
        });
    });

Task("Push-NuGet")
    .IsDependentOn("Pack-NuGet")
    .Does(() => {
        if (!String.IsNullOrEmpty (feedzKey)) {
            Information("Have a feedz key so pushing package");

            DotNetCoreNuGetPush($"./{artifactsDir}/KubernetesClient.{buildNumber}.nupkg", new DotNetCoreNuGetPushSettings {
                Source = "https://f.feedz.io/gearsethq/gearset-kubernetes-client/nuget",
                ApiKey = feedzKey
            });
        } else {
            Information("No Feedz key so skipping package push");
        }
    });

Task("Default")
    .IsDependentOn("Push-NuGet");

RunTarget(target);
