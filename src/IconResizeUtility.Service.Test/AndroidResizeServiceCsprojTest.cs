using System.Collections.Generic;
using System.IO;
using IconResizeUtility.TestInfrastructure;
using NUnit.Framework;

namespace IconResizeUtility.Service.Test
{
    public class AndroidResizeServiceCsprojTest
    {
        private AndroidResultChecker _resultChecker;
        private AndroidResizeService _service;
        private DroidProjectFileUpdater _csprojFileUpdater;
        private ProjectFileTester _csprojFileTester;

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
                return Path.Combine(TestContext.CurrentContext.TestDirectory, "TestData", "ProjectFiles", "ResizeUtility.App.Android.csproj");
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
            _resultChecker = new AndroidResultChecker(resizer, imageRenamer);
            _csprojFileUpdater = new DroidProjectFileUpdater();
            _service = new AndroidResizeService(resizer, imageRenamer, _csprojFileUpdater);
            _csprojFileTester = new ProjectFileTester(_csprojFileUpdater);

            if (Directory.Exists(OutDir))
            {
                Directory.Delete(OutDir, true);
            }
        }

        [Test]
        public void TestResize()
        {
            IList<int> expectedResolutions = new List<int>(){48};

            _csprojFileUpdater.LoadProjectFile(ProjectFile);
            _service.Resize(SrcDataDir, OutDir, false, string.Empty, expectedResolutions);
            _csprojFileUpdater.Save(ProjectFile);

            _resultChecker.AssertIconsExistAndMatchSize(SrcDataDir, OutDir, expectedResolutions, false, string.Empty);
            _resultChecker.AssertIconCount(SrcDataDir, OutDir, expectedResolutions);
            _csprojFileTester.AssertContainsIcon(ProjectFile, new List<string>{ "material_icon_addchar.png", "material_icon_alarm.png" });
        }
    }
}
