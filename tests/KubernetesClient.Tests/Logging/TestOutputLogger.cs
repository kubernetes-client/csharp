using Microsoft.Extensions.Logging;
using System;
using System.Reactive.Disposables;
using Xunit.Abstractions;

namespace k8s.Tests.Logging
{
    /// <summary>
    ///     An implementation of <see cref="ILogger"/> that writes to the output of the current Xunit test.
    /// </summary>
    internal sealed class TestOutputLogger
        : ILogger
    {
        /// <summary>
        ///     Create a new <see cref="TestOutputLogger"/>.
        /// </summary>
        /// <param name="testOutput">
        ///     The output for the current test.
        /// </param>
        /// <param name="loggerCategory">
        ///     The logger's category name.
        /// </param>
        /// <param name="minLogLevel">
        ///     The logger's minimum log level.
        /// </param>
        public TestOutputLogger(ITestOutputHelper testOutput, string loggerCategory, LogLevel minLogLevel)
        {
            if (testOutput == null)
            {
                throw new ArgumentNullException(nameof(testOutput));
            }

            if (string.IsNullOrWhiteSpace(loggerCategory))
            {
                throw new ArgumentException(
                    "Argument cannot be null, empty, or entirely composed of whitespace: 'loggerCategory'.",
                    nameof(loggerCategory));
            }

            TestOutput = testOutput;
            LoggerCategory = loggerCategory;
            MinLogLevel = minLogLevel;
        }

        /// <summary>
        ///     The output for the current test.
        /// </summary>
        public ITestOutputHelper TestOutput { get; }

        /// <summary>
        ///     The logger's category name.
        /// </summary>
        public string LoggerCategory { get; }

        /// <summary>
        ///     The logger's minimum log level.
        /// </summary>
        public LogLevel MinLogLevel { get; }

        /// <summary>
        ///     Emit a log entry.
        /// </summary>
        /// <param name="level">
        ///     The log entry's level.
        /// </param>
        /// <param name="eventId">
        ///     The log entry's associated event Id.
        /// </param>
        /// <param name="state">
        ///     The log entry to be written. Can be also an object.
        /// </param>
        /// <param name="exception">
        ///     The exception (if any) related to the log entry.
        /// </param>
        /// <param name="formatter">
        ///     A function that creates a <c>string</c> log message from the <paramref name="state"/> and <paramref name="exception"/>.
        /// </param>
        public void Log<TState>(LogLevel level, EventId eventId, TState state, Exception exception,
            Func<TState, Exception, string> formatter)
        {
            if (formatter == null)
            {
                throw new ArgumentNullException(nameof(formatter));
            }

            TestOutput.WriteLine(string.Format("[{0}] {1}: {2}",
                level,
                LoggerCategory,
                formatter(state, exception)));

            if (exception != null)
            {
                TestOutput.WriteLine(
                    exception.ToString());
            }
        }

        /// <summary>
        ///     Check if the given <paramref name="logLevel"/> is enabled.
        /// </summary>
        /// <param name="logLevel">
        ///     The level to be checked.
        /// </param>
        /// <returns>
        ///     <c>true</c> if enabled; otherwise, <c>false</c>.
        /// </returns>
        public bool IsEnabled(LogLevel logLevel) => logLevel >= MinLogLevel;

        /// <summary>
        ///     Begin a logical operation scope.
        /// </summary>
        /// <param name="state">
        ///     An identifier for the scope.
        /// </param>
        /// <returns>
        ///     An <see cref="IDisposable"/> that ends the logical operation scope when disposed.
        /// </returns>
        public IDisposable BeginScope<TState>(TState state) => Disposable.Empty;
    }
}
