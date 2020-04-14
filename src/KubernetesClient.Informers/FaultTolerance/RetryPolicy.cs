using System;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace k8s.Informers.FaultTolerance
{
    /// <summary>
    ///     Specifies retry policy to apply to a Task or Observable
    /// </summary>
    /// <remarks>
    ///     This class could potentially be replaced by Polly, but currently Polly doesn't work with observables (need access to policy rules in the builder, which are internal atm).
    /// </remarks>
    public class RetryPolicy
    {
        /// <summary>
        ///     No retry policy should be applied
        /// </summary>
        public static readonly RetryPolicy None = new RetryPolicy((_, __) => false, _ => TimeSpan.Zero);

        /// <param name="shouldRetry">A delegate which accepts exception being handled and retry attempt, and returns if retry should be attempted </param>
        /// <param name="retryDelay">A delegate that accepts retry attempt and returns delay till next retry attempt</param>
        public RetryPolicy(Func<Exception, int, bool> shouldRetry, Func<int, TimeSpan> retryDelay)
        {
            ShouldRetry = shouldRetry;
            RetryDelay = retryDelay;
        }

        internal Func<Exception, int, bool> ShouldRetry { get; }
        internal Func<int, TimeSpan> RetryDelay { get; }

        /// <summary>
        ///     Executes a given task while applying the specified retry policy
        /// </summary>
        /// <param name="action">Delegate for the task to execute</param>
        /// <typeparam name="TResult">Return type of the Task</typeparam>
        /// <returns>Task result</returns>
        public async Task<TResult> ExecuteAsync<TResult>(Func<Task<TResult>> action)
        {
            var retryCount = 1;
            while (true)
                try
                {
                    return await action().ConfigureAwait(false); ;
                }
                catch (Exception e)
                {
                    if (!ShouldRetry(e, retryCount))
                    {
                        throw;
                    }
                    retryCount++;
                    await Task.Delay(RetryDelay(retryCount)).ConfigureAwait(false); ;
                }
        }
    }

    public static class RetryPolicyExtensions
    {
        /// <summary>
        ///     Catches any exceptions in observable sequence and handles them with the specified retry policy.
        ///     Resubscribes to the observable if the policy determines that retry should be attempted
        /// </summary>
        /// <param name="observable">The source observable</param>
        /// <param name="retryPolicy">The retry policy to apply</param>
        /// <typeparam name="T">The type of the observable</typeparam>
        /// <returns>Original observable wrapped in retry policy</returns>
        public static IObservable<T> WithRetryPolicy<T>(this IObservable<T> observable, RetryPolicy retryPolicy)
        {
            var retryCounter = 1;
            return observable.Catch<T, Exception>(exception =>
            {
                if (!retryPolicy.ShouldRetry(exception, retryCounter))
                {
                    return Observable.Throw<T>(exception);
                }
                retryCounter++;
                return observable.DelaySubscription(retryPolicy.RetryDelay(retryCounter));
            });
        }
    }
}
