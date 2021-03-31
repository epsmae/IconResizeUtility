using System.Collections.Generic;
using System.IO;
using System.Linq;
using IconResizeUtility.Service;
using IconResizeUtility.Service.DataModel;
using IconResizeUtility.TestInfrastructure;
using Newtonsoft.Json;
using NUnit.Framework;

namespace IconResizeUtility.App.Test
{
    public class AndroidResizeTest
    {
        private AndroidResultChecker _androidResultChecker;

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
                return Path.Combine(TestContext.CurrentContext.TestDirectory, "TestProjectFile", "ResizeUtility.App.Android.csproj");
            }
        }

        private string WorkProjectFile
        {
            get
            {
                return Path.Combine(OutDir, "ResizeUtility.App.Android.csproj");
            }
        }

        private string OutDir
        {
            get
            {
                return Path.Combine(TestContext.CurrentContext.WorkDirectory, "out");
            }
        }


        private string OutIconDir
        {
            get
            {
                return Path.Combine(OutDir, "Icons");
            }
        }

        [SetUp]
        public void Setup()
        {
            ImageResizer resizer = new ImageResizer();
            ImageRenamer imageRenamer = new ImageRenamer();
            _androidResultChecker = new AndroidResultChecker(resizer, imageRenamer);

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
            IList<int> expectedSizes = DroidResizeService.DefaultRequiredSizes;

            Program.Main(new[] { "resize", "--type", "droid", "--dstFolder", OutIconDir, "--srcFolder", SrcDataDir, "--prefix", "icon_", "--postfixSize", "false" });

            _androidResultChecker.AssertIconsExistAndMatchSize(SrcDataDir, OutIconDir, expectedSizes, true, "icon_");
            _androidResultChecker.AssertIconCount(SrcDataDir, OutIconDir, expectedSizes);
        }

        [Test]
        public void TestResizeWithoutRename()
        {
            IList<int> expectedSizes = DroidResizeService.DefaultRequiredSizes;

            Program.Main(new[] { "resize", "--doNotRename", "--type", "droid", "--dstFolder", OutIconDir, "--srcFolder", SrcDataDir, "--prefix", "icon_", "--postfixSize", "false" });

            _androidResultChecker.AssertIconsExistAndMatchSize(SrcDataDir, OutIconDir, expectedSizes, true, "icon_", null, false);
            _androidResultChecker.AssertIconCount(SrcDataDir, OutIconDir, expectedSizes);
        }

        [Test]
        public void TestResizeUseCache()
        {
            IList<int> expectedSizes = DroidResizeService.DefaultRequiredSizes;

            Program.Main(new[] { "resize", "--useCache", "--type", "droid", "--dstFolder", OutIconDir, "--srcFolder", SrcDataDir, "--prefix", "icon_", "--postfixSize", "false" });

            _androidResultChecker.AssertIconsExistAndMatchSize(SrcDataDir, OutIconDir, expectedSizes, true, "icon_");
            _androidResultChecker.AssertIconCount(SrcDataDir, OutIconDir, expectedSizes);
        }

        [Test]
        public void TestResizeWithSingleSize()
        {
            IList<int> expectedSizes = new List<int> {42};

            Program.Main(new[] { "resize", "--type", "droid", "--dstFolder", OutIconDir, "--srcFolder", SrcDataDir, "--prefix", "icon_", "--iconSize", "42", "--postfixSize", "false" });

            _androidResultChecker.AssertIconsExistAndMatchSize(SrcDataDir, OutIconDir, expectedSizes, false, "icon_");
            _androidResultChecker.AssertIconCount(SrcDataDir, OutIconDir, expectedSizes);
        }

        [Test]
        public void TestResizeWithSingleSizePostfix()
        {
            IList<int> expectedSizes = new List<int> { 42 };

            Program.Main(new[] { "resize", "--type", "droid", "--dstFolder", OutIconDir, "--srcFolder", SrcDataDir, "--iconSize", "42", "--postfixSize", "true" });

            _androidResultChecker.AssertIconsExistAndMatchSize(SrcDataDir, OutIconDir, expectedSizes, true, "");
            _androidResultChecker.AssertIconCount(SrcDataDir, OutIconDir, expectedSizes);
        }

        [Test]
        public void TestResizeWithMultipleSizes()
        {
            IList<int> expectedSizes = new List<int> { 18, 28, 38 };

            Program.Main(new[] { "resize", "--type", "droid", "--dstFolder", OutIconDir, "--srcFolder", SrcDataDir, "--prefix", "icon_", "--iconSize", "18,28, 38", "--postfixSize", "false" });

            _androidResultChecker.AssertIconsExistAndMatchSize(SrcDataDir, OutIconDir, expectedSizes, true, "icon_");
            _androidResultChecker.AssertIconCount(SrcDataDir, OutIconDir, expectedSizes);
        }

        [Test]
        public void TestCsprojUpdate()
        {
            IList<int> expectedSizes = new List<int> { 42 };

            Program.Main(new[] { "resize", "--type", "droid", "--dstFolder", OutIconDir, "--srcFolder", SrcDataDir, "--iconSize", "42", "--csproj", WorkProjectFile});

            _androidResultChecker.AssertIconsExistAndMatchSize(SrcDataDir, OutIconDir, expectedSizes, false, string.Empty);
            _androidResultChecker.AssertIconCount(SrcDataDir, OutIconDir, expectedSizes);

            ProjectFileTester tester = new ProjectFileTester(new DroidProjectFileUpdater());
            tester.AssertContainsIcon(WorkProjectFile, new List<string>
            {
                "material_icon_bug.png",
                "material_icon_build.png"
            });
            tester.AssertContainsText(WorkProjectFile, "<AndroidResource Include=\"Resources\\drawable-xxhdpi\\material_icon_bug.png\" />");
        }

        [Test]
        public void TestSingleColorCsProj()
        {
            IList<int> expectedSizes = new List<int> { 42 };
            string color = "#FF0000";

            IList<RequiredColor> colors = new List<RequiredColor> {new RequiredColor {ColorHexValue = color}};

            Program.Main(new[] { "resize", "--type", "droid", "--dstFolder", OutIconDir, "--srcFolder", SrcDataDir, "--iconSize", "42", "--csproj", WorkProjectFile, "--color", color});

            _androidResultChecker.AssertIconsExistAndMatchSize(SrcDataDir, OutIconDir, expectedSizes, false, string.Empty, colors);
            _androidResultChecker.AssertIconCount(SrcDataDir, OutIconDir, expectedSizes, colors);

            ProjectFileTester tester = new ProjectFileTester(new DroidProjectFileUpdater());
            tester.AssertContainsIcon(WorkProjectFile, new List<string>
            {
                "material_icon_bug.png",
                "material_icon_build.png"
            });
            tester.AssertContainsText(WorkProjectFile, "<AndroidResource Include=\"Resources\\drawable-xxhdpi\\material_icon_bug.png\" />");
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

            Program.Main(new[] { "resize", "--type", "droid", "--dstFolder", OutIconDir, "--srcFolder", SrcDataDir, "--iconSize", "42", "--csproj", WorkProjectFile, "--color", color });

            _androidResultChecker.AssertIconsExistAndMatchSize(SrcDataDir, OutIconDir, expectedSizes, false, string.Empty, colors);
            _androidResultChecker.AssertIconCount(SrcDataDir, OutIconDir, expectedSizes, colors);

            ProjectFileTester tester = new ProjectFileTester(new DroidProjectFileUpdater());
            tester.AssertContainsIcon(WorkProjectFile, new List<string>
            {
                "material_icon_bug_red.png",
                "material_icon_bug_green.png",
                "material_icon_build_red.png",
                "material_icon_build_green.png"
            });
            tester.AssertContainsText(WorkProjectFile, "<AndroidResource Include=\"Resources\\drawable-xxhdpi\\material_icon_bug_green.png\" />");
            tester.AssertContainsText(WorkProjectFile, "<AndroidResource Include=\"Resources\\drawable-xxhdpi\\material_icon_bug_red.png\" />");
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

            Program.Main(new[] { "resize", "--type", "droid", "--dstFolder", OutIconDir, "--srcFolder", SrcDataDir, "--iconSize", "18,28, 38", "--csproj", WorkProjectFile, "--color", color });

            _androidResultChecker.AssertIconCount(SrcDataDir, OutIconDir, expectedSizes, colors);

            ProjectFileTester tester = new ProjectFileTester(new DroidProjectFileUpdater());
            tester.AssertContainsIcon(WorkProjectFile, new List<string>
            {
                "material_icon_bug_red_18.png",
                "material_icon_bug_red_28.png",
                "material_icon_bug_red_38.png",
                "material_icon_bug_green_18.png",
                "material_icon_bug_green_28.png",
                "material_icon_bug_green_38.png",
                "material_icon_build_red_18.png",
                "material_icon_build_red_28.png",
                "material_icon_build_red_38.png",
                "material_icon_build_green_18.png",
                "material_icon_build_green_28.png",
                "material_icon_build_green_38.png"
            });
            tester.AssertContainsText(WorkProjectFile, "<AndroidResource Include=\"Resources\\drawable-xxhdpi\\material_icon_bug_green_18.png\" />");
            tester.AssertContainsText(WorkProjectFile, "<AndroidResource Include=\"Resources\\drawable-xxhdpi\\material_icon_bug_red_18.png\" />");
        }
    }
}