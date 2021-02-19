using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace IconResizeUtility.Service
{
    public class AndroidResizeService
    {
        private readonly ImageResizer _resizer;
        private readonly RenameUtility _renameUtility;
        private readonly string _cacheDirectory;


        public static IDictionary<string, double> ResolutionFolders = new Dictionary<string, double>
        {
            {"drawable-mdpi", 1.0}, // 48*48
            {"drawable-hdpi", 1.5}, // 72*72
            {"drawable-xhdpi", 2.0}, // 96*96
            {"drawable-xxhdpi", 3.0}, // 144*144
            {"drawable-xxxhdpi", 4.0}, // 192* 192
        };

        public static IList<int> DefaultRequiredSizes = new List<int>
        {
            18,
            20,
            24,
            36,
            48
        };


        public AndroidResizeService(ImageResizer resizer, RenameUtility renameUtility)
        {
            _resizer = resizer;
            _renameUtility = renameUtility;
        }


        public void Resize(string sourcePath, string destinationPath, bool postfixSize, string suffix, IList<int> requiredSizes)
        {
            string[] resolutionFolders = ResolutionFolders.Keys.ToArray();

            EnsureDirectoriesExist(destinationPath, resolutionFolders);

            DirectoryInfo directoryInfo = new DirectoryInfo(sourcePath);

            foreach (FileInfo file in directoryInfo.EnumerateFiles())
            {
                foreach (string resolutionFolder in resolutionFolders)
                {
                    string baseIconName = _renameUtility.ConvertToValidIconName(file.Name);

                    string path = Path.Combine(destinationPath, resolutionFolder);

                    foreach (int requiredSize in requiredSizes)
                    {
                        string finalIconName;

                        if (postfixSize || requiredSizes.Count > 1)
                        {
                            finalIconName = _renameUtility.AddPostfix(baseIconName, $"_{requiredSize}");
                        }
                        else
                        {
                            finalIconName = baseIconName;
                        }

                        if (!string.IsNullOrEmpty(suffix))
                        {
                            finalIconName = _renameUtility.AddSuffix(finalIconName, suffix);
                        }

                        string destinationIconPath = Path.Combine(path, finalIconName);

                        int size = (int) (requiredSize * ResolutionFolders[resolutionFolder]);

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
            if (!System.IO.Directory.Exists(directory))
            {
                System.IO.Directory.CreateDirectory(directory);
            }
        }
    }
}
