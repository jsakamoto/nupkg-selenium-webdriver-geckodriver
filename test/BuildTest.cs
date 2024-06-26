namespace Selenium.WebDriver.GeckoDriver.NuPkg.Test;

[Parallelizable(ParallelScope.All)]
public class BuildTest
{
    public static object[][] Runtimes => [
        ["win-x86", "geckodriver.exe", Format.PE32],
        ["win-x64", "geckodriver.exe", Format.PE64],
        ["osx-x64", "geckodriver", Format.MachO],
        ["osx-arm64", "geckodriver", Format.MachO],
        ["linux-x64", "geckodriver", Format.ELF],
    ];

    private static WorkDirectory CreateWorkDir()
    {
        var unitTestProjectDir = FileIO.FindContainerDirToAncestor("*.csproj");
        return WorkDirectory.CreateCopyFrom(Path.Combine(unitTestProjectDir, "Project"), predicate: item => item.Name is not "obj" and not "bin");
    }

    [Test]
    [TestCaseSource(nameof(Runtimes))]
    public async Task BuildWithRuntimeIdentifier_Test(string rid, string driverFileName, Format executableFileFormat)
    {
        using var workDir = CreateWorkDir();

        var dotnet = await XProcess.Start("dotnet", $"build -r {rid} -o out", workDir).WaitForExitAsync();
        dotnet.ExitCode.Is(0, message: dotnet.Output);

        var driverFullPath = Path.Combine(workDir, "out", driverFileName);
        File.Exists(driverFullPath).IsTrue();

        DetectFormat(driverFullPath).Is(executableFileFormat);
    }

    [Test]
    [TestCaseSource(nameof(Runtimes))]
    public async Task PublishWithRuntimeIdentifier_NoPublish_Test(string rid, string driverFileName, Format _)
    {
        using var workDir = CreateWorkDir();

        var dotnet = await XProcess.Start("dotnet", $"publish -r {rid} -o out", workDir).WaitForExitAsync();
        dotnet.ExitCode.Is(0, message: dotnet.Output);

        var driverFullPath = Path.Combine(workDir, "out", driverFileName);
        File.Exists(driverFullPath).IsFalse();
    }

    [Test]
    [TestCaseSource(nameof(Runtimes))]
    public async Task PublishWithRuntimeIdentifier_with_MSBuildProp_Test(string rid, string driverFileName, Format executableFileFormat)
    {
        using var workDir = CreateWorkDir();

        await XProcess.Start("dotnet", $"publish -r {rid} -o out -p:PublishGeckoDriver=true", workDir)
            .ExitCodeIs(0);

        var driverFullPath = Path.Combine(workDir, "out", driverFileName);
        File.Exists(driverFullPath).IsTrue();

        DetectFormat(driverFullPath).Is(executableFileFormat);
    }

    [Test]
    [TestCaseSource(nameof(Runtimes))]
    public async Task PublishWithRuntimeIdentifier_with_DefineConstants_Test(string rid, string driverFileName, Format executableFileFormat)
    {
        using var workDir = CreateWorkDir();

        await XProcess.Start("dotnet", $"publish -r {rid} -o out -p:DefineConstants=_PUBLISH_GECKODRIVER", workDir)
            .ExitCodeIs(0);

        var driverFullPath = Path.Combine(workDir, "out", driverFileName);
        File.Exists(driverFullPath).IsTrue();

        DetectFormat(driverFullPath).Is(executableFileFormat);
    }

    [Test]
    public async Task Publish_with_SingleFileEnabled_Test()
    {
        var rid = "win-x64";
        var driverFileName = "geckodriver.exe";
        var executableFileFormat = Format.PE64;

        using var workDir = CreateWorkDir();

        var publishCommand = new[] {
            "dotnet", "publish", "-r", rid, "-o", "out",
            "-c:Release",
            "-p:PublishGeckoDriver=true",
            "-p:PublishSingleFile=true",
            "-p:SelfContained=false"
        };

        // IMPORTANT: 2nd time of publishing, sometimes lost driver file in the published folder, so we have to validate it..
        for (var i = 0; i < 2; i++)
        {
            await XProcess.Start(
                filename: publishCommand.First(),
                arguments: String.Join(' ', publishCommand.Skip(1)),
                workDir).ExitCodeIs(0);

            var driverFullPath = Path.Combine(workDir, "out", driverFileName);
            File.Exists(driverFullPath).IsTrue();

            DetectFormat(driverFullPath).Is(executableFileFormat);
        }
    }
}
