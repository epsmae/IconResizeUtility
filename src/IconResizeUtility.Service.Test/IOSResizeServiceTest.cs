using System.Collections.Generic;
using System.IO;
using IconResizeUtility.TestInfrastructure;
using NUnit.Framework;

namespace IconResizeUtility.Service.Test
{
    public class IOSResizeServiceTest
    {
        private IOSResultChecker _resultChecker;
        private IOSImageResizeService _service;

        private string SrcDataDir
        {
            get
            {
                return Path.Combine(TestContext.CurrentContext.TestDirectory, "TestData", "Icons");
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
            _service = new IOSImageResizeService(resizer, imageRenamer);


            if (Directory.Exists(OutDir))
            {
                Directory.Delete(OutDir, true);
            }
        }

        [Test]
        public void TestResize()
        {
            IList<int> expectedResolutions = IOSImageResizeService.DefaultRequiredSizes;
            string expectedPrefix = "ic_";
            const bool postFixSize = true;

            _service.Resize(SrcDataDir, OutDir, postFixSize, expectedPrefix, expectedResolutions);

            _resultChecker.AssertIconsExistAndMatchSize(SrcDataDir, OutDir, expectedResolutions, postFixSize, expectedPrefix);
            _resultChecker.AssertIconCount(SrcDataDir, OutDir, expectedResolutions);
        }

        [Test]
        public void TestResizeWithoutPrefix()
        {
            IList<int> expectedResolutions = AndroidResizeService.DefaultRequiredSizes;
            string expectedPrefix = "";
            const bool postFixSize = true;

            _service.Resize(SrcDataDir, OutDir, postFixSize, expectedPrefix, expectedResolutions);

            _resultChecker.AssertIconsExistAndMatchSize(SrcDataDir, OutDir, expectedResolutions, postFixSize, expectedPrefix);
            _resultChecker.AssertIconCount(SrcDataDir, OutDir, expectedResolutions);
        }

        [Test]
        public void TestResizeWithoutPostfix()
        {
            IList<int> expectedResolutions = new List<int> { 48 };
            string expectedPrefix = "ic_";
            const bool postFixSize = false;

            _service.Resize(SrcDataDir, OutDir, postFixSize, expectedPrefix, expectedResolutions);

            _resultChecker.AssertIconsExistAndMatchSize(SrcDataDir, OutDir, expectedResolutions, postFixSize, expectedPrefix);
            _resultChecker.AssertIconCount(SrcDataDir, OutDir, expectedResolutions);
        }
    }
}
