using System;
using System.IO;
using NUnit.Framework;

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

        private string DestinationImage
        {
            get
            {
                return Path.Combine(WorkDirectory, "test_image_resized.png");
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
    }
}