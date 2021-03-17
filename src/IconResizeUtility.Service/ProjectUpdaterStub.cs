using System.Collections.Generic;

namespace IconResizeUtility.Service
{
    public class ProjectUpdaterStub : IProjectFileUpdater
    {
        public void LoadProjectFile(string fullFilePath)
        {
        }

        public void AddIcon(string iconId)
        {
        }

        public bool ContainsIcon(string iconId)
        {
            return true;
        }

        public void Save(string fullFilePath)
        {
        }

        public IList<string> Icons
        {
            get
            {
                return new List<string>();
            }
        }
    }
}
