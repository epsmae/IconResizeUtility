using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using IconResizeUtility.Service;
using NUnit.Framework;

namespace IconResizeUtility.TestInfrastructure
{
    public class ProjectFileTester
    {
        private readonly IProjectFileUpdater _projectFileUpdater;

        public ProjectFileTester(IProjectFileUpdater projectFileUpdater)
        {
            _projectFileUpdater = projectFileUpdater;
        }

        public void AssertContainsIcon(string csprojFile, IList<string> icons)
        {
            _projectFileUpdater.LoadProjectFile(csprojFile);

            IList<string> loadedIcons = _projectFileUpdater.Icons;

            foreach (string icon in icons)
            {
                Assert.True(loadedIcons.Any(i => i.Contains(icon)));
            }
        }

        public void AssertContainsText(string csprojFile, string expectedText)
        {
            string fileContent = File.ReadAllText(csprojFile);
            Assert.True(fileContent.Contains(expectedText));
        }
    }
}
