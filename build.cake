var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");

////////////////////////////////////////////////////////////////
// Tasks

Task("Build")
    .Does(context => 
{
    DotNetBuild("./src/Spectre.Tui.sln", new DotNetBuildSettings {
        Configuration = configuration,
        NoIncremental = context.HasArgument("rebuild"),
        MSBuildSettings = new DotNetMSBuildSettings()
            .TreatAllWarningsAs(MSBuildTreatAllWarningsAs.Error)
    });
});

Task("Test")
    .IsDependentOn("Build")
    .Does(context => 
{
    DotNetTest("./src/Spectre.Tui.sln", new DotNetTestSettings {
        Configuration = configuration,
        NoRestore = true,
        NoBuild = true,
    });
});

Task("Package")
    .IsDependentOn("Test")
    .Does(context => 
{
    context.CleanDirectory("./.artifacts");

    context.DotNetPack($"./src/Spectre.Tui/Spectre.Tui.csproj", new DotNetPackSettings {
        Configuration = configuration,
        NoRestore = true,
        NoBuild = true,
        OutputDirectory = "./.artifacts",
        MSBuildSettings = new DotNetMSBuildSettings()
            .TreatAllWarningsAs(MSBuildTreatAllWarningsAs.Error)
    });
});

////////////////////////////////////////////////////////////////
// Targets

Task("Default")
    .IsDependentOn("Package");

////////////////////////////////////////////////////////////////
// Execution

RunTarget(target)