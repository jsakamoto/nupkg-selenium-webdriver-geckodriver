namespace Selenium.WebDriver.GeckoDriver.NuPkg.Test;

[Parallelizable(ParallelScope.All)]
public class BuildTest
{
    public static object[][] Runtimes => new object[][]{
        new object[] { "win7-x86", "geckodriver.exe", Format.PE32 },
        new object[] { "win-x64", "geckodriver.exe", Format.PE64 },
        new object[] { "osx.10.12-x64", "geckodriver", Format.MachO },
        new object[] { "linux-x64", "geckodriver", Format.ELF },
    };

    [Test]
    [TestCaseSource(nameof(Runtimes))]
    public async Task BuildWithRuntimeIdentifier_Test(string rid, string driverFileName, Format executableFileFormat)
    {
        var unitTestProjectDir = FileIO.FindContainerDirToAncestor("*.csproj");
        using var workDir = WorkDirectory.CreateCopyFrom(Path.Combine(unitTestProjectDir, "Project"), item => item.Name is not "obj" and not "bin");

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
        var unitTestProjectDir = FileIO.FindContainerDirToAncestor("*.csproj");
        using var workDir = WorkDirectory.CreateCopyFrom(Path.Combine(unitTestProjectDir, "Project"), item => item.Name is not "obj" and not "bin");

        var dotnet = await XProcess.Start("dotnet", $"publish -r {rid} -o out", workDir).WaitForExitAsync();
        dotnet.ExitCode.Is(0, message: dotnet.Output);

        var driverFullPath = Path.Combine(workDir, "out", driverFileName);
        File.Exists(driverFullPath).IsFalse();
    }

    [Test]
    [TestCaseSource(nameof(Runtimes))]
    public async Task PublishWithRuntimeIdentifier_with_MSBuildProp_Test(string rid, string driverFileName, Format executableFileFormat)
    {
        var unitTestProjectDir = FileIO.FindContainerDirToAncestor("*.csproj");
        using var workDir = WorkDirectory.CreateCopyFrom(Path.Combine(unitTestProjectDir, "Project"), item => item.Name is not "obj" and not "bin");

        var dotnet = await XProcess.Start("dotnet", $"publish -r {rid} -o out -p:PublishGeckoDriver=true", workDir).WaitForExitAsync();
        dotnet.ExitCode.Is(0, message: dotnet.Output);

        var driverFullPath = Path.Combine(workDir, "out", driverFileName);
        File.Exists(driverFullPath).IsTrue();

        DetectFormat(driverFullPath).Is(executableFileFormat);
    }

    [Test]
    [TestCaseSource(nameof(Runtimes))]
    public async Task PublishWithRuntimeIdentifier_with_DefineConstants_Test(string rid, string driverFileName, Format executableFileFormat)
    {
        var unitTestProjectDir = FileIO.FindContainerDirToAncestor("*.csproj");
        using var workDir = WorkDirectory.CreateCopyFrom(Path.Combine(unitTestProjectDir, "Project"), item => item.Name is not "obj" and not "bin");

        var dotnet = await XProcess.Start("dotnet", $"publish -r {rid} -o out -p:DefineConstants=_PUBLISH_GECKODRIVER", workDir).WaitForExitAsync();
        dotnet.ExitCode.Is(0, message: dotnet.Output);

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

        var unitTestProjectDir = FileIO.FindContainerDirToAncestor("*.csproj");
        using var workDir = WorkDirectory.CreateCopyFrom(Path.Combine(unitTestProjectDir, "Project"), item => item.Name is not "obj" and not "bin");
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
            var dotnet = await XProcess.Start(
                filename: publishCommand.First(),
                arguments: String.Join(' ', publishCommand.Skip(1)),
                workDir).WaitForExitAsync();
            dotnet.ExitCode.Is(0, message: dotnet.Output);

            var driverFullPath = Path.Combine(workDir, "out", driverFileName);
            File.Exists(driverFullPath).IsTrue();

            DetectFormat(driverFullPath).Is(executableFileFormat);
        }
    }
}
