using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Xml.Linq;

namespace IconResizeUtility.Service
{
    public class DroidProjectFileUpdater : IProjectFileUpdater
    {
        private XDocument _xmldoc;
        private string _msbuildNamespace;
        private List<XElement> _iconElements;

        public void LoadProjectFile(string fullFilePath)
        {
            _xmldoc = XDocument.Load(fullFilePath);
            _msbuildNamespace = "{http://schemas.microsoft.com/developer/msbuild/2003}";

            _iconElements = _xmldoc.Descendants(_msbuildNamespace + "AndroidResource").Where(IsIcon).ToList();
        }

        public void AddIcon(string iconId)
        {
            if(!ContainsIcon(iconId))
            {
                XElement root = new XElement(_msbuildNamespace + "ItemGroup");
                XElement resource = new XElement(_msbuildNamespace + "AndroidResource");
                resource.Add(new XAttribute("Include", iconId));
                root.Add(resource);
                _xmldoc.Root.Add(root);
                _iconElements.Add(resource);
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
