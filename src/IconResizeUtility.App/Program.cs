using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using IconResizeUtility.Service;

namespace IconResizeUtility.App
{
    public class Program
    {
        public static void Main(string[] args)
        {
            RootCommand rootCommand = new RootCommand();

            Command resizeCommand = new Command("resize")
            {
                new Option<string>("--type")
                {
                    IsRequired = true,
                    Description = "droid or ios"
                },
                new Option<string>("--srcFolder")
                {
                    IsRequired = true,
                    Description = "Folder where the source images are located"
                },
                new Option<string>("--dstFolder")
                {
                    IsRequired = true,
                    Description = "Folder where the results image are written to"
                },
                new Option<string>("--prefix")
                {
                    IsRequired = false,
                    Description = "Optional, prefix icon name"
                },
                new Option<string>("--iconSize")
                {
                    IsRequired = false, 
                    Description = "Optional, provide specific icon size. If non provided the default sizes where used. e.g. 18,24,48"
                },
                new Option<bool>("--postfixSize")
                {
                    IsRequired = false,
                    Description = "Optional, postfix icon size. Required if multiple sizes where choosen"
                },
                new Option<string>("--csproj")
                {
                    IsRequired = false,
                    Description = "Optional, full path to the Xamarin .csproj file where the icons should be added"
                }
            };

            resizeCommand.Handler = CommandHandler.Create((string type, string srcFolder, string dstFolder, string prefix, string iconSize, bool postfixSize, string csproj) =>
                    Resize(type, srcFolder, dstFolder, prefix, iconSize, postfixSize, csproj));
            
            rootCommand.AddCommand(resizeCommand);

            rootCommand.Invoke(args);
        }

        private static void Resize(string type, string srcFolder, string dstFolder, string prefix, string iconSize, bool postfixSize, string csproj)
        {
            if (!(type.ToLower() == "ios" || type.ToLower() == "droid"))
            {
                Console.WriteLine("--type only allows ios or droid");
                return;
            }

            List<int> sizeList = new List<int>();
            Console.WriteLine($"Type: {type}");
            Console.WriteLine($"Source folder: {srcFolder}");
            Console.WriteLine($"Destination folder: {dstFolder}");
            if (!string.IsNullOrEmpty(prefix))
            {
                Console.WriteLine($"Prefix: {prefix}");
            }

            if (string.IsNullOrEmpty(iconSize))
            {
                sizeList.AddRange(AndroidResizeService.DefaultRequiredSizes);
            }
            else
            {
                string[] sizes = iconSize.Replace(" ", "").Split(',');
                foreach (string size in sizes)
                {
                    sizeList.Add(int.Parse(size));
                }
            }

            Console.WriteLine($"Use icons sizes: {string.Join(", ", sizeList)}");

            Console.WriteLine($"PostfixSize: {postfixSize || sizeList.Count > 1}");

            if (!string.IsNullOrEmpty(csproj))
            {
                Console.WriteLine($"Csproj: {csproj}");
            }

            ImageRenamer imageRenamer = new ImageRenamer();
            ImageResizer imageResizer = new ImageResizer();
            IProjectFileUpdater projectUpdater;

            if (type.ToLower() == "ios")
            {
                if (string.IsNullOrEmpty(csproj))
                {
                    projectUpdater = new ProjectUpdaterStub();
                }
                else
                {
                    projectUpdater = new DroidProjectFileUpdater();
                    projectUpdater.LoadProjectFile(csproj);
                }

                IOSImageResizeService resizeService = new IOSImageResizeService(imageResizer, imageRenamer, projectUpdater);
                resizeService.Resize(srcFolder, dstFolder, postfixSize, prefix, sizeList);
                projectUpdater.Save(csproj);
            }
            else
            {
                
                if (string.IsNullOrEmpty(csproj))
                {
                    projectUpdater = new ProjectUpdaterStub();
                }
                else
                {
                    projectUpdater = new DroidProjectFileUpdater();
                    projectUpdater.LoadProjectFile(csproj);
                }

                AndroidResizeService resizeService = new AndroidResizeService(imageResizer, imageRenamer, projectUpdater);
                resizeService.Resize(srcFolder, dstFolder, postfixSize, prefix, sizeList);
                projectUpdater.Save(csproj);
            }
        }
    }
}
