using FireLibs.Logging;
using FireLibs.Logging.Loggers;
using System.Runtime.CompilerServices;

namespace DSRemapper
{
    /// <summary>
    /// Logger class for DSRemapper
    /// </summary>
    public static class DSRLogger
    {
        private static readonly LogLevel[] DefaultLogLevels = [LogLevel.Information, LogLevel.Warning, LogLevel.Error, LogLevel.Critical];
        private static FileLoggerConfiguration fileConfig = new(DefaultLogLevels);
        private static EventLoggerConfiguration eventConfig = new([.. DefaultLogLevels, LogLevel.Debug], [Logger_OnLog]);

        private static readonly Logger logger = LoggerFactory.GetOrCreateLogger("DSRMainLogger",
            (builder) => builder.AddFileLogger(fileConfig).AddEventLogger(eventConfig));

        /// <summary>
        /// This event is called every time that a log occurs.
        /// </summary>
        public static event EventLoggerDelegate? OnLog = null;
        /// <summary>
        /// All log entries occured from the program start
        /// </summary>
        public static List<LogEntry> Entries { get; private set; } = [];
        private static void Logger_OnLog(LogEntry log)
        {
            Entries.Add(log);
            OnLog?.Invoke(log);
        }

        /// <inheritdoc cref="Logger.Log(LogLevel,string)"/>
        public static void Log(LogLevel level, string message) => logger.Log(level,message);
        /// <inheritdoc cref="Logger.LogDebug(string)"/>
        public static void LogDebug(string message) => logger.LogDebug(message);
        /// <inheritdoc cref="Logger.LogInformation(string)"/>
        public static void LogInformation(string message)=>logger.LogInformation(message);
        /// <inheritdoc cref="Logger.LogWarning(string)"/>
        public static void LogWarning(string message) => logger.LogWarning(message);
        /// <inheritdoc cref="Logger.LogError(string)"/>
        public static void LogError(string message) => logger.LogError(message);
        /// <inheritdoc cref="Logger.LogCritical(string)"/>
        public static void LogCritical(string message) => logger.LogCritical(message);
    }
    /// <summary>
    /// LogEntry extension functions
    /// </summary>
    public static class LogEntryExtensions
    {
        /// <summary>
        /// Generates a string with XML/HTML format
        /// </summary>
        /// <returns>A XML/HTML formatted string</returns>
        public static string ToHTMLTag(this LogEntry log)
        {
            return $"<{log.LogLevel}>{log}</{log.LogLevel}>";
        }
    }
}
