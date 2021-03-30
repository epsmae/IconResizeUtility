using System.Collections.Generic;
using System.IO;
using System.Linq;
using IconResizeUtility.Service.DataModel;

namespace IconResizeUtility.Service
{
    public class DroidResizeService : IImageResizeService
    {
        private readonly IImageResizer _resizer;
        private readonly IImageRenamer _imageRenamer;
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
        
        public DroidResizeService(IImageResizer resizer, IImageRenamer imageRenamer, IProjectFileUpdater projectFileUpdater)
        {
            _resizer = resizer;
            _imageRenamer = imageRenamer;
            _projectFileUpdater = projectFileUpdater;
        }
        
        public void Resize(string sourcePath, string destinationPath, bool postfixSize, string prefix, IList<int> requiredSizes, bool convertToValidIconName = true, IList<RequiredColor> requiredColors = null)
        {
            string[] resolutionFolders = ResFolderAssociation.Keys.ToArray();

            EnsureDirectoriesExist(destinationPath, resolutionFolders);

            DirectoryInfo directoryInfo = new DirectoryInfo(sourcePath);

            foreach (FileInfo file in directoryInfo.EnumerateFiles())
            {
                foreach (string resolutionFolder in resolutionFolders)
                {
                    string baseIconName = convertToValidIconName ? _imageRenamer.ConvertToValidIconName(file.Name) : file.Name;

                    string path = Path.Combine(destinationPath, resolutionFolder);

                    foreach (int requiredSize in requiredSizes)
                    {
                        string finalIconName = baseIconName;
                        
                        finalIconName = HandlePrefix(prefix, finalIconName);

                        int size = (int) (requiredSize * ResFolderAssociation[resolutionFolder]);

                        if (requiredColors != null && requiredColors.Any())
                        {
                            foreach (RequiredColor requiredColor in requiredColors)
                            {
                                string colorIconName = finalIconName;
                                
                                colorIconName = HandlePostfixColor(requiredColors, colorIconName, requiredColor);

                                colorIconName = HandlePostfixSize(postfixSize, requiredSizes, colorIconName, requiredSize);

                                Resize(path, colorIconName, file, size, requiredColor, resolutionFolder);
                            }
                        }
                        else
                        {
                            finalIconName = HandlePostfixSize(postfixSize, requiredSizes, finalIconName, requiredSize);
                            Resize(path, finalIconName, file, size, null, resolutionFolder);
                        }
                    }
                }
            }
        }

        private string HandlePostfixColor(IList<RequiredColor> requiredColors, string colorIconName, RequiredColor requiredColor)
        {
            if (requiredColors.Count > 1)
            {
                colorIconName = _imageRenamer.AddPostfix(colorIconName, $"_{requiredColor.ColorName}");
            }

            return colorIconName;
        }

        private void Resize(string path, string colorIconName, FileInfo file, int size, RequiredColor? requiredColor,
            string resolutionFolder)
        {
            string destinationIconPath = Path.Combine(path, colorIconName);
            _resizer.Resize(file.FullName, destinationIconPath, size, size, requiredColor?.ColorHexValue);
            string relativeIconPath = Path.Combine("Resources", resolutionFolder, colorIconName);
            _projectFileUpdater.AddIcon(relativeIconPath);
        }

        private string HandlePostfixSize(bool postfixSize, IList<int> requiredSizes, string finalIconName, int requiredSize)
        {
            if (postfixSize || requiredSizes.Count > 1)
            {
                finalIconName = _imageRenamer.AddPostfix(finalIconName, $"_{requiredSize}");
            }

            return finalIconName;
        }

        private string HandlePrefix(string prefix, string iconName)
        {
            if (!string.IsNullOrEmpty(prefix))
            {
                iconName = _imageRenamer.AddPrefix(iconName, prefix);
            }

            return iconName;
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
