using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace IconResizeUtility.Service
{
    public class AndroidResizeService
    {
        private readonly ImageResizer _resizer;
        private readonly ImageRenamer _imageRenamer;

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
        
        public AndroidResizeService(ImageResizer resizer, ImageRenamer imageRenamer)
        {
            _resizer = resizer;
            _imageRenamer = imageRenamer;
        }


        public void Resize(string sourcePath, string destinationPath, bool postfixSize, string prefix, IList<int> requiredSizes)
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
                        string finalIconName;

                        if (postfixSize || requiredSizes.Count > 1)
                        {
                            finalIconName = _imageRenamer.AddPostfix(baseIconName, $"_{requiredSize}");
                        }
                        else
                        {
                            finalIconName = baseIconName;
                        }

                        if (!string.IsNullOrEmpty(prefix))
                        {
                            finalIconName = _imageRenamer.AddPrefix(finalIconName, prefix);
                        }

                        string destinationIconPath = Path.Combine(path, finalIconName);

                        int size = (int) (requiredSize * ResFolderAssociation[resolutionFolder]);

                        _resizer.Resize(file.FullName, destinationIconPath, size, size);
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
