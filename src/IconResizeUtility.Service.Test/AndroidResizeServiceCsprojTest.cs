using System.Collections.Generic;
using System.IO;
using IconResizeUtility.TestInfrastructure;
using NUnit.Framework;

namespace IconResizeUtility.Service.Test
{
    public class AndroidResizeServiceCsprojTest
    {
        private AndroidResultChecker _resultChecker;
        private DroidResizeService _service;
        private DroidProjectFileUpdater _csprojFileUpdater;
        private ProjectFileTester _csprojFileTester;

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
                return Path.Combine(TestContext.CurrentContext.TestDirectory, "TestData", "ProjectFiles", "ResizeUtility.App.Android.csproj");
            }
        }

        private string OutProjectFile
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
            _resultChecker = new AndroidResultChecker(resizer, imageRenamer);
            _csprojFileUpdater = new DroidProjectFileUpdater();
            _service = new DroidResizeService(resizer, imageRenamer, _csprojFileUpdater);
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

            _csprojFileUpdater.LoadProjectFile(SrcProjectFile);
            _service.Resize(SrcDataDir, OutIconDir, false, string.Empty, expectedResolutions);
            _csprojFileUpdater.Save(OutProjectFile);

            _resultChecker.AssertIconsExistAndMatchSize(SrcDataDir, OutIconDir, expectedResolutions, false, string.Empty);
            _resultChecker.AssertIconCount(SrcDataDir, OutIconDir, expectedResolutions);
            _csprojFileTester.AssertContainsIcon(OutProjectFile, new List<string>{ "material_icon_addchar.png", "material_icon_alarm.png" });
            _csprojFileTester.AssertContainsText(OutProjectFile, "<AndroidResource Include=\"Resources\\drawable-xxhdpi\\material_icon_addchar.png\" />");
        }
    }
}
