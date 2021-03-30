using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using IconResizeUtility.App.DataModel;
using IconResizeUtility.Service;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

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
                },
                new Option("--doNotRename")
                {
                    IsRequired = false,
                    Description = "Avoid replacing invalid characters in the icon name with an '_'"
                }
            };

            resizeCommand.Handler = CommandHandler.Create((ProgArgs progArgs) => Resize(progArgs));
            
            rootCommand.AddCommand(resizeCommand);

            rootCommand.Invoke(args);
        }

        private static void Resize(ProgArgs progArgs)
        {
            Arguments args = ParameterHelper.Parse(progArgs);
            
            ServiceCollection services = new ServiceCollection();
            services.AddLogging(builder => builder.AddConsole());
            services.AddSingleton(GetCsProjUpdater(args.Platform, args.Csproj));
            services.AddSingleton<IImageRenamer, ImageRenamer>();
            services.AddSingleton<IImageResizer, ImageResizer>();
            services.AddSingleton<IArgumentsPrinter, ArgumentsPrinter>();
            services.AddSingleton<IIconResizeUtilityService, IconResizeUtitlityService>();
            AddResizeService(services, args.Platform);
            IServiceProvider serviceProvider = services.BuildServiceProvider();

            IArgumentsPrinter printer = serviceProvider.GetService<IArgumentsPrinter>();
            printer.PrintArguments(args);

            IIconResizeUtilityService utilityService = serviceProvider.GetService<IIconResizeUtilityService>();
            utilityService.Resize(args.SourceFolder, args.DestinationFolder, args.Csproj, args.PostfixSize, args.Prefix, args.Sizes, args.Colors, args.ResizeToValidIconName);
        }

        private static void AddResizeService(IServiceCollection serviceCollection, EPlatforms platform)
        {
            if (platform == EPlatforms.Ios)
            {
                serviceCollection.AddSingleton<IImageResizeService, IOSImageResizeService>();
            }
            else
            {
                serviceCollection.AddSingleton<IImageResizeService, DroidResizeService>();
            }
        }

        private static IProjectFileUpdater GetCsProjUpdater(EPlatforms platform, string csproj)
        {
            if (platform == EPlatforms.Ios && csproj != null)
            {
                return new IOSProjectFileUpdater();
            }
            
            if (platform == EPlatforms.Droid && csproj != null)
            {
                return new DroidProjectFileUpdater();
            }

            return new ProjectUpdaterStub();
        }
    }
}
