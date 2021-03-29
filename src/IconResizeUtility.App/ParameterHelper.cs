using System.Collections.Generic;
using IconResizeUtility.App.DataModel;
using IconResizeUtility.Service;
using IconResizeUtility.Service.DataModel;
using Newtonsoft.Json;

namespace IconResizeUtility.App
{
    public static class ParameterHelper
    {
        public static Arguments Parse(ProgArgs progArgs)
        {
            return new Arguments
            {
                Platform = GetPlatform(progArgs.Type),
                SourceFolder = progArgs.SrcFolder,
                DestinationFolder = progArgs.DstFolder,
                Prefix = progArgs.Prefix,
                PostfixSize = progArgs.PostfixSize,
                Csproj = progArgs.Csproj,
                Sizes = ParseSizes(progArgs.IconSize, GetPlatform(progArgs.Type)),
                Colors = ParseColors(progArgs.Color)
            };
        }

        private static IList<RequiredColor> ParseColors(string color)
        {
            IList<RequiredColor> colors = new List<RequiredColor>();

            if (!string.IsNullOrEmpty(color))
            {
                if (color.StartsWith("#"))
                {
                    colors.Add(new RequiredColor { ColorHexValue = color });
                }
                else
                {
                    IDictionary<string, string> items = JsonConvert.DeserializeObject<Dictionary<string, string>>(color);
                    foreach (KeyValuePair<string, string> item in items)
                    {
                        colors.Add(new RequiredColor
                        {
                            ColorName = item.Key,
                            ColorHexValue = item.Value
                        });
                    }
                }
            }

            return colors;
        }

        private static List<int> ParseSizes(string iconSize, EPlatforms platform)
        {
            List<int> sizeList = new List<int>();

            if(!string.IsNullOrEmpty(iconSize))
            {
                string[] sizes = iconSize.Replace(" ", "").Split(',');
                foreach (string size in sizes)
                {
                    sizeList.Add(int.Parse(size));
                }
            }
            else if (string.IsNullOrEmpty(iconSize) && platform == EPlatforms.Ios)
            {
                sizeList.AddRange(IOSImageResizeService.DefaultRequiredSizes);
            }
            else
            {
                sizeList.AddRange(DroidResizeService.DefaultRequiredSizes);
            }

            return sizeList;
        }

        private static EPlatforms GetPlatform(string type)
        {
            if (type.ToLower() == "ios")
            {
                return EPlatforms.Ios;
            }

            return EPlatforms.Droid;
        }
    }
}
