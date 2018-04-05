using k8s.Tests.Logging;
using System;
using System.Reactive.Disposables;
using System.Reflection;
using System.Threading;
using Microsoft.Extensions.Logging;
using Xunit;
using Xunit.Abstractions;

namespace k8s.Tests
{
    /// <summary>
    ///     The base class for test suites.
    /// </summary>
    public abstract class TestBase
        : IDisposable
    {
        /// <summary>
        ///     Create a new test-suite.
        /// </summary>
        /// <param name="testOutput">
        ///     Output for the current test.
        /// </param>
        protected TestBase(ITestOutputHelper testOutput)
        {
            if (testOutput == null)
                throw new ArgumentNullException(nameof(testOutput));

            // We *must* have a synchronisation context for the test, or we'll see random deadlocks.
            if (SynchronizationContext.Current == null)
            {
                SynchronizationContext.SetSynchronizationContext(
                    new SynchronizationContext()
                );
            }

            TestOutput = testOutput;
            LoggerFactory = new LoggerFactory().AddTestOutput(TestOutput, MinLogLevel);
            Log = LoggerFactory.CreateLogger("CurrentTest");

            // Ugly hack to get access to metadata for the current test.
            CurrentTest = (ITest)
                TestOutput.GetType()
                    .GetField("test", BindingFlags.NonPublic | BindingFlags.Instance)
                    .GetValue(TestOutput);

            Assert.True(CurrentTest != null, "Cannot retrieve current test from ITestOutputHelper.");

            Disposal.Add(
                Log.BeginScope("CurrentTest", CurrentTest.DisplayName)
            );
        }

        /// <summary>
        ///     Finaliser for <see cref="TestBase"/>.
        /// </summary>
        ~TestBase()
        {
            Dispose(false);
        }

        /// <summary>
        ///     Dispose of resources being used by the test suite.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        ///     Dispose of resources being used by the test suite.
        /// </summary>
        /// <param name="disposing">
        ///     Explicit disposal?
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                try
                {
                    Disposal.Dispose();
                }
                finally
                {
                    if (LoggerFactory is IDisposable loggerFactoryDisposal)
                        loggerFactoryDisposal.Dispose();

                    if (Log is IDisposable logDisposal)
                        logDisposal.Dispose();
                }
            }
        }

        /// <summary>
        ///     A <see cref="CompositeDisposable"/> representing resources used by the test.
        /// </summary>
        protected CompositeDisposable Disposal { get; } = new CompositeDisposable();

        /// <summary>
        ///     Output for the current test.
        /// </summary>
        protected ITestOutputHelper TestOutput { get; }

        /// <summary>
        ///     A <see cref="ITest"/> representing the current test.
        /// </summary>
        protected ITest CurrentTest { get; }

        /// <summary>
        ///     The logger for the current test.
        /// </summary>
        protected ILogger Log { get; }

        /// <summary>
        ///     The logger factory for the current test.
        /// </summary>
        protected ILoggerFactory LoggerFactory { get; }

        /// <summary>
        ///     The logging level for the current test.
        /// </summary>
        protected virtual LogLevel MinLogLevel => LogLevel.Information;

        /// <summary>
        ///     The test server logging level for the current test.
        /// </summary>
        protected virtual LogLevel MinServerLogLevel => LogLevel.Warning;
    }
}
