﻿using System.Collections.Generic;
using System.IO;
using System.Linq;
using IconResizeUtility.Service.DataModel;
using Newtonsoft.Json;

namespace IconResizeUtility.Service
{
    public class IOSImageResizeService
    {
        /// <summary>
        /// Association between resource name and scale factor
        /// </summary>
        public static IDictionary<string, double> ResNameAssociation = new Dictionary<string, double>
        {
            {"1x", 1.0}, // 48*48
            {"2x", 2.0}, // 96*96
            {"3x", 3.0}, // 144*144
        };


        /// <summary>
        /// The usually provided sizes for icons
        /// </summary>
        public static IList<int> DefaultRequiredSizes = new List<int>
        {
            18,
            20,
            24,
            36,
            48
        };

        private readonly ImageResizer _resizer;
        private readonly ImageRenamer _imageRenamer;
        private readonly IProjectFileUpdater _projectFileUpdater;

        public IOSImageResizeService(ImageResizer resizer, ImageRenamer imageRenamer, IProjectFileUpdater projectFileUpdater)
        {
            _resizer = resizer;
            _imageRenamer = imageRenamer;
            _projectFileUpdater = projectFileUpdater;
        }

        public void Resize(string sourcePath, string destinationPath, bool postfixSize, string prefix, IList<int> requiredSizes)
        {
            string[] resolutionFolders = ResNameAssociation.Keys.ToArray();
            
            EnsureDirectoryExists(destinationPath);

            DirectoryInfo directoryInfo = new DirectoryInfo(sourcePath);

            foreach (FileInfo file in directoryInfo.EnumerateFiles())
            {
                foreach (int requiredSize in requiredSizes)
                {
                    IList<Image> imagesInfo = new List<Image>();

                    string baseIconName = _imageRenamer.ConvertToValidIconName(file.Name);

                    if (!string.IsNullOrEmpty(prefix))
                    {
                        baseIconName = _imageRenamer.AddPrefix(baseIconName, prefix);
                    }

                    if (postfixSize || requiredSizes.Count > 1)
                    {
                        baseIconName = _imageRenamer.AddPostfix(baseIconName, $"_{requiredSize}pt");
                    }


                    string folderName = $"{Path.GetFileNameWithoutExtension(baseIconName)}.imageset";
                    string folderPath = Path.Combine(destinationPath, folderName);
                    EnsureDirectoryExists(folderPath);
                    
                    foreach (string scaleFactorString in resolutionFolders)
                    {
                        string finalIconName = _imageRenamer.AddPostfix(baseIconName, $"_{scaleFactorString}");
                        
                        string destinationIconPath = Path.Combine(folderPath, finalIconName);

                        int size = (int)(requiredSize * ResNameAssociation[scaleFactorString]);

                        _resizer.Resize(file.FullName, destinationIconPath, size, size);

                        Image image = new Image
                        {
                            Appearances = new string[]{},
                            Scale = scaleFactorString,
                            Idiom = "universal",
                            FileName = finalIconName,
                        };
                        imagesInfo.Add(image);
                        string relativeIconPath = Path.Combine("Assets.xcassets", folderName, finalIconName);
                        _projectFileUpdater.AddIcon(relativeIconPath);
                    }

                    CreateContentJson(folderPath, imagesInfo);
                    string relativeContentFilePath = Path.Combine("Assets.xcassets", folderName, "Contents.json");
                    _projectFileUpdater.AddIcon(relativeContentFilePath);
                }
            }
        }

        private void CreateContentJson(string path, IList<Image> imagesInfo)
        {
            Contents contents = new Contents
            {
                Images = imagesInfo.ToArray(),
                Properties = new Properties(),
                Info = new Info {Author = "", Version = 1}
            };

            string fullFileName = Path.Combine(path, "Contents.json");
            File.WriteAllText(fullFileName, JsonConvert.SerializeObject(contents, Formatting.Indented));
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
