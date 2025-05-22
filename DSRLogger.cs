using Microsoft.Extensions.Logging;
using System.Runtime.CompilerServices;
using System.Web;
using DSRemapper.Core.Loggers;
using System;

namespace DSRemapper
{
    /// <summary>
    /// Logger class for DSRemapper
    /// </summary>
    public class DSRLogger
    {
        private static FileLoggerConfiguration fileConfig = new();
        private static ILoggerFactory logFac = LoggerFactory.Create((builder) =>
            builder.AddProvider(new FileLoggerProvider(fileConfig))
            .AddProvider(new EventLoggerProvider([Logger_OnLog])));

        private static ILogger logger = logFac.CreateLogger("Default Logger");

        /// <summary>
        /// This event is called every time that a log occurs.
        /// </summary>
        public static event LogEventHandler? OnLog = null;
        /// <summary>
        /// All log entries occured from the program start
        /// </summary>
        public static List<(LogLevel logLevel, EventId eventId, string category, string message)> Entries { get; private set; } = [];

        private static void Logger_OnLog(LogLevel logLevel, EventId eventId, string category, string message)
        {
            Entries.Add((logLevel, eventId, category, message));
            OnLog?.Invoke(logLevel, eventId, category, message);
        }

        /// <inheritdoc cref="LoggerExtensions.Log(ILogger, LogLevel, string?, object?[])"/>
        public static void StaticLog(LogLevel level, string message, params object?[] args) => logger.Log(level, message, args);
        /// <inheritdoc cref="LoggerExtensions.LogTrace(ILogger, string?, object?[])"/>
        public static void StaticLogTrace(string message, params object?[] args) => logger.LogTrace(message, args);
        /// <inheritdoc cref="LoggerExtensions.LogDebug(ILogger, string?, object?[])"/>
        public static void StaticLogDebug(string message, params object?[] args) => logger.LogDebug(message, args);
        /// <inheritdoc cref="LoggerExtensions.LogInformation(ILogger, string?, object?[])"/>
        public static void StaticLogInformation(string message, params object?[] args) => logger.LogInformation(message, args);
        /// <inheritdoc cref="LoggerExtensions.LogWarning(ILogger, string?, object?[])"/>
        public static void StaticLogWarning(string message, params object?[] args) => logger.LogWarning(message, args);
        /// <inheritdoc cref="LoggerExtensions.LogError(ILogger, string?, object?[])"/>
        public static void StaticLogError(string message, params object?[] args) => logger.LogError(message, args);
        /// <inheritdoc cref="LoggerExtensions.LogCritical(ILogger, string?, object?[])"/>
        public static void StaticLogCritical(string message, params object?[] args) => logger.LogCritical(message, args);

        /// <summary>
        /// Initialize a DSRLogger with a name to identify log entries on the log file.
        /// Alternative you can use static log functions, which not have any name added.
        /// </summary>
        /// <param name="subLoggerId">The DSRLogger name associated with the instance</param>
        /// <returns>A instance of DSRLogger</returns>
        public static DSRLogger GetLogger(string subLoggerId) => new(subLoggerId);
        /// <summary>
        /// Initialize a DSRLogger with a name to identify log entries on the log file.
        /// Alternative you can use static log functions, which not have any name added.
        /// </summary>
        /// <typeparam name="T">The type which name will be used for the logger</typeparam>
        /// <returns>A instance of DSRLogger</returns>
        public static DSRLogger GetLogger<T>() => new(typeof(T).FullName ?? "");


        private readonly ILogger _logger;
        private DSRLogger(string subLoggerId) : this(logFac.CreateLogger(subLoggerId)) { }
        private DSRLogger(ILogger logger) => _logger = logger;

        /// <inheritdoc cref="LoggerExtensions.Log(ILogger, LogLevel, string?, object?[])"/>
        public void Log(LogLevel level, string message, params object?[] args) => _logger.Log(level,message,args);
        /// <inheritdoc cref="LoggerExtensions.LogTrace(ILogger, string?, object?[])"/>
        public void LogTrace(string message, params object?[] args) => _logger.LogTrace(message, args);
        /// <inheritdoc cref="LoggerExtensions.LogDebug(ILogger, string?, object?[])"/>
        public void LogDebug(string message, params object?[] args) => _logger.LogDebug(message,args);
        /// <inheritdoc cref="LoggerExtensions.LogInformation(ILogger, string?, object?[])"/>
        public void LogInformation(string message, params object?[] args) => _logger.LogInformation(message, args);
        /// <inheritdoc cref="LoggerExtensions.LogWarning(ILogger, string?, object?[])"/>
        public void LogWarning(string message, params object?[] args) => _logger.LogWarning(message, args);
        /// <inheritdoc cref="LoggerExtensions.LogError(ILogger, string?, object?[])"/>
        public void LogError(string message, params object?[] args) => _logger.LogError(message, args);
        /// <inheritdoc cref="LoggerExtensions.LogCritical(ILogger, string?, object?[])"/>
        public void LogCritical(string message, params object?[] args) => _logger.LogCritical(message, args);

    }

    /// <summary>
    /// LogEntry extension functions
    /// </summary>
    public static class LogEntryExtensions
    {
        /// <summary>
        /// Generates a string with the log information
        /// </summary>
        /// <returns>A string</returns>
        public static string ToStringExtended(this (LogLevel logLevel, EventId eventId, string category, string message) log)
        {
            return $"{log.logLevel}: [{log.category} ({log.eventId})] {log.message}";
        }
        /// <summary>
        /// Generates a string with XML/HTML format
        /// </summary>
        /// <returns>A XML/HTML formatted string</returns>
        public static string ToHTMLTag(this (LogLevel logLevel, EventId eventId, string category, string message) log)
        {
            return $"<{log.logLevel}>{HttpUtility.HtmlEncode(log.ToStringExtended())}</{log.logLevel}>";
        }
    }
}
