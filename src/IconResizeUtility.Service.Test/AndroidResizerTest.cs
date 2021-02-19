using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace IconResizeUtility.Service.Test
{
    public class AndroidResizerTest
    {
        private ImageResizer _imageResizer;
        private RenameUtility _renameUtility;

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
            _imageResizer = new ImageResizer();
            _renameUtility = new RenameUtility();

            if (Directory.Exists(OutDir))
            {
                Directory.Delete(OutDir, true);
            }
        }


        [Test]
        public void TestResize()
        {
            AndroidResizeService service = new AndroidResizeService(_imageResizer, _renameUtility);

            IList<int> expectedResolutions = AndroidResizeService.DefaultRequiredSizes;
            string expectedPrefix = "ic_";
            const bool postFixSize = true;

            service.Resize(SrcDataDir, OutDir, postFixSize, expectedPrefix, expectedResolutions);

            AssertIconsExistAndMatchSize(SrcDataDir, OutDir, expectedResolutions, postFixSize, expectedPrefix);
            AssertIconCount(SrcDataDir, OutDir, expectedResolutions);
        }

        [Test]
        public void TestResizeWithoutPrefix()
        {
            AndroidResizeService service = new AndroidResizeService(_imageResizer, _renameUtility);

            IList<int> expectedResolutions = AndroidResizeService.DefaultRequiredSizes;
            string expectedPrefix = "";
            const bool postFixSize = true;

            service.Resize(SrcDataDir, OutDir, postFixSize, expectedPrefix, expectedResolutions);

            AssertIconsExistAndMatchSize(SrcDataDir, OutDir, expectedResolutions, postFixSize, expectedPrefix);
            AssertIconCount(SrcDataDir, OutDir, expectedResolutions);
        }

        [Test]
        public void TestResizeWithoutPostfix()
        {
            AndroidResizeService service = new AndroidResizeService(_imageResizer, _renameUtility);

            IList<int> expectedResolutions = new List<int>{48};
            string expectedPrefix = "ic_";
            const bool postFixSize = false;

            service.Resize(SrcDataDir, OutDir, postFixSize, expectedPrefix, expectedResolutions);

            AssertIconsExistAndMatchSize(SrcDataDir, OutDir, expectedResolutions, postFixSize, expectedPrefix);
            AssertIconCount(SrcDataDir, OutDir, expectedResolutions);
        }
        
        private void AssertIconsExistAndMatchSize(string testDataDir, string outDir, IList<int> expectedResolutions, bool postfixSize, string expectedPrefix)
        {
            DirectoryInfo srcFolderInfo = new DirectoryInfo(testDataDir);
            foreach (FileInfo file in srcFolderInfo.EnumerateFiles())
            {
                foreach (string folder in AndroidResizeService.ResolutionFolders.Keys.ToArray())
                {
                    foreach (int expectedResolution in expectedResolutions)
                    {
                        int expectedSize = (int) (expectedResolution * AndroidResizeService.ResolutionFolders[folder]);
                        AssertContainsIconSize(outDir, folder, file.Name, postfixSize, expectedPrefix, expectedResolution,
                            expectedSize);
                    }
                }
            }
        }

        private void AssertIconCount(string srcDataDir, string outDir, IList<int> expectedResolutions)
        {
            int srcFileCount = Directory.EnumerateFiles(srcDataDir).Count();
            int iconCount = Directory.EnumerateFiles(outDir, "*", SearchOption.AllDirectories).Count();
            int expectedCount = srcFileCount * expectedResolutions.Count * AndroidResizeService.ResolutionFolders.Count;
            Assert.AreEqual(expectedCount, iconCount);
        }

        private void AssertContainsIconSize(string outputDirectory, string folder, string iconName, bool postfixSize, string suffix, int size, int expectedSize)
        {
            string adjustedName = GetIconName(iconName, postfixSize, suffix, size);

            string fullImagePath = Path.Combine(outputDirectory, folder, adjustedName);

            Assert.True(File.Exists(fullImagePath));

            ImageInfo info = _imageResizer.GetInfo(fullImagePath);

            Assert.AreEqual(expectedSize, info.Height);
            Assert.AreEqual(expectedSize, info.Width);
        }

        private string GetIconName(string iconName, bool postfixSize, string suffix, int size)
        {

            string baseName = _renameUtility.ConvertToValidIconName(iconName);

            if (postfixSize)
            {
                baseName = _renameUtility.AddPostfix(baseName, $"_{size}");
            }

            if (!string.IsNullOrEmpty(suffix))
            {
                baseName = _renameUtility.AddSuffix(baseName, suffix);
            }

            return baseName;
        }
    }
}
