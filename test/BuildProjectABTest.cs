namespace Selenium.WebDriver.GeckoDriver.NuPkg.Test;

[Parallelizable(ParallelScope.All)]
public class BuildProjectABTest
{
    [Test]
    public async Task Output_of_ProjectB_Contains_DriverFile_Test()
    {
        var unitTestProjectDir = FileIO.FindContainerDirToAncestor("*.csproj");
        using var workDir = WorkDirectory.CreateCopyFrom(Path.Combine(unitTestProjectDir, "ProjectAB"), item => item.Name is not "obj" and not "bin");

        var devenvExe = @"C:\Program Files\Microsoft Visual Studio\2022\Preview\Common7\IDE\devenv.exe";
        var nuget = await XProcess.Start("nuget", "restore", workDir).WaitForExitAsync();
        nuget.ExitCode.Is(0, message: nuget.Output);

        var devenv = await XProcess.Start(devenvExe, "ProjectAB.sln /Build", workDir).WaitForExitAsync();
        devenv.ExitCode.Is(0, message: devenv.Output);

        var outDir = Path.Combine(workDir, "ProjectB", "bin", "Debug", "net472");
        var driverFullPath1 = Path.Combine(outDir, "geckodriver");
        var driverFullPath2 = Path.Combine(outDir, "geckodriver.exe");
        (File.Exists(driverFullPath1) || File.Exists(driverFullPath2)).IsTrue();
    }
}
