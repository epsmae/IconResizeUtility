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

        private string ProjectFile
        {
            get
            {
                return Path.Combine(TestContext.CurrentContext.TestDirectory, "TestData", "ProjectFiles", "ResizeUtility.App.iOS.csproj");
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

            _projectFileUpdater.LoadProjectFile(ProjectFile);
            _service.Resize(SrcDataDir, OutDir, false, string.Empty, expectedResolutions);
            _projectFileUpdater.Save(ProjectFile);

            _resultChecker.AssertIconsExistAndMatchSize(SrcDataDir, OutDir, expectedResolutions, false, string.Empty);
            _resultChecker.AssertIconCount(SrcDataDir, OutDir, expectedResolutions);
            _projectFileTester.AssertContainsIcon(ProjectFile, new List<string>
            {
                "material_icon_addchar_1x.png",
                "material_icon_addchar_2x.png",
                "material_icon_addchar_3x.png",
                "material_icon_alarm_1x.png",
                "material_icon_alarm_2x.png",
                "material_icon_alarm_3x.png"
            });
        }
    }
}
