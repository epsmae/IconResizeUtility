using NUnit.Framework;

namespace IconResizeUtility.App.Test
{
    public class Tests
    {
        [Test]
        public void TestVersion()
        {
            Program.Main(new []{"--version"});
        }
    }
}