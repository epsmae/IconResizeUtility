using System.Collections.Generic;
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

            //EnsureDirectoriesExist(destinationPath, resolutionFolders);

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
                            FileName = finalIconName,
                            Idiom = "universal",
                            Scale = scaleFactorString,
                            Size = $"{requiredSize}x{requiredSize}"
                        };
                        imagesInfo.Add(image);
                        string relativeIconPath = Path.Combine("Assets.xcassets", folderName, finalIconName);
                        _projectFileUpdater.AddIcon(relativeIconPath);
                    }

                    CreateContentJson(folderPath, imagesInfo);
                }
            }
        }

        private void CreateContentJson(string path, IList<Image> imagesInfo)
        {
            Contents contents = new Contents();
            contents.Images = imagesInfo.ToArray();
            contents.Info = new Info
            {
                Author = "xcode",
                TemplateRenderingIntent = "template",
                Version = 1
            };

            string fullFileName = Path.Combine(path, "Contents.json");
            File.WriteAllText(fullFileName, JsonConvert.SerializeObject(contents, Formatting.Indented));
        }

        //private void EnsureDirectoriesExist(string destinationPath, string[] directories)
        //{
        //    foreach (string directory in directories)
        //    {
        //        EnsureDirectoryExists(Path.Combine(destinationPath, directory));
        //    }
        //}

        private void EnsureDirectoryExists(string directory)
        {
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
        }
    }
}
