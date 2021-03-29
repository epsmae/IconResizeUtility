using System.Collections.Generic;
using IconResizeUtility.Service.DataModel;

namespace IconResizeUtility.App.DataModel
{
    public class Arguments
    {
        public EPlatforms Platform { get; set; }

        public string SourceFolder { get; set; }

        public string DestinationFolder { get; set; }

        public string Prefix { get; set; }

        public bool PostfixSize { get; set; }

        public List<int> Sizes { get; set; }

        public string Csproj { get; set; }

        public IList<RequiredColor> Colors { get; set; }
    }
}
