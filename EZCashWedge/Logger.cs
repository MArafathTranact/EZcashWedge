using Serilog;
using System;
using System.IO;


namespace EZCashWedge
{
    public static class Logger
    {
        // Serilog handles thread-safety internally, so we don't need manual locking objects or queues.
        static Logger()
        {

            string serviceFolder = AppDomain.CurrentDomain.BaseDirectory;

            string archiveFolder = Path.Combine(serviceFolder, "Archivedlogs");

            var fileSize = string.IsNullOrEmpty(ServiceConfiguration.GetFileLocation("TraceFileSize")) ? 100 : Convert.ToInt16(ServiceConfiguration.GetFileLocation("TraceFileSize"));
            var rollOutDays = string.IsNullOrEmpty(ServiceConfiguration.GetFileLocation("DeleteArchived")) ? 60 : Convert.ToInt16(ServiceConfiguration.GetFileLocation("DeleteArchived"));
            rollOutDays = rollOutDays == 0 ? 60 : rollOutDays;
            fileSize = fileSize == 0 ? 100 : fileSize;
            string outputTemplate = "{Timestamp:yyyy-MM-dd HH:mm:ss.ffff} [{Level:u3}] {Message:lj}{NewLine}{Exception}";

            Log.Logger = new LoggerConfiguration()
                            .WriteTo.File(
                               path: Path.Combine(serviceFolder, "EZCashWedgeServer.txt"),
                               outputTemplate: outputTemplate, // Apply the custom format here
                               rollingInterval: RollingInterval.Infinite,
                               rollOnFileSizeLimit: true, // MUST be true to trigger the hook
                               fileSizeLimitBytes: fileSize * 1024 * 1024, // 1MB
                               hooks: new ArchiveFileHook(archiveFolder, rollOutDays),
                               retainedFileCountLimit: null
                            )
                            .CreateLogger();
        }

        public static void LogWithNoLock(string message)
        {
            Log.Information(message);
        }

        public static void LogExceptionWithNoLock(string message, Exception exception)
        {
            // Serilog automatically formats the message and the StackTrace
            Log.Error(exception, message);
        }

        public static void LogWarningWithNoLock(string message)
        {
            Log.Warning(message);
        }
    }
    //public static class Logger
    //{
    //    public static object _locked = new object();
    //    private static ILogger logger { get; set; }
    //    static Logger()
    //    {
    //        logger = LogManager.GetCurrentClassLogger();

    //    }

    //    private static void LogInfo(string information)
    //    {
    //        try
    //        {
    //            logger.Info(information);
    //        }
    //        catch (Exception)
    //        {
    //        }
    //    }

    //    private static void LogError(string message, Exception exception)
    //    {
    //        try
    //        {
    //            logger.Error(exception, message);
    //        }
    //        catch (Exception)
    //        {
    //        }
    //    }

    //    private static void LogWarning(string message)
    //    {
    //        try
    //        {
    //            logger.Warn(message);
    //        }
    //        catch (Exception)
    //        {
    //        }
    //    }

    //    public static void LogWithNoLock(string message)
    //    {
    //        LogInfo(message);
    //    }

    //    public static void LogExceptionWithNoLock(string message, Exception exception)
    //    {
    //        LogError(message, exception);
    //    }

    //    public static void LogWarningWithNoLock(string message)
    //    {
    //        LogWarning(message);
    //    }
    //}
}
