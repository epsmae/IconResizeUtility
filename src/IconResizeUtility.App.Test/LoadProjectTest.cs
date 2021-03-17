using System.IO;
using NUnit.Framework;

namespace IconResizeUtility.App.Test
{
    public class LoadProjectTest
    {
        [SetUp]
        public void Setup()
        {

        }


        private string SampleProjFile
        {
            get
            {
                return Path.Combine(TestContext.CurrentContext.TestDirectory, "TestProjectFile", "SampleProject.csproj");
            }
        }
    }
}