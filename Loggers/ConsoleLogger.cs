using Microsoft.Extensions.Logging;

namespace DSRemapper.Core.Loggers
{
    /// <summary>
    /// DSRemapper Formatted Console Logger
    /// </summary>
    /// <param name="category"><inheritdoc cref="ILoggerProvider.CreateLogger(string)" path="/param[@name='categoryName']"/></param>
    public class ConsoleLogger(string category) : ILogger
    {
        // Make this a thread for safe threading instead of an static that blocks all threads.
        // For now this should work considering that the Device Scanner
        // runs once per second and doesn't log if there are no changes.
        private static readonly object _lock = new();
        private string Category { get; set; } = category;
            
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
                ConsoleColor prevColor = Console.ForegroundColor;
                Console.ForegroundColor = logLevel switch
                {
                    LogLevel.Information => ConsoleColor.DarkGreen,
                    LogLevel.Warning => ConsoleColor.Yellow,
                    LogLevel.Error => ConsoleColor.Red,
                    LogLevel.Critical => ConsoleColor.DarkRed,
                    _ => ConsoleColor.White,
                };
                Console.Write($"{logLevel}:");
                Console.ForegroundColor = prevColor;
                Console.WriteLine($" [{Category} ({eventId})] {formatter(state, exception)}");
            }
        }
    }

    /// <summary>
    /// DSRemapper Formatted Console Logger Provider
    /// </summary>
    public class ConsoleLoggerProvider : ILoggerProvider
    {
        /// <inheritdoc cref="ILoggerProvider"/>
        public ILogger CreateLogger(string categoryName) => new ConsoleLogger(categoryName);
        /// <inheritdoc cref="Dispose"/>
        public void Dispose() { }
    }
}
