using NUnit.Framework;

namespace IconResizeUtility.Service.Test
{
    public class ImageRenamerTest
    {
        private ImageRenamer _imageRenamer;

        [SetUp]
        public void Setup()
        {
            _imageRenamer = new ImageRenamer();
        }

        [Test]
        public void TestSaveIconName()
        {
            string unsafeIconName = "$-.,MyIcon 12.PNG";
            string expectedstring = "____myicon_12.png";

            string result = _imageRenamer.ConvertToValidIconName(unsafeIconName);
            Assert.AreEqual(expectedstring, result);
        }

        [Test]
        public void TestAddPostFix()
        {
            string unsafeIconName = "$-.,MyIcon 12.PNG";
            string expectedstring = "____myicon_12.png";

            string result = _imageRenamer.ConvertToValidIconName(unsafeIconName);
            Assert.AreEqual(expectedstring, result);
        }

        [Test]
        public void TestAddPrefix()
        {
            string prefix = "ic_";
            string iconName = "icon.png";
            string expectedstring = $"{prefix}{iconName}";
            
            string result = _imageRenamer.AddPrefix(iconName, prefix);
            Assert.AreEqual(expectedstring, result);
        }

        [Test]
        public void TestAddPostfix()
        {
            string postfix = "_48";
            string iconName = "icon.png";
            string expectedstring = $"icon{postfix}.png";

            string result = _imageRenamer.AddPostfix(iconName, postfix);
            Assert.AreEqual(expectedstring, result);
        }
    }
}
