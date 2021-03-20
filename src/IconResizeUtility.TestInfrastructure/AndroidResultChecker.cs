using System.Collections.Generic;
using System.IO;
using System.Linq;
using IconResizeUtility.Service;
using IconResizeUtility.Service.DataModel;
using NUnit.Framework;
using SkiaSharp;

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

        public void AssertIconsExistAndMatchSize(string testDataDir, string outDir, IList<int> expectedResolutions, bool postfixSize, string expectedPrefix, IList<RequiredColor> requiredColors = null)
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

                        if (requiredColors == null || requiredColors.Count <= 1)
                        {
                            string adjustedName = GetIconName(file.Name, postfixSize, expectedPrefix, expectedResolution);

                            string fullImagePath = Path.Combine(outDir, folder, adjustedName);
                            Assert.True(File.Exists(fullImagePath));

                            AssertContainsIconSize(fullImagePath, expectedSize);

                            if (requiredColors?.Count > 0)
                            {
                                AssertIconColor(fullImagePath, requiredColors.First());
                            }
                        }
                        else
                        {
                            foreach (RequiredColor color in requiredColors)
                            {
                                string adjustedName = GetIconName(file.Name, postfixSize, expectedPrefix, expectedResolution, color.ColorName);

                                string fullImagePath = Path.Combine(outDir, folder, adjustedName);
                                Assert.True(File.Exists(fullImagePath));

                                AssertContainsIconSize(fullImagePath, expectedSize);
                                AssertIconColor(fullImagePath, color);
                            }
                        }

                    }
                }
            }

            if (cnt == 0)
            {
                Assert.Fail("No images found in source folder");
            }
        }


        public void AssertIconCount(string srcDataDir, string outDir, IList<int> expectedResolutions, IList<RequiredColor> expectedColors = null)
        {
            int srcFileCount = Directory.EnumerateFiles(srcDataDir).Count();
            int iconCount = Directory.EnumerateFiles(outDir, "*", SearchOption.AllDirectories).Count();

            int expectedCount;
            if (expectedColors != null && expectedColors.Count > 0)
            {
                expectedCount = srcFileCount * expectedResolutions.Count * AndroidResizeService.ResFolderAssociation.Count * expectedColors.Count;
            }
            else
            {
                expectedCount = srcFileCount * expectedResolutions.Count * AndroidResizeService.ResFolderAssociation.Count;
            }

            Assert.AreEqual(expectedCount, iconCount);
        }

        private void AssertContainsIconSize(string fullImagePath, int expectedSize)
        {
            ImageInfo info = _imageResizer.GetInfo(fullImagePath);

            Assert.AreEqual(expectedSize, info.Height);
            Assert.AreEqual(expectedSize, info.Width);
        }


        private void AssertIconColor(string fullImagePath, RequiredColor requiredColor)
        {

            SKColor actualColor = TestColorHelper.GetAverageColor(fullImagePath);
            
            TestColorHelper.AssertSameColor(SKColor.Parse(requiredColor.ColorHexValue), actualColor);
        }


        private string GetIconName(string iconName, bool postfixSize, string prefix, int size, string colorName = null)
        {

            string baseName = _imageRenamer.ConvertToValidIconName(iconName);

            if (postfixSize)
            {
                baseName = _imageRenamer.AddPostfix(baseName, $"_{size}");
            }

            if (!string.IsNullOrEmpty(colorName))
            {
                baseName = _imageRenamer.AddPostfix(baseName, $"_{colorName}");
            }

            if (!string.IsNullOrEmpty(prefix))
            {
                baseName = _imageRenamer.AddPrefix(baseName, prefix);
            }

            return baseName;
        }
    }
}
