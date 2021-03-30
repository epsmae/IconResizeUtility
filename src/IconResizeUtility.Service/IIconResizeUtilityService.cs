using System.Collections.Generic;
using IconResizeUtility.Service.DataModel;

namespace IconResizeUtility.Service
{
    public interface IIconResizeUtilityService
    {
        void Resize(string srcFolder, string dstFolder, string csproj, bool postfixSize, string prefix, List<int> sizeList, IList<RequiredColor> colors, bool convertToValidIconName = true);
    }
}
