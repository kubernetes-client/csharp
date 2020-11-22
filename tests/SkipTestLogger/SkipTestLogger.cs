using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Client;

namespace k8s.E2E
{
    // This TestLogger is to log test cases skipped by accident
    // when E2E test runs in github action

    [FriendlyName("SkipTestLogger")]
    [ExtensionUri("logger://Microsoft/TestPlatform/SkipTestLogger/v1")]
    public class SkipTestLogger : ITestLoggerWithParameters
    {
        public void Initialize(TestLoggerEvents events, Dictionary<string, string> parameters)
        {
            if (parameters != null && parameters.TryGetValue("file", out var file))
            {
                InnerInitialize(events, file);
            }
            else
            {
                throw new ArgumentNullException("file", "log file path must be set");
            }
        }

        private static void InnerInitialize(TestLoggerEvents events, string file)
        {
            if (events == null)
            {
                throw new ArgumentNullException(nameof(events));
            }

            Console.WriteLine($"using {file} for skipped test case log");
            events.TestResult += (sender, args) =>
            {
                using (var w = File.AppendText(file))
                {
                    if (args.Result.Outcome == TestOutcome.Skipped)
                    {
                        w.WriteLine(args.Result.DisplayName);
                    }
                }
            };
        }

        public void Initialize(TestLoggerEvents events, string testRunDirectory)
        {
            InnerInitialize(events, Path.Combine(testRunDirectory, "skip.log"));
        }
    }
}
