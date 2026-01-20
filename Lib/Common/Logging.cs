using Serilog;
using Serilog.Events;

namespace Lib.Common
{
    internal static class Logging
    {
        private static readonly string LogDirectory = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "Aeolus",
            "Logs");

        public static void Initialize()
        {
            Directory.CreateDirectory(LogDirectory);

            var logFilePath = Path.Combine(LogDirectory, "log-.txt");

#if DEBUG
            var minimumLevel = LogEventLevel.Debug;
#else
            var minimumLevel = LogEventLevel.Information;
#endif

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Is(minimumLevel)
                .WriteTo.File(
                    logFilePath,
                    rollingInterval: RollingInterval.Day,
                    retainedFileCountLimit: 30
                )
                .CreateLogger();
        }

        public static void Dispose()
        {
            Log.CloseAndFlush();
        }
    }
}
