﻿using System.Collections.Generic;
using System.IO;
using System.Linq;
using IconResizeUtility.Service;
using IconResizeUtility.Service.DataModel;
using NUnit.Framework;
using SkiaSharp;

namespace IconResizeUtility.TestInfrastructure
{
    public class IOSResultChecker
    {
        private readonly ImageResizer _imageResizer;
        private readonly ImageRenamer _imageRenamer;

        public IOSResultChecker(ImageResizer resizer, ImageRenamer imageRenamer)
        {
            _imageResizer = resizer;
            _imageRenamer = imageRenamer;
        }

        public void AssertIconsExistAndMatchSize(string testDataDir, string outDir, IList<int> expectedResolutions, bool postfixSize, string expectedPrefix, IList<RequiredColor> requiredColors = null, bool convertToValidIconName = true)
        {
            int cnt = 0;
            DirectoryInfo srcFolderInfo = new DirectoryInfo(testDataDir);
            foreach (FileInfo file in srcFolderInfo.EnumerateFiles())
            {
                cnt++;

                foreach (int expectedResolution in expectedResolutions)
                {
                    foreach (string scaleFactor in IOSImageResizeService.ResNameAssociation.Keys.ToArray())
                    {
                        int expectedSize = (int)(expectedResolution * IOSImageResizeService.ResNameAssociation[scaleFactor]);

                        AssertContainsIconSize(outDir, scaleFactor, file.Name, postfixSize, expectedPrefix, expectedResolution, expectedSize, convertToValidIconName, requiredColors);
                    }
                }
            }

            if (cnt == 0)
            {
                Assert.Fail("No images found in source scaleFactor");
            }
        }

        public void AssertIconCount(string srcDataDir, string outDir, IList<int> expectedResolutions,
            IList<RequiredColor> requiredColors = null)
        {
            int srcFileCount = Directory.EnumerateFiles(srcDataDir).Count();
            int iconCount = Directory.EnumerateFiles(outDir, "*", SearchOption.AllDirectories).Count();
            // json file also counts
            int expectedCount;

            if (requiredColors != null && requiredColors.Any())
            {
                expectedCount = srcFileCount * expectedResolutions.Count * (IOSImageResizeService.ResNameAssociation.Count + 1) * requiredColors.Count;
            }
            else
            {
                expectedCount = srcFileCount * expectedResolutions.Count * (IOSImageResizeService.ResNameAssociation.Count + 1);
            }
            
            Assert.AreEqual(expectedCount, iconCount);
        }

        private void AssertContainsIconSize(string outputDirectory, string scaleFactor, string iconName, bool postfixSize, string prefix, int size, int expectedSize, bool convertToValidIconName, IList<RequiredColor> requiredColors = null)
        {
            string adjustedName = GetIconName(iconName, prefix, convertToValidIconName);
            
            if (requiredColors == null || requiredColors.Count == 1)
            {
                if (postfixSize)
                {
                    adjustedName = _imageRenamer.AddPostfix(adjustedName, $"_{size}pt");
                }

                string folderPath = Path.Combine(outputDirectory, $"{Path.GetFileNameWithoutExtension(adjustedName)}.imageset");

                adjustedName = _imageRenamer.AddPostfix(adjustedName, $"_{scaleFactor}");
                AssertContainsIconSize(expectedSize, folderPath, adjustedName);

                if (requiredColors != null && requiredColors.Any())
                {
                    AssertContainsIconColor(folderPath, adjustedName, requiredColors.First());
                }
            }
            else
            {
                foreach (RequiredColor color in requiredColors)
                {
                    string colorIconName = _imageRenamer.AddPostfix(adjustedName, $"_{color.ColorName}");

                    if (postfixSize)
                    {
                        colorIconName = _imageRenamer.AddPostfix(colorIconName, $"_{size}pt");
                    }

                    string folderPath = Path.Combine(outputDirectory, $"{Path.GetFileNameWithoutExtension(colorIconName)}.imageset");

                    colorIconName = _imageRenamer.AddPostfix(colorIconName, $"_{scaleFactor}");
                    
                    AssertContainsIconSize(expectedSize, folderPath, colorIconName);
                    AssertContainsIconColor(folderPath, colorIconName, color);
                }
            }
        }

        private void AssertContainsIconColor(string folderPath, string iconName, RequiredColor expectedColor)
        {
            string fullImagePath = Path.Combine(folderPath, iconName);
            SKColor actualColor = TestColorHelper.GetAverageColor(fullImagePath);

            TestColorHelper.AssertSameColor(SKColor.Parse(expectedColor.ColorHexValue), actualColor);
        }

        private void AssertContainsIconSize(int expectedSize, string folderPath, string iconName)
        {
            string fullImagePath = Path.Combine(folderPath, iconName);

            Assert.True(File.Exists(fullImagePath));

            ImageInfo info = _imageResizer.GetInfo(fullImagePath);

            Assert.AreEqual(expectedSize, info.Height);
            Assert.AreEqual(expectedSize, info.Width);

            string jsonContentsPath = Path.Combine(folderPath, "Contents.json");

            string jsonContent = File.ReadAllText(jsonContentsPath);
            Assert.True(jsonContent.Contains(iconName));
        }

        private string GetIconName(string iconName, string prefix, bool convertToValidIconName)
        {
            string baseName = convertToValidIconName ? _imageRenamer.ConvertToValidIconName(iconName): iconName;
            
            if (!string.IsNullOrEmpty(prefix))
            {
                baseName = _imageRenamer.AddPrefix(baseName, prefix);
            }

            return baseName;
        }
    }
}
