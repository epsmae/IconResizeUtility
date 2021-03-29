using System.Collections.Generic;
using IconResizeUtility.Service.DataModel;

namespace IconResizeUtility.Service
{
    public interface IImageResizeService
    {
        void Resize(string sourcePath, string destinationPath, bool postfixSize, string prefix, IList<int> requiredSizes, IList<RequiredColor> requiredColors = null);
    }
}
