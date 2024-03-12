using FireLibs.Logging;
using FireLibs.Logging.Loggers;
using System.Runtime.CompilerServices;
using System.Web;

namespace DSRemapper
{
    /// <summary>
    /// Logger class for DSRemapper
    /// </summary>
    public class DSRLogger
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
        public static void StaticLog(LogLevel level, string message) => logger.Log(level,message);
        /// <inheritdoc cref="Logger.LogDebug(string)"/>
        public static void StaticLogDebug(string message) => logger.LogDebug(message);
        /// <inheritdoc cref="Logger.LogInformation(string)"/>
        public static void StaticLogInformation(string message) => logger.LogInformation(message);
        /// <inheritdoc cref="Logger.LogWarning(string)"/>
        public static void StaticLogWarning(string message) => logger.LogWarning(message);
        /// <inheritdoc cref="Logger.LogError(string)"/>
        public static void StaticLogError(string message) => logger.LogError(message);
        /// <inheritdoc cref="Logger.LogCritical(string)"/>
        public static void StaticLogCritical(string message) => logger.LogCritical(message);

        /// <summary>
        /// Initialize a DSRLogger with a name to identify log entries on the log file.
        /// Alternative you can use static log functions, which not have any name added.
        /// </summary>
        /// <param name="subLoggerId">The DSRLogger name associated with the instance</param>
        /// <returns>A instance of DSRLogger</returns>
        public static DSRLogger GetLogger(string subLoggerId) => new(subLoggerId);

        private readonly string subLoggerId = "Default";
        private DSRLogger(string subLoggerId) => this.subLoggerId = subLoggerId;
        /// <inheritdoc cref="Logger.Log(LogLevel,string)"/>
        public void Log(LogLevel level, string message) => StaticLog(level, $"<{subLoggerId}> {message}");
        /// <inheritdoc cref="Logger.LogDebug(string)"/>
        public void LogDebug(string message) => Log(LogLevel.Debug,message);
        /// <inheritdoc cref="Logger.LogInformation(string)"/>
        public void LogInformation(string message) => Log(LogLevel.Information, message);
        /// <inheritdoc cref="Logger.LogWarning(string)"/>
        public void LogWarning(string message) => Log(LogLevel.Warning, message);
        /// <inheritdoc cref="Logger.LogError(string)"/>
        public void LogError(string message) => Log(LogLevel.Error, message);
        /// <inheritdoc cref="Logger.LogCritical(string)"/>
        public void LogCritical(string message) => Log(LogLevel.Critical, message);

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
            
            return $"<{log.LogLevel}>{HttpUtility.HtmlEncode(log)}</{log.LogLevel}>";
        }
    }
}
