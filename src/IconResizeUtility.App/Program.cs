using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Linq;
using System.Reflection;
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
                }
            };

            resizeCommand.Handler = CommandHandler.Create((string type, string srcFolder, string dstFolder, string prefix, string iconSize, bool postfixSize) =>
                    Resize(type, srcFolder, dstFolder, prefix, iconSize, postfixSize));
            
            rootCommand.AddCommand(resizeCommand);

            rootCommand.Invoke(args);
        }

        private static void Resize(string type, string srcFolder, string dstFolder, string prefix, string iconSize, bool postfixSize)
        {
            if (!(type.ToLower() == "ios" || type.ToLower() == "droid"))
            {
                Console.WriteLine("--type only allows ios or droid");
                return;
            }

            List<int> sizeList = new List<int>();
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

            ImageRenamer imageRenamer = new ImageRenamer();
            ImageResizer imageResizer = new ImageResizer();

            if (type.ToLower() == "ios")
            {
                IOSImageResizer resizeService = new IOSImageResizer(imageResizer, imageRenamer);
                resizeService.Resize(srcFolder, dstFolder, postfixSize, prefix, sizeList);
            }
            else
            {
                AndroidResizeService resizeService = new AndroidResizeService(imageResizer, imageRenamer);
                resizeService.Resize(srcFolder, dstFolder, postfixSize, prefix, sizeList);
            }
        }
    }
}
