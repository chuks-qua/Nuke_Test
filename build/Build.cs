using System;
using System.Linq;
using System.Numerics;
using Nuke.Common;
using Nuke.Common.CI;
using Nuke.Common.CI.GitHubActions;
using Nuke.Common.Execution;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Utilities.Collections;
using static Nuke.Common.EnvironmentInfo;
using static Nuke.Common.IO.FileSystemTasks;
using static Nuke.Common.IO.PathConstruction;
using static Nuke.Common.Tools.DotNet.DotNetTasks;

[GitHubActions(
        "build-and-test",
        GitHubActionsImage.UbuntuLatest,
        OnPullRequestBranches = new[] { "master", "main" },
        InvokedTargets = new[] { nameof(RunTests) }
        )
    ]
class Build : NukeBuild
{
    /// Support plugins are available for:
    ///   - JetBrains ReSharper        https://nuke.build/resharper
    ///   - JetBrains Rider            https://nuke.build/rider
    ///   - Microsoft VisualStudio     https://nuke.build/visualstudio
    ///   - Microsoft VSCode           https://nuke.build/vscode

    public static int Main () => Execute<Build>(x => x.RunTests);

    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;
    [Solution] readonly Solution Solution;
        
    //[Parameter("Hello param desc")]
    //readonly string Hello;
    //bool proceed = false;

    //[PathExecutable] readonly Tool Git;

    Target Clean => _ => _
        .Before(Restore)
        .Executes(() =>
        {
            //Git("status");
        });


    Target Restore => _ => _
        .DependsOn(Clean)
        .Executes(() =>
        {
            DotNetRestoreSettings settings = new ();
            settings.SetProjectFile(Solution);
        });

    Target Compile => _ => _
        .DependsOn(Restore, Clean)
        .Executes(() =>
        {
           DotNetBuildSettings settings = new ();
            settings.SetProjectFile(Solution)
                .SetConfiguration(Configuration)
                .EnableNoRestore();
            DotNetBuild(settings);
        });

    Target UnitTests => _ => _ 
        .DependsOn(Compile)
        .Executes(() =>
        {
            DotNetTest(s => s
                .SetProjectFile(RootDirectory / "TestApi.UnitTest")
                .EnableNoBuild()
                .EnableNoRestore()
            );
        });

    public IProcess APIProcess { get; private set; }
    Target StartApi => _ => _
        .DependsOn(Compile)
        .Triggers(StopApi)
        .Executes(() =>
        {
            APIProcess = ProcessTasks.StartProcess("dotnet", "run", RootDirectory / "TestApi");
            //Logger.Normal(APIProcess.);
            
        });

    Target FunctionalTests => _ => _
        .DependsOn(StartApi, Compile)
        .Triggers(StopApi)
        .Executes(() =>
        {
            DotNetTest(s => s
                .SetProjectFile(RootDirectory / "TestApi.FunctionalTest")
                .EnableNoBuild()
                .EnableNoRestore()
            );
        });


    Target StopApi => _ => _
        .DependsOn(FunctionalTests)
        .AssuredAfterFailure()
        .Executes(() =>
        {
            APIProcess.Kill();
        });


    Target RunTests => _ => _.DependsOn(UnitTests, FunctionalTests);
}
