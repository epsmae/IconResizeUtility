using System.Linq;
using IconResizeUtility.App.DataModel;
using Microsoft.Extensions.Logging;

namespace IconResizeUtility.App
{
    public class ArgumentsPrinter : IArgumentsPrinter
    {
        private readonly ILogger<ArgumentsPrinter> _logger;

        public ArgumentsPrinter(ILogger<ArgumentsPrinter> logger)
        {
            _logger = logger;
        }

        public void PrintArguments(Arguments args)
        {
            _logger.LogInformation($"Type: {args.Platform}");
            _logger.LogInformation($"Source folder: {args.SourceFolder}");
            _logger.LogInformation($"Destination folder: {args.DestinationFolder}");

            if (!string.IsNullOrEmpty(args.Prefix))
            {
                _logger.LogInformation($"Prefix: {args.Prefix}");
            }

            if (args.Colors != null && args.Colors.Any())
            {
                _logger.LogInformation($"Use icons sizes: {string.Join(", ", args.Colors.Select(c => $"{c.ColorName}, {c.ColorHexValue}"))}");
            }

            if (args.Sizes != null && args.Sizes.Any())
            {
                _logger.LogInformation($"Use icons sizes: {string.Join(", ", args.Sizes)}");
            }
        }
    }
}
