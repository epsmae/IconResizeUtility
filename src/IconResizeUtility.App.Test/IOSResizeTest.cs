using System.Collections.Generic;
using System.IO;
using IconResizeUtility.Service;
using IconResizeUtility.TestInfrastructure;
using NUnit.Framework;

namespace IconResizeUtility.App.Test
{
    public class IOSResizeTest
    {
        private IOSResultChecker _iOSResultChecker;

        private string SrcDataDir
        {
            get
            {
                return Path.Combine(TestContext.CurrentContext.TestDirectory, "TestData");
            }
        }

        private string ProjectFile
        {
            get
            {
                return Path.Combine(TestContext.CurrentContext.TestDirectory, "TestProjectFile", "ResizeUtility.App.iOS.csproj");
            }
        }
        
        private string OutDir
        {
            get
            {
                return Path.Combine(TestContext.CurrentContext.WorkDirectory, "out", "Icons");
            }
        }

        [SetUp]
        public void Setup()
        {
            ImageResizer resizer = new ImageResizer();
            ImageRenamer imageRenamer = new ImageRenamer();
            _iOSResultChecker = new IOSResultChecker(resizer, imageRenamer);

            if (Directory.Exists(OutDir))
            {
                Directory.Delete(OutDir, true);
            }
        }

        [Test]
        public void TestVersion()
        {
            Program.Main(new []{"--version"});
        }

        [Test]
        public void TestHelp()
        {
            Program.Main(new[] { "resize", "--help" });
        }

        [Test]
        public void TestResizeWithoutSize()
        {
            IList<int> expectedSizes = AndroidResizeService.DefaultRequiredSizes;

            Program.Main(new[] { "resize", "--type", "ios", "--dstFolder", OutDir, "--srcFolder", SrcDataDir, "--prefix", "icon_", "--postfixSize", "false" });

            _iOSResultChecker.AssertIconsExistAndMatchSize(SrcDataDir, OutDir, expectedSizes, true, "icon_");
            _iOSResultChecker.AssertIconCount(SrcDataDir, OutDir, expectedSizes);
        }

        [Test]
        public void TestResizeWithSingleSize()
        {
            IList<int> expectedSizes = new List<int> {42};

            Program.Main(new[] { "resize", "--type", "ios", "--dstFolder", OutDir, "--srcFolder", SrcDataDir, "--prefix", "icon_", "--iconSize", "42", "--postfixSize", "false" });

            _iOSResultChecker.AssertIconsExistAndMatchSize(SrcDataDir, OutDir, expectedSizes, false, "icon_");
            _iOSResultChecker.AssertIconCount(SrcDataDir, OutDir, expectedSizes);
        }

        [Test]
        public void TestResizeWithSingleSizePostfix()
        {
            IList<int> expectedSizes = new List<int> { 42 };

            Program.Main(new[] { "resize", "--type", "ios",  "--dstFolder", OutDir, "--srcFolder", SrcDataDir, "--iconSize", "42", "--postfixSize", "true" });

            _iOSResultChecker.AssertIconsExistAndMatchSize(SrcDataDir, OutDir, expectedSizes, true, "");
            _iOSResultChecker.AssertIconCount(SrcDataDir, OutDir, expectedSizes);
        }

        [Test]
        public void TestResizeWithMultipleSizes()
        {
            IList<int> expectedSizes = new List<int> { 18, 28, 38 };

            Program.Main(new[] { "resize", "--type", "ios", "--dstFolder", OutDir, "--srcFolder", SrcDataDir, "--prefix", "icon_", "--iconSize", "18,28, 38", "--postfixSize", "false" });

            _iOSResultChecker.AssertIconsExistAndMatchSize(SrcDataDir, OutDir, expectedSizes, true, "icon_");
            _iOSResultChecker.AssertIconCount(SrcDataDir, OutDir, expectedSizes);
        }


        [Test]
        public void TestCsprojUpdate()
        {
            IList<int> expectedSizes = new List<int> { 42 };

            Program.Main(new[] { "resize", "--type", "ios", "--dstFolder", OutDir, "--srcFolder", SrcDataDir, "--prefix", "icon_", "--iconSize", "42", "--postfixSize", "false", "--csproj", ProjectFile });

            _iOSResultChecker.AssertIconsExistAndMatchSize(SrcDataDir, OutDir, expectedSizes, false, "icon_");
            _iOSResultChecker.AssertIconCount(SrcDataDir, OutDir, expectedSizes);

            new ProjectFileTester(new DroidProjectFileUpdater()).AssertContainsIcon(ProjectFile, new List<string>
            {
                "Assets.xcassets\\icon_material_icon_bug.imageset\\icon_material_icon_bug_1x.png",
                "Assets.xcassets\\icon_material_icon_bug.imageset\\icon_material_icon_bug_2x.png",
                "Assets.xcassets\\icon_material_icon_bug.imageset\\icon_material_icon_bug_3x.png",
                "Assets.xcassets\\icon_material_icon_bug.imageset\\Contents.json",
                "Assets.xcassets\\icon_material_icon_build.imageset\\icon_material_icon_build_1x.png",
                "Assets.xcassets\\icon_material_icon_build.imageset\\icon_material_icon_build_2x.png",
                "Assets.xcassets\\icon_material_icon_build.imageset\\icon_material_icon_build_3x.png",
                "Assets.xcassets\\icon_material_icon_build.imageset\\Contents.json"
            });
        }
    }
}