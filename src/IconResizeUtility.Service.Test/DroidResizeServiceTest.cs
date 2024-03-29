﻿using System.Collections.Generic;
using System.IO;
using IconResizeUtility.Service.DataModel;
using IconResizeUtility.TestInfrastructure;
using NUnit.Framework;

namespace IconResizeUtility.Service.Test
{
    public class DroidResizeServiceTest
    {
        private AndroidResultChecker _resultChecker;
        private DroidResizeService _service;

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
            _resultChecker = new AndroidResultChecker(resizer, imageRenamer);
            _service = new DroidResizeService(resizer, imageRenamer, new ProjectUpdaterStub());


            if (Directory.Exists(OutDir))
            {
                Directory.Delete(OutDir, true);
            }
        }

        [Test]
        public void TestResize()
        {
            IList<int> expectedResolutions = DroidResizeService.DefaultRequiredSizes;
            string expectedPrefix = "ic_";
            const bool postFixSize = true;

            _service.Resize(SrcDataDir, OutDir, postFixSize, expectedPrefix, expectedResolutions);

            _resultChecker.AssertIconsExistAndMatchSize(SrcDataDir, OutDir, expectedResolutions, postFixSize, expectedPrefix);
            _resultChecker.AssertIconCount(SrcDataDir, OutDir, expectedResolutions);
        }

        [Test]
        public void TestResizeWithoutRename()
        {
            IList<int> expectedResolutions = DroidResizeService.DefaultRequiredSizes;
            string expectedPrefix = "ic_";
            const bool postFixSize = true;

            _service.Resize(SrcDataDir, OutDir, postFixSize, expectedPrefix, expectedResolutions, false);

            _resultChecker.AssertIconsExistAndMatchSize(SrcDataDir, OutDir, expectedResolutions, postFixSize, expectedPrefix, null, false);
            _resultChecker.AssertIconCount(SrcDataDir, OutDir, expectedResolutions);
        }

        [Test]
        public void TestResizeWithoutPrefix()
        {
            IList<int> expectedResolutions = DroidResizeService.DefaultRequiredSizes;
            string expectedPrefix = "";
            const bool postFixSize = true;

            _service.Resize(SrcDataDir, OutDir, postFixSize, expectedPrefix, expectedResolutions);

            _resultChecker.AssertIconsExistAndMatchSize(SrcDataDir, OutDir, expectedResolutions, postFixSize, expectedPrefix);
            _resultChecker.AssertIconCount(SrcDataDir, OutDir, expectedResolutions);
        }

        [Test]
        public void TestResizeWithoutPostfix()
        {
            IList<int> expectedResolutions = new List<int>{48};
            string expectedPrefix = "ic_";
            const bool postFixSize = false;

            _service.Resize(SrcDataDir, OutDir, postFixSize, expectedPrefix, expectedResolutions);

            _resultChecker.AssertIconsExistAndMatchSize(SrcDataDir, OutDir, expectedResolutions, postFixSize, expectedPrefix);
            _resultChecker.AssertIconCount(SrcDataDir, OutDir, expectedResolutions);
        }

        [Test]
        public void TestSingleColor()
        {
            IList<int> expectedResolutions = new List<int> { 48 };
            string expectedPrefix = "ic_";
            const bool postFixSize = false;

            IList<RequiredColor> colors = new List<RequiredColor>
            {
                new RequiredColor
                {
                    ColorHexValue = "#FF0000",
                    ColorName = "red"
                }
            };

            _service.Resize(SrcDataDir, OutDir, postFixSize, expectedPrefix, expectedResolutions, true, colors);

            _resultChecker.AssertIconsExistAndMatchSize(SrcDataDir, OutDir, expectedResolutions, postFixSize, expectedPrefix, colors);
            _resultChecker.AssertIconCount(SrcDataDir, OutDir, expectedResolutions);
        }

        [Test]
        public void TestMultipleColors()
        {
            IList<int> expectedResolutions = new List<int> { 48 };
            string expectedPrefix = "ic_";
            const bool postFixSize = false;

            IList<RequiredColor> colors = new List<RequiredColor>
            {
                new RequiredColor
                {
                    ColorHexValue = "#FF0000",
                    ColorName = "red"
                },
                new RequiredColor
                {
                    ColorHexValue = "#00FF00",
                    ColorName = "green"
                },
                new RequiredColor
                {
                    ColorHexValue = "#0000FF",
                    ColorName = "blue"
                },
                new RequiredColor
                {
                    ColorHexValue = "#000000",
                    ColorName = "black"
                }
            };

            _service.Resize(SrcDataDir, OutDir, postFixSize, expectedPrefix, expectedResolutions, true, colors);

            _resultChecker.AssertIconsExistAndMatchSize(SrcDataDir, OutDir, expectedResolutions, postFixSize, expectedPrefix, colors);
            _resultChecker.AssertIconCount(SrcDataDir, OutDir, expectedResolutions, colors);
        }
    }
}
