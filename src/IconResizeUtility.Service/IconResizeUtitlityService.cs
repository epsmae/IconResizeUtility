using System.Collections.Generic;
using IconResizeUtility.Service.DataModel;

namespace IconResizeUtility.Service
{
    public class IconResizeUtitlityService : IIconResizeUtilityService
    {
        private readonly IImageResizeService _resizeService;
        private readonly IProjectFileUpdater _projectFileUpdater;

        public IconResizeUtitlityService(IImageResizeService resizeService, IProjectFileUpdater projectFileUpdater)
        {
            _resizeService = resizeService;
            _projectFileUpdater = projectFileUpdater;
        }

        public void Resize(string srcFolder, string dstFolder, string csproj, bool postfixSize, string prefix, List<int> sizeList, IList<RequiredColor> colors)
        {
            _projectFileUpdater.LoadProjectFile(csproj);
            _resizeService.Resize(srcFolder, dstFolder, postfixSize, prefix, sizeList, colors);
            _projectFileUpdater.Save(csproj);
        }
    }
}
