using System.IO;
using NUnit.Framework;

namespace IconResizeUtility.Service.Test
{
    [TestFixture]
    public class DroidProjectFileUpdaterTest
    {
        private DroidProjectFileUpdater _projectFileUpdater;

        private string ProjectFile
        {
            get
            {
                return Path.Combine(TestContext.CurrentContext.TestDirectory, "TestData", "ProjectFiles", "ResizeUtility.App.Android.csproj");
            }
        }

        private string TmpProjectFile
        {
            get
            {
                return Path.Combine(TestContext.CurrentContext.TestDirectory, "TestData", "ProjectFiles", "ResizeUtility.App.Android.csproj.tmp");
            }
        }



        [SetUp]
        public void Setup()
        {
            _projectFileUpdater = new DroidProjectFileUpdater();
        }

        [Test]
        public void TestAddIcons()
        {
            string expectedIcon1 = "my_test_icon";
            string expectedIcon2 = "my_test_icon2";

            _projectFileUpdater.LoadProjectFile(ProjectFile);

            _projectFileUpdater.AddIcon(expectedIcon1);
            _projectFileUpdater.AddIcon(expectedIcon2);

            Assert.True(_projectFileUpdater.ContainsIcon(expectedIcon2));
            Assert.True(_projectFileUpdater.ContainsIcon(expectedIcon1));
        }

        [Test]
        public void TestAddDuplication()
        {
            string expectedIcon1 = "my_test_icon";

            _projectFileUpdater.LoadProjectFile(ProjectFile);

            _projectFileUpdater.AddIcon(expectedIcon1);
            _projectFileUpdater.AddIcon(expectedIcon1);

            Assert.True(_projectFileUpdater.ContainsIcon(expectedIcon1));
        }


        [Test]
        public void TestSaveLoad()
        {
            string expectedIcon1 = "my_test_icon";
            string expectedIcon2 = "my_test_icon2";

            _projectFileUpdater.LoadProjectFile(ProjectFile);

            _projectFileUpdater.AddIcon(expectedIcon2);
            _projectFileUpdater.AddIcon(expectedIcon1);
            
            _projectFileUpdater.Save(TmpProjectFile);
            _projectFileUpdater.LoadProjectFile(TmpProjectFile);
            
            Assert.True(_projectFileUpdater.ContainsIcon(expectedIcon1));
            Assert.True(_projectFileUpdater.ContainsIcon(expectedIcon2));
        }
    }
}
