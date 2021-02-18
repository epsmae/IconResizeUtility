using NUnit.Framework;

namespace IconResizeUtility.Service.Test
{
    public class ImageInfoExtensionsTest
    {
        [Test]
        public void TestScaleDownSameSize()
        {
            const int expectedSize = 48;
            ImageInfo info = new ImageInfo{Height = 96, Width = 96};
            info.ScaleByWidth(expectedSize);
            Assert.AreEqual(expectedSize, info.Width);
            Assert.AreEqual(expectedSize, info.Height);
        }

        [Test]
        public void TestScaleUpSameSize()
        {
            const int expectedSize = 192;
            ImageInfo info = new ImageInfo { Height = 96, Width = 96 };
            info.ScaleByWidth(expectedSize);
            Assert.AreEqual(expectedSize, info.Width);
            Assert.AreEqual(expectedSize, info.Height);
        }

        [Test]
        public void TestScaleDownLongerWidth()
        {
            const int expectedSize = 48;
            ImageInfo info = new ImageInfo { Height = 48, Width = 96 };
            info.ScaleByWidth(expectedSize);
            Assert.AreEqual(expectedSize, info.Width);
            Assert.AreEqual(expectedSize / 2, info.Height);
        }

        [Test]
        public void TestScaleUpLongerWidth()
        {
            const int expectedSize = 96;
            ImageInfo info = new ImageInfo { Height = 96, Width = 48 };
            info.ScaleByWidth(expectedSize);
            Assert.AreEqual(expectedSize, info.Width);
            Assert.AreEqual(expectedSize * 2, info.Height);
        }
    }
}
