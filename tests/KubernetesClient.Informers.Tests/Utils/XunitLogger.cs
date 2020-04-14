using System;
using System.Threading;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace k8s.Tests.Utils
{
    public class XunitLogger<T> : ILogger<T>, IDisposable
    {
        private static DateTime AppStartUp = DateTime.UtcNow;
        private ITestOutputHelper _output;

        public XunitLogger(ITestOutputHelper output)
        {
            _output = output;
        }


        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {

            var nowTotalMilliseconds = DateTime.Now.ToUniversalTime().Subtract(AppStartUp).TotalMilliseconds;
            _output.WriteLine($"{nowTotalMilliseconds} | {state} | ThreadID: {Thread.CurrentThread.ManagedThreadId}");
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return this;
        }

        public void Dispose()
        {
        }
    }
}
