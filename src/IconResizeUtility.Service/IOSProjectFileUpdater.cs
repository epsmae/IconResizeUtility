using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace IconResizeUtility.Service
{
    public class IOSProjectFileUpdater : IProjectFileUpdater
    {
        private XDocument _xmldoc;
        private string _msbuildNamespace;
        private List<XElement> _iconElements;

        public void LoadProjectFile(string fullFilePath)
        {
            _xmldoc = XDocument.Load(fullFilePath);
            _msbuildNamespace = "{http://schemas.microsoft.com/developer/msbuild/2003}";

            _iconElements = _xmldoc.Descendants(_msbuildNamespace + "ImageAsset").Where(IsIcon).ToList();
        }

        public void AddIcon(string iconId)
        {
            if(!ContainsIcon(iconId))
            {
                XElement root = new XElement(_msbuildNamespace + "ItemGroup");
                XElement asset = new XElement(_msbuildNamespace + "ImageAsset");
                asset.Add(new XAttribute("Include", iconId));
                asset.Add(new XElement(_msbuildNamespace + "Visible", "false"));
                root.Add(asset);
                _xmldoc.Root.Add(root);
                _iconElements.Add(asset);
            }
        }

        public IList<string> Icons
        {
            get
            {
                return _iconElements.Select(element => element.Attribute("Include").Value).ToList();
            }
        }

        public bool ContainsIcon(string iconId)
        {
            return _iconElements.Any(element => IsPng(element, iconId));
        }

        public void Save(string fullFilePath)
        {
            _xmldoc.Save(fullFilePath);
        }

        private static bool IsIcon(XElement element)
        {
            XAttribute attribute = element.Attribute("Include");

            return attribute != null && ! string.IsNullOrEmpty(attribute.Value);
        }


        private static bool IsPng(XElement element, string iconName)
        {
            XAttribute attribute = element.Attribute("Include");

            return attribute != null && attribute.Value == iconName;
        }
    }
}
