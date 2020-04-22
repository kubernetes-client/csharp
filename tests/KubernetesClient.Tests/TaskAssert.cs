using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace k8s.Tests
{
    static class TaskAssert
    {
        public static void NotCompleted(Task task, string message = "Task should not be completed")
        {
            Assert.False(task.IsCompleted, message);
        }

        public static async Task Completed(Task task, TimeSpan timeout, string message = "Task timed out")
        {
            var timeoutTask = Task.Delay(
                TimeSpan.FromMilliseconds(1000)
            );

            var completedTask = await Task.WhenAny(task, timeoutTask);
            Assert.True(ReferenceEquals(task, completedTask), message);

            await completedTask;
        }

        public static async Task<T> Completed<T>(Task<T> task, TimeSpan timeout, string message = "Task timed out")
        {
            var timeoutTask =
                Task.Delay(
                    TimeSpan.FromMilliseconds(1000)
                )
                .ContinueWith(
                    completedTimeoutTask => default(T) // Value is never returned, but we need a task of the same result type in order to use Task.WhenAny.
                );

            var completedTask = await Task.WhenAny(task, timeoutTask);
            Assert.True(ReferenceEquals(task, completedTask), message);

            return await completedTask;
        }
    }
}
