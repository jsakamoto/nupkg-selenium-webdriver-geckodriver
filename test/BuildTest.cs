using System.IO;
using NUnit.Framework;
using Selenium.WebDriver.GeckoDriver.NuPkg.Test.Lib;
using static Selenium.WebDriver.GeckoDriver.NuPkg.Test.Lib.ExecutableFile;

namespace Selenium.WebDriver.GeckoDriver.NuPkg.Test
{
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
        public void BuildWithRuntimeIdentifier_Test(string rid, string driverFileName, Format executableFileFormat)
        {
            using var workSpace = new WorkSpace(copyFrom: "Project");

            var exitCode = Shell.Run(workSpace, "dotnet", "build", "-r", rid, "-o", "out");
            exitCode.Is(0);

            var driverFullPath = Path.Combine(workSpace, "out", driverFileName);
            File.Exists(driverFullPath).IsTrue();

            DetectFormat(driverFullPath).Is(executableFileFormat);
        }

        [Test]
        [TestCaseSource(nameof(Runtimes))]
        public void PublishWithRuntimeIdentifier_NoPublish_Test(string rid, string driverFileName, Format _)
        {
            using var workSpace = new WorkSpace(copyFrom: "Project");

            var exitCode = Shell.Run(workSpace, "dotnet", "publish", "-r", rid, "-o", "out");
            exitCode.Is(0);

            var driverFullPath = Path.Combine(workSpace, "out", driverFileName);
            File.Exists(driverFullPath).IsFalse();
        }

        [Test]
        [TestCaseSource(nameof(Runtimes))]
        public void PublishWithRuntimeIdentifier_with_MSBuildProp_Test(string rid, string driverFileName, Format executableFileFormat)
        {
            using var workSpace = new WorkSpace(copyFrom: "Project");

            var exitCode = Shell.Run(workSpace, "dotnet", "publish", "-r", rid, "-o", "out", "-p:PublishGeckoDriver=true");
            exitCode.Is(0);

            var driverFullPath = Path.Combine(workSpace, "out", driverFileName);
            File.Exists(driverFullPath).IsTrue();

            DetectFormat(driverFullPath).Is(executableFileFormat);
        }

        [Test]
        [TestCaseSource(nameof(Runtimes))]
        public void PublishWithRuntimeIdentifier_with_DefineConstants_Test(string rid, string driverFileName, Format executableFileFormat)
        {
            using var workSpace = new WorkSpace(copyFrom: "Project");

            var exitCode = Shell.Run(workSpace, "dotnet", "publish", "-r", rid, "-o", "out", "-p:DefineConstants=_PUBLISH_GECKODRIVER");
            exitCode.Is(0);

            var driverFullPath = Path.Combine(workSpace, "out", driverFileName);
            File.Exists(driverFullPath).IsTrue();

            DetectFormat(driverFullPath).Is(executableFileFormat);
        }

        [Test]
        public void Publish_with_SingleFileEnabled_Test()
        {
            var rid = "win-x64";
            var driverFileName = "geckodriver.exe";
            var executableFileFormat = Format.PE64;

            using var workSpace = new WorkSpace(copyFrom: "Project");
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
                var exitCode = Shell.Run(workSpace, publishCommand);
                exitCode.Is(0);

                var driverFullPath = Path.Combine(workSpace, "out", driverFileName);
                File.Exists(driverFullPath).IsTrue();

                DetectFormat(driverFullPath).Is(executableFileFormat);
            }
        }
    }
}
