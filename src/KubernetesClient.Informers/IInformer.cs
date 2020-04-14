using System;
using k8s.Informers.Notifications;

namespace k8s.Informers
{
    /// <summary>
    ///     Provides observable abstraction over collections of resource of type <typeparamref name="TResource" /> which support List/Watch semantics
    /// </summary>
    /// <typeparam name="TResource">The type of resource</typeparam>
    public interface IInformer<TResource>
    {
        /// <summary>
        ///     Exposes an Observable stream over a resource of a particular type
        /// </summary>
        /// <remarks>
        ///     Message stream semantics are as following
        ///     - When subscription is first established and <see cref="ResourceStreamType" /> is has <see cref="ResourceStreamType.List" /> flag set
        ///     the first batch of messages that will be sent when subscription is opened the current state of all the objects being monitored.
        ///     This batch is referred to as "resource list reset".
        ///     - Each message in reset event will be of type <see cref="EventTypeFlags.Reset" />
        ///     - The boundaries of the reset event will be marked with <see cref="EventTypeFlags.ResetStart" /> and <see cref="EventTypeFlags.ResetEnd" />
        ///     - If there are no objects in a reset list event, and the <paramref name="type" /> has a flag <see cref="ResourceStreamType.Watch" /> set,
        ///     message with flag <see cref="EventTypeFlags.ResetEmpty" /> is used to mark the end of List operation and start of Watch
        /// </remarks>
        /// <param name="type">Observable type</param>
        /// <returns>Observable stream for resources of a particular type</returns>
        IObservable<ResourceEvent<TResource>> GetResource(ResourceStreamType type);
    }

    /// <summary>
    ///     Provides observable abstraction over collections of resource of type <typeparamref name="TResource" /> which support List/Watch semantics,
    ///     and support subscriptions with type <typeparamref name="TOptions" />
    /// </summary>
    /// <typeparam name="TResource">The type of resource</typeparam>
    /// <typeparam name="TOptions">The type of options</typeparam>
    public interface IInformer<TResource, in TOptions>
    {
        /// <summary>
        ///     Exposes an Observable stream over a resource of a particular type
        /// </summary>
        /// <remarks>
        ///     Message stream semantics are as following
        ///     - When subscription is first established and <see cref="ResourceStreamType" /> is has <see cref="ResourceStreamType.List" /> flag set
        ///     the first batch of messages that will be sent when subscription is opened the current state of all the objects being monitored.
        ///     This batch is referred to as "resource list reset".
        ///     - Each message in reset event will be of type <see cref="EventTypeFlags.Reset" />
        ///     - The boundaries of the reset event will be marked with <see cref="EventTypeFlags.ResetStart" /> and <see cref="EventTypeFlags.ResetEnd" />
        ///     - If there are no objects in a reset list event, and the <paramref name="type" /> has a flag <see cref="ResourceStreamType.Watch" /> set,
        ///     message with flag <see cref="EventTypeFlags.ResetEmpty" /> is used to mark the end of List operation and start of Watch
        /// </remarks>
        /// <param name="type">Observable type</param>
        /// <param name="options"></param>
        /// <returns></returns>
        IObservable<ResourceEvent<TResource>> GetResource(ResourceStreamType type, TOptions options);
    }
}
