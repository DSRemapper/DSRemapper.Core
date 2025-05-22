using Microsoft.Extensions.Logging;
using System.Text.RegularExpressions;

namespace DSRemapper.Core.Loggers
{
    /// <summary>
    /// A simple file logger service
    /// </summary>
    public class FileLogger(string category, string fileName, LogLevel minLevel) : ILogger
    {
        private readonly string fileName = fileName;
        private readonly LogLevel minLevel = minLevel;
        private static readonly object _lock = new();
        private string Category { get; set; } = category;

        /// <inheritdoc/>
        public IDisposable? BeginScope<TState>(TState state) where TState : notnull
        {
            return null;
        }

        /// <inheritdoc/>
        public bool IsEnabled(LogLevel logLevel) => logLevel >= minLevel;

        /// <inheritdoc/>
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            lock (_lock)
            {
                File.AppendAllText(fileName, $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] {logLevel}: [{Category} ({eventId})] {formatter(state, exception)}\n");
            }
        }
    }

    /// <summary>
    /// DSRemapper File Logger Provider
    /// </summary>
    public class FileLoggerProvider : ILoggerProvider
    {
        private readonly LogLevel minLevel;
        private readonly FileLoggerConfiguration config;

        private readonly string fileName;
        private readonly Regex rgFilter;
        /// <summary>
        /// Creates an <see cref="FileLoggerProvider"/> with the configuration provided
        /// </summary>
        /// <param name="config">The configuration for the logger service</param>
        /// <param name="minimumLevel">The minimum log level that can be written to the file</param>
        public FileLoggerProvider(FileLoggerConfiguration config, LogLevel minimumLevel = LogLevel.Information)
        {
            minLevel = minimumLevel;

            this.config = config;
            rgFilter = new(@$"{config.FileIdentifier}(\d{{8}}-\d{{6}})");
            fileName = GetNewFileName();
            string repeated = "";
            int i = 0;
            if (!Directory.Exists(config.LogFolder))
                Directory.CreateDirectory(config.LogFolder);
            while (File.Exists(fileName + repeated))
                repeated = $" ({++i})";

            fileName += repeated + ".txt";
            File.Create(fileName).Close();
            DeleteOldFiles();
        }
        private string GetNewFileName() => Path.Combine(config.LogFolder,
            $"{config.FileIdentifier}{DateTime.Now:yyyyMMdd-HHmmss}");

        private void DeleteOldFiles()
        {
            if (config.MaxFiles <= 0)
                return;

            List<FileInfo> files = [.. Directory.GetFiles(config.LogFolder, "*.*")
                .Where(f => rgFilter.IsMatch(Path.GetFileNameWithoutExtension(f)))
                .Select(f => new FileInfo(f)).OrderBy(f => f.CreationTime)];

            int maxFiles = config.MaxFiles;
            while (files.Count > maxFiles)
            {
                files[0].Delete();
                files.RemoveAt(0);
            }
        }
        /// <inheritdoc/>
        public ILogger CreateLogger(string categoryName) => new FileLogger(categoryName, fileName, minLevel);
        /// <inheritdoc/>
        public void Dispose() { }
    }

    /// <summary>
    /// Configuration structure for <see cref="FileLogger"/>
    /// </summary>
    public struct FileLoggerConfiguration
    {
        /// <summary>
        /// A file indentifier for the <see cref="FileLogger"/>. The <see cref="FileLogger"/> will add the time and date of the creation concatenated after this string.
        /// </summary>
        public string FileIdentifier { get; private set; }
        /// <summary>
        /// The folder where the log files will be created.
        /// </summary>
        public string LogFolder { get; private set; }
        /// <summary>
        /// The maximum number of files (corresponding to <see cref="FileIdentifier"/> parameter) that will be retained.
        /// </summary>
        public int MaxFiles { get; private set; }
        /// <summary>
        /// Creates an instance of <see cref="FileLoggerConfiguration"/>
        /// </summary>
        /// <param name="fileName"><inheritdoc cref="FileIdentifier" path="/summary"/></param>
        /// <param name="folderName"><inheritdoc cref="LogFolder" path="/summary"/></param>
        /// <param name="maxFiles"><inheritdoc cref="MaxFiles" path="/summary"/></param>
        public FileLoggerConfiguration(string fileName = "log-", string folderName = "Logs", int maxFiles = 5)
        {
            FileIdentifier = fileName;
            LogFolder = folderName;
            MaxFiles = maxFiles;
        }
    }
}
