using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;
using System.Reflection;

namespace IconResizeUtility.App
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Create a root command with some options
            var rootCommand = new RootCommand();

            Command versionCommand = new Command("version");
            versionCommand.Handler = CommandHandler.Create(GetVersion);
            rootCommand.AddCommand(versionCommand);
            
            // Parse the incoming args and invoke the handler
            rootCommand.InvokeAsync(args).Wait();
        }

        private static void GetVersion()
        {
            string version = Assembly.GetAssembly(typeof(Program)).GetName().Version.ToString();
            Console.WriteLine($"Version: {version}");
        }
    }
}
