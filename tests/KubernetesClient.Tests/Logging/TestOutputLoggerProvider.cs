using Microsoft.Extensions.Logging;
using System;
using Xunit.Abstractions;

namespace k8s.Tests.Logging
{
    /// <summary>
    ///     Logger provider for logging to Xunit test output.
    /// </summary>
    internal sealed class TestOutputLoggerProvider
        : ILoggerProvider
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TestOutputLoggerProvider"/> class.
        ///     Create a new <see cref="TestOutputLoggerProvider"/>.
        /// </summary>
        /// <param name="testOutput">
        ///     The output for the current test.
        /// </param>
        /// <param name="minLogLevel">
        ///     The logger's minimum log level.
        /// </param>
        public TestOutputLoggerProvider(ITestOutputHelper testOutput, LogLevel minLogLevel)
        {
            if (testOutput == null)
            {
                throw new ArgumentNullException(nameof(testOutput));
            }

            TestOutput = testOutput;
            MinLogLevel = minLogLevel;
        }

        /// <summary>
        ///     Dispose of resources being used by the logger provider.
        /// </summary>
        public void Dispose()
        {
        }

        /// <summary>
        ///     The output for the current test.
        /// </summary>
        private ITestOutputHelper TestOutput { get; }

        /// <summary>
        ///     The logger's minimum log level.
        /// </summary>
        public LogLevel MinLogLevel { get; }

        /// <summary>
        ///     Create a new logger.
        /// </summary>
        /// <param name="categoryName">
        ///     The logger category name.
        /// </param>
        /// <returns>
        ///     The logger, as an <see cref="ILogger"/>.
        /// </returns>
        public ILogger CreateLogger(string categoryName) => new TestOutputLogger(TestOutput, categoryName, MinLogLevel);
    }
}
