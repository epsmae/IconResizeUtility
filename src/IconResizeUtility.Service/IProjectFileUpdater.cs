using System.Collections.Generic;

namespace IconResizeUtility.Service
{
    public interface IProjectFileUpdater
    {
        void LoadProjectFile(string fullFilePath);

        void AddIcon(string iconId);

        bool ContainsIcon(string iconId);

        void Save(string fullFilePath);

        IList<string> Icons { get; }
    }
}
