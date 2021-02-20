using System.Text.RegularExpressions;

namespace IconResizeUtility.Service
{
    public class ImageRenamer
    {
        private readonly Regex _regex;

        public ImageRenamer()
        {
            _regex = new Regex("[^0-9a-zA-Z_]");
        }
        
        public string ConvertToValidIconName(string iconName)
        {
            int index = iconName.LastIndexOf('.');
            string extension = iconName.Substring(index, iconName.Length - index);
            string name = iconName.Substring(0, index);

            return $"{_regex.Replace(name, "_").ToLower()}{extension.ToLower()}";
        }

        public string AddPostfix(string iconName, string postfix)
        {
            int index = iconName.LastIndexOf('.');
            string extension = iconName.Substring(index, iconName.Length - index);
            string name = iconName.Substring(0, index);

            return $"{name}{postfix}{extension}";
        }

        public string AddPrefix(string iconName, string pefix)
        {
            return $"{pefix}{iconName}";
        }
    }
}
