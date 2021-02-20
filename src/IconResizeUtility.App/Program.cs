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
                new Option<string>("--srcFolder") { IsRequired = true },
                new Option<string>("--dstFolder"){ IsRequired = true },
                new Option<string>("--prefix") { IsRequired = false },
                new Option<string>("--iconSize") { IsRequired = false },
                new Option<bool>("--postfixSize") { IsRequired = false }
            };

            resizeCommand.Handler = CommandHandler.Create((string srcFolder, string dstFolder, string prefix, string iconSize, bool postfixSize) =>
                    Resize(srcFolder, dstFolder, prefix, iconSize, postfixSize));
            
            rootCommand.AddCommand(resizeCommand);

            rootCommand.Invoke(args);
        }

        private static void Resize(string srcFolder, string dstFolder, string prefix, string iconSize, bool postfixSize)
        {
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

            RenameUtility renameUtility = new RenameUtility();
            ImageResizer imageResizer = new ImageResizer();

            AndroidResizeService resizeService = new AndroidResizeService(imageResizer, renameUtility);

            resizeService.Resize(srcFolder, dstFolder, postfixSize, prefix, sizeList);
        }
    }
}
