using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using k8s.Informers.Cache;
using k8s.Informers.Notifications;

namespace k8s.Informers
{
    public static class ResourceObservableExtensions
    {


        public static IObservable<ResourceEvent<TResource>> WithResets<TResource>(this IObservable<ResourceEvent<TResource>> source, Func<List<ResourceEvent<TResource>>, IEnumerable<ResourceEvent<TResource>>> action)
        {
            return source.WithResets(action, e => e);
        }

        public static IObservable<TOut> WithResets<TResource, TOut>(this IObservable<ResourceEvent<TResource>> source, Func<List<ResourceEvent<TResource>>, IEnumerable<ResourceEvent<TResource>>> resetSelector, Func<ResourceEvent<TResource>, TOut> itemSelector)
        {
            return Observable.Create<TOut>(observer =>
                {
                    var resetBuffer = new List<ResourceEvent<TResource>>();

                    void FlushBuffer()
                    {
                        if (!resetBuffer.Any())
                        {
                            return;
                        }

                        foreach (var item in resetSelector(resetBuffer))
                        {
                            observer.OnNext(itemSelector(item));
                        }
                        resetBuffer.Clear();
                    }

                    void OnComplete()
                    {
                        FlushBuffer();
                        observer.OnCompleted();
                    }

                    void OnError(Exception e)
                    {
                        observer.OnError(e);
                    }

                    var upstreamSubscription = source
                        .Subscribe(notification =>
                        {
                            if (notification.EventFlags.HasFlag(EventTypeFlags.Reset))
                            {
                                resetBuffer.Add(notification);
                                if (!notification.EventFlags.HasFlag(EventTypeFlags.ResetEnd)) // continue buffering till we reach the end of list window
                                {
                                    return;
                                }
                            }

                            if (notification.EventFlags.HasFlag(EventTypeFlags.ResetEnd))
                            {
                                FlushBuffer();
                                return;
                            }

                            if (!notification.EventFlags.HasFlag(EventTypeFlags.Reset) && resetBuffer.Count > 0)
                            {
                                FlushBuffer();
                            }

                            observer.OnNext(itemSelector(notification));
                        }, OnError, OnComplete);
                    return StableCompositeDisposable.Create(upstreamSubscription, Disposable.Create(OnComplete));
                })
                .ObserveOn(Scheduler.Immediate);
        }

        /// <summary>
        ///     Synchronizes the specified cache with resource event stream such that cache is maintained up to date.
        /// </summary>
        /// <param name="source">The source sequence</param>
        /// <param name="cache">The cache to synchronize</param>
        /// <param name="keySelector">The key selector function</param>
        /// <typeparam name="TKey">The type of key</typeparam>
        /// <typeparam name="TResource">The type of resource</typeparam>
        /// <returns>Source sequence wrapped into <see cref="CacheSynchronized{T}" />, which allows downstream consumers to synchronize themselves with cache version</returns>
        public static IObservable<CacheSynchronized<ResourceEvent<TResource>>> SynchronizeCache<TKey, TResource>(
            this IObservable<ResourceEvent<TResource>> source,
            ICache<TKey, TResource> cache,
            Func<TResource, TKey> keySelector,
            Func<TResource, TResource, TResource> mapper = null)
        {
            if (mapper == null)
            {
                mapper = (oldResource, newResource) => newResource;
            }

            return Observable.Defer(() =>
            {
                long msgNum = 0;

                return source
                    .Do(_ => msgNum++)
                    .WithResets(events =>
                    {
                        var reset = events
                            .Select(x => x.Value)
                            .Where(x => x != null)
                            .ToDictionary(keySelector, x => x);

                        cache.Reset(reset);
                        cache.Version += events.Count;
                        return events;
                    }, notification =>
                    {
                        if (notification.EventFlags.HasFlag(EventTypeFlags.Reset))
                        {
                        }
                        else if (!notification.EventFlags.HasFlag(EventTypeFlags.Delete))
                        {
                            cache.Version++;
                            if (notification.Value != null)
                            {
                                var key = keySelector(notification.Value);
                                if (cache.TryGetValue(key, out var existing))
                                {
                                    notification = new ResourceEvent<TResource>(notification.EventFlags, mapper(existing, notification.Value), existing);
                                }
                                cache[keySelector(notification.Value)] = notification.Value;
                            }
                        }
                        else
                        {
                            cache.Remove(keySelector(notification.OldValue));
                        }

                        return new CacheSynchronized<ResourceEvent<TResource>>(msgNum, cache.Version, notification);
                    });
            });
        }


        public static IObservable<ResourceEvent<TResource>> ComputeMissedEventsBetweenResets<TKey, TResource>(this IObservable<ResourceEvent<TResource>> source, Func<TResource, TKey> keySelector, IEqualityComparer<TResource> comparer)
        {
            return Observable.Create<ResourceEvent<TResource>>(observer =>
            {
                var cache = new SimpleCache<TKey, TResource>();
                var cacheSynchronized = false;
                return source
                    .WithResets(resetBuffer =>
                    {
                        if (!cacheSynchronized)
                        {
                            return resetBuffer;
                        }

                        var cacheSnapshot = cache.Snapshot();
                        var newKeys = resetBuffer
                            .Where(x => x.Value != null)
                            .Select(x => keySelector(x.Value))
                            .ToHashSet();

                        var addedEntities = resetBuffer
                            .Select(x => x.Value)
                            .Where(x => x != null && !cacheSnapshot.ContainsKey(keySelector(x)))
                            .Select(x => x.ToResourceEvent(EventTypeFlags.Add | EventTypeFlags.Computed))
                            .ToList();
                        var addedKeys = addedEntities
                            .Select(x => keySelector(x.Value))
                            .ToHashSet();

                        var deletedEntities = cacheSnapshot
                            .Where(x => !newKeys.Contains(x.Key))
                            .Select(x => x.Value.ToResourceEvent(EventTypeFlags.Delete | EventTypeFlags.Computed))
                            .ToList();
                        var deletedKeys = deletedEntities
                            .Select(x => keySelector(x.Value))
                            .ToHashSet();

                        // we can only compute updates if we are given a proper comparer to determine equality between objects
                        // if not provided, will be sent downstream as just part of reset
                        var updatedEntities = new List<ResourceEvent<TResource>>();
                        if (comparer != null)
                        {
                            var previouslyKnownEntitiesInResetWindowKeys = cacheSnapshot
                                .Keys
                                .Intersect(resetBuffer.Select(x => keySelector(x.Value)));

                            updatedEntities = resetBuffer
                                .Where(x => previouslyKnownEntitiesInResetWindowKeys.Contains(keySelector(x.Value)))
                                .Select(x => x.Value) // stuff in buffer that also existed in cache (by key)
                                .Except(cacheSnapshot.Select(x => x.Value), comparer)
                                .Select(x => x.ToResourceEvent(EventTypeFlags.Modify | EventTypeFlags.Computed))
                                .ToList();
                        }

                        var updatedKeys = updatedEntities
                            .Select(x => keySelector(x.Value))
                            .ToHashSet();

                        var resetEntities = resetBuffer
                            .Select(x => x.Value)
                            .Where(x => x != null &&
                                        !addedKeys.Contains(keySelector(x)) &&
                                        !deletedKeys.Contains(keySelector(x)) &&
                                        !updatedKeys.Contains(keySelector(x)))
                            .ToReset()
                            .ToList();

                        return deletedEntities
                            .Union(addedEntities)
                            .Union(updatedEntities)
                            .Union(resetEntities);
                    })
                    .SynchronizeCache(cache, keySelector)
                    .Do(msg => { cacheSynchronized = true; })
                    .Select(x => x.Value)
                    .ObserveOn(Scheduler.Immediate)
                    .Subscribe(observer);
            });
        }

        /// <summary>
        ///     Injects a <see cref="ResourceEvent{T}" /> of type <see cref="ResourceStreamType.Sync" /> into the observable for each item produced
        ///     by the <see cref="ResourceStreamType.List" /> operation from <paramref name="source" />
        /// </summary>
        /// <param name="source">The source sequence that will have sync messages appended</param>
        /// <param name="timeSpan">The timespan interval at which the messages should be produced</param>
        /// <typeparam name="T">The type of resource</typeparam>
        /// <returns>Original sequence with resync applied</returns>
        public static IObservable<ResourceEvent<T>> Resync<T>(this IObservable<ResourceEvent<T>> source, TimeSpan timeSpan, IScheduler scheduler = null)
        {
            scheduler ??= DefaultScheduler.Instance;
            return Observable.Create<ResourceEvent<T>>(observer =>
            {
                var timerSubscription = Observable
                    .Interval(timeSpan, scheduler)
                    .SelectMany(_ => source
                        .TakeUntil(x => x.EventFlags.HasFlag(EventTypeFlags.ResetEnd))
                        .Do(x =>
                        {
                            if (!x.EventFlags.HasFlag(EventTypeFlags.Reset))
                            {
                                throw new InvalidOperationException("Resync was applied to an observable sequence that does not issue a valid List event block when subscribed to");
                            }
                        })
                        .Select(x => x.Value.ToResourceEvent(EventTypeFlags.Sync)))
                    .Subscribe(observer);
                // this ensures that both timer and upstream subscription is closed when subscriber disconnects
                var sourceSubscription = source.Subscribe(
                    observer.OnNext,
                    observer.OnError,
                    () =>
                    {
                        observer.OnCompleted();
                        timerSubscription.Dispose();
                    });
                return StableCompositeDisposable.Create(timerSubscription, sourceSubscription);
            });
        }

        /// <summary>
        ///     Wraps an instance of <see cref="IInformer{TResource,TOptions}" /> as <see cref="IInformer{TResource}" /> by using the same
        ///     set of <see cref="TOptions" /> for every subscription
        /// </summary>
        /// <param name="optionedInformer">The original instance of <see cref="IInformer{TResource,TOptions}" /></param>
        /// <param name="options">The options to use</param>
        /// <typeparam name="TResource">The type of resource</typeparam>
        /// <typeparam name="TOptions"></typeparam>
        /// <returns></returns>
        public static IInformer<TResource> WithOptions<TResource, TOptions>(this IInformer<TResource, TOptions> optionedInformer, TOptions options) =>
            new WrappedOptionsInformer<TResource, TOptions>(optionedInformer, options);

        private class WrappedOptionsInformer<TResource, TOptions> : IInformer<TResource>
        {
            private readonly IInformer<TResource, TOptions> _informer;
            private readonly TOptions _options;

            public WrappedOptionsInformer(IInformer<TResource, TOptions> informer, TOptions options)
            {
                _informer = informer;
                _options = options;
            }

            public IObservable<ResourceEvent<TResource>> GetResource(ResourceStreamType type) => _informer.GetResource(type, _options);
        }
    }
}
