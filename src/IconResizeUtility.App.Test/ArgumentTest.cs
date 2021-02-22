using System.Collections.Generic;
using System.IO;
using IconResizeUtility.Service;
using IconResizeUtility.TestInfrastructure;
using NUnit.Framework;

namespace IconResizeUtility.App.Test
{
    public class ArgumentTest
    {
        [Test]
        public void TestVersion()
        {
            Program.Main(new []{"--version"});
        }

        [Test]
        public void TestHelp()
        {
            Program.Main(new[] { "resize", "--help" });
        }
    }
}