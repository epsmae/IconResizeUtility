using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using IconResizeUtility.Service;
using IconResizeUtility.Service.DataModel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

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
                },
                new Option<string>("--color")
                {
                    IsRequired = false,
                    Description = "Optional, single color like \"#55FF5560\" or multiple colors like \"{\"red\":\"#FF0000\",\"green\":\"#00FF00\"}\""
                }
            };

            resizeCommand.Handler = CommandHandler.Create((string type, string srcFolder, string dstFolder, string prefix, string iconSize, bool postfixSize, string csproj, string color) =>
                    Resize(type, srcFolder, dstFolder, prefix, iconSize, postfixSize, csproj, color));
            
            rootCommand.AddCommand(resizeCommand);

            rootCommand.Invoke(args);
        }

        private static void Resize(string type, string srcFolder, string dstFolder, string prefix, string iconSize, bool postfixSize, string csproj, string color)
        {
            ServiceCollection services = new ServiceCollection();
            services.AddLogging(builder => builder.AddConsole());

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

            IList<RequiredColor> colors = new List<RequiredColor>();

            if (!string.IsNullOrEmpty(color))
            {
                Console.WriteLine($"Color: {color}");

                if (color.StartsWith("#"))
                {
                    colors.Add(new RequiredColor(){ColorHexValue = color});
                }
                else
                {
                    IDictionary<string, string> items = JsonConvert.DeserializeObject<Dictionary<string, string>>(color);
                    foreach (KeyValuePair<string, string> item in items)
                    {
                        colors.Add(new RequiredColor
                        {
                            ColorName = item.Key,
                            ColorHexValue = item.Value
                        });
                    }
                }
            }
            
            services.AddSingleton(GetCsProjUpdater(type.ToLower(), csproj));
            services.AddSingleton(new ImageRenamer());
            services.AddSingleton(new ImageResizer());
            services.AddSingleton<IIconResizeUtilityService, IconResizeUtitlityService>();

            if (type.ToLower() == "ios")
            {
                services.AddSingleton<IImageResizeService, IOSImageResizeService>();
            }
            else
            {
                services.AddSingleton<IImageResizeService, AndroidResizeService>();
            }

            IServiceProvider serviceProvider = services.BuildServiceProvider();

            IIconResizeUtilityService utilityService = serviceProvider.GetService<IIconResizeUtilityService>();
            utilityService.Resize(srcFolder, dstFolder, csproj, postfixSize, prefix, sizeList, colors);
        }

        private static IProjectFileUpdater GetCsProjUpdater(string type, string csproj)
        {
            if (type == "ios" && csproj != null)
            {
                return new IOSProjectFileUpdater();

            }
            
            if (type == "droid" && csproj != null)
            {
                return new DroidProjectFileUpdater();
            }

            return new ProjectUpdaterStub();
        }
    }
}
