using System.Collections.Generic;
using System.IO;
using IconResizeUtility.TestInfrastructure;
using NUnit.Framework;

namespace IconResizeUtility.Service.Test
{
    public class IOSResizeServiceCsprojTest
    {
        private IOSResultChecker _resultChecker;
        private IOSImageResizeService _service;
        private IOSProjectFileUpdater _projectFileUpdater;
        private ProjectFileTester _projectFileTester;

        private string SrcDataDir
        {
            get
            {
                return Path.Combine(TestContext.CurrentContext.TestDirectory, "TestData", "Icons");
            }
        }

        private string SrcProjectFile
        {
            get
            {
                return Path.Combine(TestContext.CurrentContext.TestDirectory, "TestData", "ProjectFiles", "ResizeUtility.App.iOS.csproj");
            }
        }

        private string OutProjectFile
        {
            get
            {
                return Path.Combine(OutDir, "ResizeUtility.App.iOS.csproj.out");
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
            _resultChecker = new IOSResultChecker(resizer, imageRenamer);
            _projectFileUpdater = new IOSProjectFileUpdater();
            _service = new IOSImageResizeService(resizer, imageRenamer, _projectFileUpdater);
            _projectFileTester = new ProjectFileTester(_projectFileUpdater);


            if (Directory.Exists(OutDir))
            {
                Directory.Delete(OutDir, true);
            }
        }

        [Test]
        public void TestResize()
        {
            IList<int> expectedResolutions = new List<int>{48};

            _projectFileUpdater.LoadProjectFile(SrcProjectFile);
            _service.Resize(SrcDataDir, OutIconDir, false, string.Empty, expectedResolutions);
            _projectFileUpdater.Save(OutProjectFile);

            _resultChecker.AssertIconsExistAndMatchSize(SrcDataDir, OutIconDir, expectedResolutions, false, string.Empty);
            _resultChecker.AssertIconCount(SrcDataDir, OutIconDir, expectedResolutions);
            _projectFileTester.AssertContainsIcon(OutProjectFile, new List<string>
            {
                "Assets.xcassets\\material_icon_addchar.imageset\\material_icon_addchar_1x.png",
                "Assets.xcassets\\material_icon_addchar.imageset\\material_icon_addchar_2x.png",
                "Assets.xcassets\\material_icon_addchar.imageset\\material_icon_addchar_3x.png",
                "Assets.xcassets\\material_icon_addchar.imageset\\Contents.json",
                "Assets.xcassets\\material_icon_alarm.imageset\\material_icon_alarm_1x.png",
                "Assets.xcassets\\material_icon_alarm.imageset\\material_icon_alarm_2x.png",
                "Assets.xcassets\\material_icon_alarm.imageset\\material_icon_alarm_3x.png",
                "Assets.xcassets\\material_icon_alarm.imageset\\Contents.json"
            });

            _projectFileTester.AssertContainsText(OutProjectFile, "<ImageAsset Include=\"Assets.xcassets\\material_icon_alarm.imageset\\material_icon_alarm_3x.png\">");
            _projectFileTester.AssertContainsText(OutProjectFile, "<ImageAsset Include=\"Assets.xcassets\\material_icon_alarm.imageset\\Contents.json\">");

        }
    }
}
