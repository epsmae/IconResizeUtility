using System.Collections.Generic;
using System.IO;
using System.Linq;
using IconResizeUtility.Service.DataModel;

namespace IconResizeUtility.Service
{
    public class AndroidResizeService
    {
        private readonly ImageResizer _resizer;
        private readonly ImageRenamer _imageRenamer;
        private readonly IProjectFileUpdater _projectFileUpdater;

        /// <summary>
        /// Association between resource folder and scale factor
        /// </summary>
        public static IDictionary<string, double> ResFolderAssociation = new Dictionary<string, double>
        {
            {"drawable-mdpi", 1.0}, // 48*48
            {"drawable-hdpi", 1.5}, // 72*72
            {"drawable-xhdpi", 2.0}, // 96*96
            {"drawable-xxhdpi", 3.0}, // 144*144
            {"drawable-xxxhdpi", 4.0}, // 192* 192
        };

        /// <summary>
        /// The usually provided sizes for icons
        /// The size is related to the drawable-mdpi otherwise ResFolderAssociation
        /// </summary>
        public static IList<int> DefaultRequiredSizes = new List<int>
        {
            18,
            20,
            24,
            36,
            48
        };
        
        public AndroidResizeService(ImageResizer resizer, ImageRenamer imageRenamer, IProjectFileUpdater projectFileUpdater)
        {
            _resizer = resizer;
            _imageRenamer = imageRenamer;
            _projectFileUpdater = projectFileUpdater;
        }
        
        public void Resize(string sourcePath, string destinationPath, bool postfixSize, string prefix, IList<int> requiredSizes, IList<RequiredColor> requiredColors = null)
        {
            string[] resolutionFolders = ResFolderAssociation.Keys.ToArray();

            EnsureDirectoriesExist(destinationPath, resolutionFolders);

            DirectoryInfo directoryInfo = new DirectoryInfo(sourcePath);

            foreach (FileInfo file in directoryInfo.EnumerateFiles())
            {
                foreach (string resolutionFolder in resolutionFolders)
                {
                    string baseIconName = _imageRenamer.ConvertToValidIconName(file.Name);

                    string path = Path.Combine(destinationPath, resolutionFolder);

                    foreach (int requiredSize in requiredSizes)
                    {
                        string finalIconName = baseIconName;
                        
                        if (postfixSize || requiredSizes.Count > 1)
                        {
                            finalIconName = _imageRenamer.AddPostfix(finalIconName, $"_{requiredSize}");
                        }

                        if (!string.IsNullOrEmpty(prefix))
                        {
                            finalIconName = _imageRenamer.AddPrefix(finalIconName, prefix);
                        }

                        int size = (int) (requiredSize * ResFolderAssociation[resolutionFolder]);

                        if (requiredColors != null && requiredColors.Any())
                        {
                            foreach (RequiredColor requiredColor in requiredColors)
                            {
                                string colorIconName = finalIconName;
                                if (requiredColors.Count > 1)
                                {
                                    colorIconName = _imageRenamer.AddPostfix(colorIconName, $"_{requiredColor.ColorName}");
                                }

                                string destinationIconPath = Path.Combine(path, colorIconName);
                                _resizer.Resize(file.FullName, destinationIconPath, size, size, requiredColor.ColorHexValue);
                                string relativeIconPath = Path.Combine("Resources", resolutionFolder, colorIconName);
                                _projectFileUpdater.AddIcon(relativeIconPath);
                            }
                        }
                        else
                        {
                            string destinationIconPath = Path.Combine(path, finalIconName);
                            _resizer.Resize(file.FullName, destinationIconPath, size, size);
                            string relativeIconPath = Path.Combine("Resources", resolutionFolder, finalIconName);
                            _projectFileUpdater.AddIcon(relativeIconPath);
                        }
                    }
                }
            }
        }

        private void EnsureDirectoriesExist(string destinationPath, string[] directories)
        {
            foreach (string directory in directories)
            {
                EnsureDirectoryExists(Path.Combine(destinationPath, directory));
            }
        }

        private void EnsureDirectoryExists(string directory)
        {
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
        }
    }
}
