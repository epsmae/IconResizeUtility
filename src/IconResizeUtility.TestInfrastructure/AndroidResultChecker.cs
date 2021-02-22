using System.Collections.Generic;
using System.IO;
using System.Linq;
using IconResizeUtility.Service;
using NUnit.Framework;

namespace IconResizeUtility.TestInfrastructure
{
    public class AndroidResultChecker
    {
        private readonly ImageResizer _imageResizer;
        private readonly ImageRenamer _imageRenamer;

        public AndroidResultChecker(ImageResizer resizer, ImageRenamer imageRenamer)
        {
            _imageResizer = resizer;
            _imageRenamer = imageRenamer;
        }

        public void AssertIconsExistAndMatchSize(string testDataDir, string outDir, IList<int> expectedResolutions, bool postfixSize, string expectedPrefix)
        {
            int cnt = 0;
            DirectoryInfo srcFolderInfo = new DirectoryInfo(testDataDir);
            foreach (FileInfo file in srcFolderInfo.EnumerateFiles())
            {
                cnt++;
                foreach (string folder in AndroidResizeService.ResFolderAssociation.Keys.ToArray())
                {
                    foreach (int expectedResolution in expectedResolutions)
                    {
                        int expectedSize = (int)(expectedResolution * AndroidResizeService.ResFolderAssociation[folder]);
                        AssertContainsIconSize(outDir, folder, file.Name, postfixSize, expectedPrefix, expectedResolution,
                            expectedSize);
                    }
                }
            }

            if (cnt == 0)
            {
                Assert.Fail("No images found in source folder");
            }
        }

        public void AssertIconCount(string srcDataDir, string outDir, IList<int> expectedResolutions)
        {
            int srcFileCount = Directory.EnumerateFiles(srcDataDir).Count();
            int iconCount = Directory.EnumerateFiles(outDir, "*", SearchOption.AllDirectories).Count();
            int expectedCount = srcFileCount * expectedResolutions.Count * AndroidResizeService.ResFolderAssociation.Count;
            Assert.AreEqual(expectedCount, iconCount);
        }

        private void AssertContainsIconSize(string outputDirectory, string folder, string iconName, bool postfixSize, string prefix, int size, int expectedSize)
        {
            string adjustedName = GetIconName(iconName, postfixSize, prefix, size);

            string fullImagePath = Path.Combine(outputDirectory, folder, adjustedName);

            Assert.True(File.Exists(fullImagePath));

            ImageInfo info = _imageResizer.GetInfo(fullImagePath);

            Assert.AreEqual(expectedSize, info.Height);
            Assert.AreEqual(expectedSize, info.Width);
        }

        private string GetIconName(string iconName, bool postfixSize, string prefix, int size)
        {

            string baseName = _imageRenamer.ConvertToValidIconName(iconName);

            if (postfixSize)
            {
                baseName = _imageRenamer.AddPostfix(baseName, $"_{size}");
            }

            if (!string.IsNullOrEmpty(prefix))
            {
                baseName = _imageRenamer.AddPrefix(baseName, prefix);
            }

            return baseName;
        }
    }
}
