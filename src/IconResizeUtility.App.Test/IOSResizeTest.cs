using System.Collections.Generic;
using System.IO;
using IconResizeUtility.Service;
using IconResizeUtility.Service.DataModel;
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

        private string WorkProjectFile
        {
            get
            {
                return Path.Combine(OutDir, "ResizeUtility.App.iOS.csproj");
            }
        }

        private string OutDir
        {
            get
            {
                return Path.Combine(TestContext.CurrentContext.WorkDirectory, "out");
            }
        }

        private string IconDir
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

            if (!Directory.Exists(Path.GetDirectoryName(WorkProjectFile)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(WorkProjectFile));
            }

            File.Copy(ProjectFile, WorkProjectFile);
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
            IList<int> expectedSizes = IOSImageResizeService.DefaultRequiredSizes;

            Program.Main(new[] { "resize", "--type", "ios", "--dstFolder", IconDir, "--srcFolder", SrcDataDir, "--prefix", "icon_", "--postfixSize", "false" });

            _iOSResultChecker.AssertIconsExistAndMatchSize(SrcDataDir, IconDir, expectedSizes, true, "icon_");
            _iOSResultChecker.AssertIconCount(SrcDataDir, IconDir, expectedSizes);
        }

        [Test]
        public void TestResizeWithSingleSize()
        {
            IList<int> expectedSizes = new List<int> {42};

            Program.Main(new[] { "resize", "--type", "ios", "--dstFolder", IconDir, "--srcFolder", SrcDataDir, "--prefix", "icon_", "--iconSize", "42", "--postfixSize", "false" });

            _iOSResultChecker.AssertIconsExistAndMatchSize(SrcDataDir, IconDir, expectedSizes, false, "icon_");
            _iOSResultChecker.AssertIconCount(SrcDataDir, IconDir, expectedSizes);
        }

        [Test]
        public void TestResizeWithSingleSizePostfix()
        {
            IList<int> expectedSizes = new List<int> { 42 };

            Program.Main(new[] { "resize", "--type", "ios",  "--dstFolder", IconDir, "--srcFolder", SrcDataDir, "--iconSize", "42", "--postfixSize", "true" });

            _iOSResultChecker.AssertIconsExistAndMatchSize(SrcDataDir, IconDir, expectedSizes, true, "");
            _iOSResultChecker.AssertIconCount(SrcDataDir, IconDir, expectedSizes);
        }

        [Test]
        public void TestResizeWithMultipleSizes()
        {
            IList<int> expectedSizes = new List<int> { 18, 28, 38 };

            Program.Main(new[] { "resize", "--type", "ios", "--dstFolder", IconDir, "--srcFolder", SrcDataDir, "--prefix", "icon_", "--iconSize", "18,28, 38", "--postfixSize", "false" });

            _iOSResultChecker.AssertIconsExistAndMatchSize(SrcDataDir, IconDir, expectedSizes, true, "icon_");
            _iOSResultChecker.AssertIconCount(SrcDataDir, IconDir, expectedSizes);
        }


        [Test]
        public void TestCsprojUpdate()
        {
            IList<int> expectedSizes = new List<int> { 18, 28, 38 };

            Program.Main(new[] { "resize", "--type", "ios", "--dstFolder", IconDir, "--srcFolder", SrcDataDir, "--prefix", "icon_", "--iconSize", "18, 28, 38", "--postfixSize", "false", "--csproj", WorkProjectFile });

            _iOSResultChecker.AssertIconsExistAndMatchSize(SrcDataDir, IconDir, expectedSizes, true, "icon_");
            _iOSResultChecker.AssertIconCount(SrcDataDir, IconDir, expectedSizes);
            ProjectFileTester tester = new ProjectFileTester(new IOSProjectFileUpdater());
            tester.AssertContainsIcon(WorkProjectFile, new List<string>
            {
                "Assets.xcassets\\icon_material_icon_bug_18pt.imageset\\icon_material_icon_bug_18pt_1x.png",
                "Assets.xcassets\\icon_material_icon_bug_18pt.imageset\\icon_material_icon_bug_18pt_2x.png",
                "Assets.xcassets\\icon_material_icon_bug_18pt.imageset\\icon_material_icon_bug_18pt_3x.png",
                "Assets.xcassets\\icon_material_icon_bug_18pt.imageset\\Contents.json",
                "Assets.xcassets\\icon_material_icon_build_18pt.imageset\\icon_material_icon_build_18pt_1x.png",
                "Assets.xcassets\\icon_material_icon_build_18pt.imageset\\icon_material_icon_build_18pt_2x.png",
                "Assets.xcassets\\icon_material_icon_build_18pt.imageset\\icon_material_icon_build_18pt_3x.png",
                "Assets.xcassets\\icon_material_icon_build_18pt.imageset\\Contents.json"
            });

            tester.AssertContainsText(WorkProjectFile, "<ImageAsset Include=\"Assets.xcassets\\icon_material_icon_build_18pt.imageset\\icon_material_icon_build_18pt_3x.png\">");
            tester.AssertContainsText(WorkProjectFile, "<ImageAsset Include=\"Assets.xcassets\\icon_material_icon_build_18pt.imageset\\Contents.json\">");
        }

        [Test]
        public void TestSingleColorCsProj()
        {
            IList<int> expectedSizes = new List<int> { 42 };
            string color = "#FF0000";

            IList<RequiredColor> colors = new List<RequiredColor> { new RequiredColor { ColorHexValue = color } };

            Program.Main(new[] { "resize", "--type", "ios", "--dstFolder", IconDir, "--srcFolder", SrcDataDir, "--prefix", "icon_", "--iconSize", "42", "--postfixSize", "false", "--csproj", WorkProjectFile, "--color", color });

            _iOSResultChecker.AssertIconsExistAndMatchSize(SrcDataDir, IconDir, expectedSizes, false, "icon_", colors);
            _iOSResultChecker.AssertIconCount(SrcDataDir, IconDir, expectedSizes, colors);
            ProjectFileTester tester = new ProjectFileTester(new IOSProjectFileUpdater());
            tester.AssertContainsIcon(WorkProjectFile, new List<string>
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

            tester.AssertContainsText(WorkProjectFile, "<ImageAsset Include=\"Assets.xcassets\\icon_material_icon_build.imageset\\icon_material_icon_build_3x.png\">");
            tester.AssertContainsText(WorkProjectFile, "<ImageAsset Include=\"Assets.xcassets\\icon_material_icon_build.imageset\\Contents.json\">");
        }

        [Test]
        public void TestMultipleColorCsProj()
        {
            IList<int> expectedSizes = new List<int> { 42 };
            string color = "{\"red\":\"#FF0000\",\"green\":\"#00FF00\"}";

            IList<RequiredColor> colors = new List<RequiredColor>()
            {
                new RequiredColor
                {
                    ColorName = "red",
                    ColorHexValue = "#FF0000"
                },
                new RequiredColor
                {
                    ColorName = "green",
                    ColorHexValue = "00FF00"
                }
            };

            Program.Main(new[] { "resize", "--type", "ios", "--dstFolder", IconDir, "--srcFolder", SrcDataDir, "--prefix", "icon_", "--iconSize", "42", "--postfixSize", "false", "--csproj", WorkProjectFile, "--color", color });

            _iOSResultChecker.AssertIconsExistAndMatchSize(SrcDataDir, IconDir, expectedSizes, false, "icon_", colors);
            _iOSResultChecker.AssertIconCount(SrcDataDir, IconDir, expectedSizes, colors);
            ProjectFileTester tester = new ProjectFileTester(new IOSProjectFileUpdater());
            tester.AssertContainsIcon(WorkProjectFile, new List<string>
            {
                "Assets.xcassets\\icon_material_icon_bug_red.imageset\\icon_material_icon_bug_red_1x.png",
                "Assets.xcassets\\icon_material_icon_bug_red.imageset\\icon_material_icon_bug_red_2x.png",
                "Assets.xcassets\\icon_material_icon_bug_red.imageset\\icon_material_icon_bug_red_3x.png",
                "Assets.xcassets\\icon_material_icon_bug_red.imageset\\Contents.json",
                "Assets.xcassets\\icon_material_icon_build_red.imageset\\icon_material_icon_build_red_1x.png",
                "Assets.xcassets\\icon_material_icon_build_red.imageset\\icon_material_icon_build_red_2x.png",
                "Assets.xcassets\\icon_material_icon_build_red.imageset\\icon_material_icon_build_red_3x.png",
                "Assets.xcassets\\icon_material_icon_build_red.imageset\\Contents.json",
                "Assets.xcassets\\icon_material_icon_bug_green.imageset\\icon_material_icon_bug_green_1x.png",
                "Assets.xcassets\\icon_material_icon_bug_green.imageset\\icon_material_icon_bug_green_2x.png",
                "Assets.xcassets\\icon_material_icon_bug_green.imageset\\icon_material_icon_bug_green_3x.png",
                "Assets.xcassets\\icon_material_icon_bug_green.imageset\\Contents.json",
                "Assets.xcassets\\icon_material_icon_build_green.imageset\\icon_material_icon_build_green_1x.png",
                "Assets.xcassets\\icon_material_icon_build_green.imageset\\icon_material_icon_build_green_2x.png",
                "Assets.xcassets\\icon_material_icon_build_green.imageset\\icon_material_icon_build_green_3x.png",
                "Assets.xcassets\\icon_material_icon_build_green.imageset\\Contents.json",
            });

            tester.AssertContainsText(WorkProjectFile, "<ImageAsset Include=\"Assets.xcassets\\icon_material_icon_build_red.imageset\\icon_material_icon_build_red_3x.png\">");
            tester.AssertContainsText(WorkProjectFile, "<ImageAsset Include=\"Assets.xcassets\\icon_material_icon_build_red.imageset\\Contents.json\">");
            tester.AssertContainsText(WorkProjectFile, "<ImageAsset Include=\"Assets.xcassets\\icon_material_icon_build_green.imageset\\icon_material_icon_build_green_3x.png\">");
            tester.AssertContainsText(WorkProjectFile, "<ImageAsset Include=\"Assets.xcassets\\icon_material_icon_build_green.imageset\\Contents.json\">");
        }

        [Test]
        public void TestResizeWithMultipleSizesCsprojMultipleColors()
        {
            IList<int> expectedSizes = new List<int> { 18, 28, 38 };
            string color = "{\"red\":\"#FF0000\",\"green\":\"#00FF00\"}";

            IList<RequiredColor> colors = new List<RequiredColor>()
            {
                new RequiredColor
                {
                    ColorName = "red",
                    ColorHexValue = "#FF0000"
                },
                new RequiredColor
                {
                    ColorName = "green",
                    ColorHexValue = "00FF00"
                }
            };

            Program.Main(new[] { "resize", "--type", "ios", "--dstFolder", IconDir, "--srcFolder", SrcDataDir, "--prefix", "icon_", "--iconSize", "18, 28, 38", "--postfixSize", "false", "--csproj", WorkProjectFile, "--color", color });

            _iOSResultChecker.AssertIconsExistAndMatchSize(SrcDataDir, IconDir, expectedSizes, true, "icon_", colors);
            _iOSResultChecker.AssertIconCount(SrcDataDir, IconDir, expectedSizes, colors);
            ProjectFileTester tester = new ProjectFileTester(new IOSProjectFileUpdater());
            tester.AssertContainsIcon(WorkProjectFile, new List<string>
            {
                "Assets.xcassets\\icon_material_icon_bug_red_18pt.imageset\\icon_material_icon_bug_red_18pt_1x.png",
                "Assets.xcassets\\icon_material_icon_bug_red_18pt.imageset\\icon_material_icon_bug_red_18pt_2x.png",
                "Assets.xcassets\\icon_material_icon_bug_red_18pt.imageset\\icon_material_icon_bug_red_18pt_3x.png",
                "Assets.xcassets\\icon_material_icon_bug_red_18pt.imageset\\Contents.json",
                "Assets.xcassets\\icon_material_icon_build_red_28pt.imageset\\icon_material_icon_build_red_28pt_1x.png",
                "Assets.xcassets\\icon_material_icon_build_red_28pt.imageset\\icon_material_icon_build_red_28pt_2x.png",
                "Assets.xcassets\\icon_material_icon_build_red_28pt.imageset\\icon_material_icon_build_red_28pt_3x.png",
                "Assets.xcassets\\icon_material_icon_build_red_28pt.imageset\\Contents.json",
                "Assets.xcassets\\icon_material_icon_bug_green_28pt.imageset\\icon_material_icon_bug_green_28pt_1x.png",
                "Assets.xcassets\\icon_material_icon_bug_green_28pt.imageset\\icon_material_icon_bug_green_28pt_2x.png",
                "Assets.xcassets\\icon_material_icon_bug_green_28pt.imageset\\icon_material_icon_bug_green_28pt_3x.png",
                "Assets.xcassets\\icon_material_icon_bug_green_28pt.imageset\\Contents.json",
                "Assets.xcassets\\icon_material_icon_build_green_28pt.imageset\\icon_material_icon_build_green_28pt_1x.png",
                "Assets.xcassets\\icon_material_icon_build_green_28pt.imageset\\icon_material_icon_build_green_28pt_2x.png",
                "Assets.xcassets\\icon_material_icon_build_green_28pt.imageset\\icon_material_icon_build_green_28pt_3x.png",
                "Assets.xcassets\\icon_material_icon_build_green_28pt.imageset\\Contents.json",
            });

            tester.AssertContainsText(WorkProjectFile, "<ImageAsset Include=\"Assets.xcassets\\icon_material_icon_build_red_28pt.imageset\\icon_material_icon_build_red_28pt_3x.png\">");
            tester.AssertContainsText(WorkProjectFile, "<ImageAsset Include=\"Assets.xcassets\\icon_material_icon_build_red_28pt.imageset\\Contents.json\">");
            tester.AssertContainsText(WorkProjectFile, "<ImageAsset Include=\"Assets.xcassets\\icon_material_icon_build_green_28pt.imageset\\icon_material_icon_build_green_28pt_3x.png\">");
            tester.AssertContainsText(WorkProjectFile, "<ImageAsset Include=\"Assets.xcassets\\icon_material_icon_build_green_28pt.imageset\\Contents.json\">");
        }
    }
}