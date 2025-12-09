using Microsoft.Extensions.Logging;

namespace DSRemapper.Core.Loggers
{
    /// <summary>
    /// Delegate for logs events
    /// </summary>
    /// <param name="logLevel"><inheritdoc cref="ILogger.Log" path="/param[@name='logLevel']"/></param>
    /// <param name="eventId"><inheritdoc cref="ILogger.Log" path="/param[@name='eventId']"/></param>
    /// <param name="category"></param>
    /// <param name="message"></param>
    public delegate void LogEventHandler(LogLevel logLevel, EventId eventId, string category, string message);

    /// <summary>
    /// DSRemapper Event Logger
    /// </summary>
    public class EventLogger : ILogger
    {
        /// <summary>
        /// Event triggered when a log is created
        /// </summary>
        private readonly LogEventHandler? OnLog = null;
        private readonly object _lock = new();

        /// <summary>
        /// Creates an instance of <see cref="EventLogger"/>
        /// </summary>
        /// <param name="category"><inheritdoc cref="ILoggerProvider.CreateLogger(string)" path="/param[@name='categoryName']"/></param>
        /// <param name="callback">Delegate called when a log is send</param>
        public EventLogger(string category, LogEventHandler? callback)
        {
            Category = category;
            OnLog = callback;
        }

        private string Category { get; set; }
            
        /// <inheritdoc/>
        public IDisposable? BeginScope<TState>(TState state) where TState : notnull
        {
            return null;
        }

        /// <inheritdoc/>
        public bool IsEnabled(LogLevel logLevel) => true;

        /// <inheritdoc/>
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            lock (_lock)
            {
                OnLog?.Invoke(logLevel, eventId, Category, formatter(state, exception));
            }
        }
    }

    /// <summary>
    /// DSRemapper Event Logger Provider
    /// </summary>
    public class EventLoggerProvider : ILoggerProvider
    {
        private event LogEventHandler? OnLog = null;

        /// <param name="delegates">Delegates invoked when a log is send</param>
        public EventLoggerProvider(LogEventHandler[] delegates)
        {
            foreach (var deleg in delegates)
                OnLog += deleg;
        }

        /// <inheritdoc/>
        public ILogger CreateLogger(string categoryName) => new EventLogger(categoryName, OnLog);
        /// <inheritdoc/>
        public void Dispose() { }
    }
}
