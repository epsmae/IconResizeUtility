using System;
using System.IO;
using IconResizeUtility.TestInfrastructure;
using NUnit.Framework;
using SkiaSharp;

namespace IconResizeUtility.Service.Test
{
    public class ImageResizerTest
    {

        private string TestDataDirectory
        {
            get
            {
                return Path.Combine(TestContext.CurrentContext.TestDirectory, "TestData");
            }
        }


        private string WorkDirectory
        {
            get
            {
                return TestContext.CurrentContext.WorkDirectory;
            }
        }

        private string TestImage
        {
            get
            {
                return Path.Combine(TestDataDirectory, "test_image.png");
            }
        }

        private string TestIcon
        {
            get
            {
                return Path.Combine(TestDataDirectory, "Icons", "material_icon_addchar.png");
            }
        }

        private string DestinationImage
        {
            get
            {
                return Path.Combine(WorkDirectory, "test_image_resized.png");
            }
        }

        private string DestinationIcon
        {
            get
            {
                return Path.Combine(WorkDirectory, "material_icon_addchar_tinted.png");
            }
        }

        [SetUp]
        public void Setup()
        {
            if (File.Exists(DestinationImage))
            {
                File.Delete(DestinationImage);
            }
        }

        [Test]
        public void TestResize()
        {
            const int scaleFactor = 4;
            ImageResizer resizer = new ImageResizer();
            
            ImageInfo info = resizer.GetInfo(TestImage);

            int expectedWidth = info.Width / scaleFactor;
            int expectedHeight = info.Height / scaleFactor;
            
            resizer.Resize(TestImage, DestinationImage, expectedWidth, expectedHeight);

            ImageInfo resizedInfo = resizer.GetInfo(DestinationImage);

            Assert.AreEqual(expectedWidth, resizedInfo.Width);
            Assert.AreEqual(expectedHeight, resizedInfo.Height);
        }

        [Test]
        public void TestTint()
        {
            string hexColor = "33FFBB99";
            SKColor expectedColor = SKColor.Parse(hexColor);
            ImageResizer resizer = new ImageResizer();
            resizer.Resize(TestIcon, DestinationIcon, 48, 48, hexColor);

            SKColor actualColor = TestColorHelper.GetAverageColor(DestinationIcon);

            TestColorHelper.AssertSameColor(expectedColor, actualColor);
        }
    }
}