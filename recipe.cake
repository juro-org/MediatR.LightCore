#load nuget:?package=Cake.Recipe&version=2.1.0

Environment.SetVariableNames();

BuildParameters.SetParameters(
  context: Context,
  buildSystem: BuildSystem,
  sourceDirectoryPath: "./src/MediatR.LightCore",
  title: "MediatR.LightCore",
  masterBranchName: "main",
  repositoryOwner: "JuergenRB",
  shouldRunDotNetCorePack: true,
  shouldUseDeterministicBuilds: true,
  preferredBuildAgentOperatingSystem: PlatformFamily.Linux,
  preferredBuildProviderType: BuildProviderType.GitHubActions);

BuildParameters.PrintParameters(Context);

ToolSettings.SetToolSettings(context: Context);

Build.RunDotNetCore();
