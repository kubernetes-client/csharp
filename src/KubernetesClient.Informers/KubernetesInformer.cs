using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using k8s.Informers.FaultTolerance;
using k8s.Informers.Notifications;
using k8s.Models;
using Microsoft.Rest.TransientFaultHandling;
using RetryPolicy = k8s.Informers.FaultTolerance.RetryPolicy;

namespace k8s.Informers
{
    /// <summary>
    ///     An implementation of Kubernetes informer that talks to Kubernetes API Server
    /// </summary>
    /// <typeparam name="TResource">The type of Kubernetes resource</typeparam>
    public class KubernetesInformer<TResource> : IKubernetesInformer<TResource> where TResource : IKubernetesObject
    {
        private readonly IKubernetes _kubernetes;
        private readonly Func<bool> _restartOnCompletion;
        private readonly RetryPolicy _retryPolicy;

        public KubernetesInformer(IKubernetes kubernetes, RetryPolicy retryPolicy = null) : this(kubernetes, retryPolicy, () => true)
        {
        }

        public KubernetesInformer(IKubernetes kubernetes, RetryPolicy retryPolicy, Func<bool> restartOnCompletion)
        {
            _kubernetes = kubernetes;
            _restartOnCompletion = restartOnCompletion;
            _retryPolicy = retryPolicy ?? DefaultRetryPolicy;
        }

        private static RetryPolicy DefaultRetryPolicy => new RetryPolicy(
            (exception, retryAttempt) => exception.IsTransient(),
            retryAttempt => TimeSpan.FromSeconds(Math.Min(Math.Pow(2, retryAttempt), 30)));

        public IObservable<ResourceEvent<TResource>> GetResource(ResourceStreamType type) => GetResource(type, KubernetesInformerOptions.Default);

        public IObservable<ResourceEvent<TResource>> GetResource(ResourceStreamType type, KubernetesInformerOptions options)
        {
            return new KubernetesInformerEmitter(this, options, type).GetObservable();
        }

        private class KubernetesInformerEmitter
        {
            private readonly KubernetesInformerOptions _options;
            private readonly KubernetesInformer<TResource> _parent;
            private readonly ResourceStreamType _type;
            private string _resourceVersion;

            public KubernetesInformerEmitter(KubernetesInformer<TResource> parent, KubernetesInformerOptions options, ResourceStreamType type)
            {
                _parent = parent;
                _options = options;
                _type = type;
            }

            public IObservable<ResourceEvent<TResource>> GetObservable()
            {
                var result = Observable.Empty<ResourceEvent<TResource>>();
                if (_type.HasFlag(ResourceStreamType.List))
                {
                    result = result.Concat(List());
                }

                if (_type.HasFlag(ResourceStreamType.Watch))
                {
                    result = result.Concat(Watch());
                }

                return result;
            }

            private IObservable<ResourceEvent<TResource>> List()
            {
                return Observable.Create<ResourceEvent<TResource>>(async (observer, cancellationToken) =>
                    {
                        var response = await _parent._kubernetes.ListWithHttpMessagesAsync<TResource>(
                            _options.Namespace,
                            resourceVersion: _resourceVersion,
                            labelSelector: _options.LabelSelector,
                            cancellationToken: cancellationToken).ConfigureAwait(false);
                        if (!response.Response.IsSuccessStatusCode)
                        {
                            throw new HttpRequestWithStatusException("Web server replied with error code") { StatusCode = response.Response.StatusCode };
                        }

                        var listKubernetesObject = response.Body;
                        _resourceVersion = listKubernetesObject.Metadata.ResourceVersion;
                        var items = listKubernetesObject.Items ?? new List<TResource>();
                        foreach (var item in items.ToReset(_type == ResourceStreamType.ListWatch))
                        {
                            if (cancellationToken.IsCancellationRequested)
                            {
                                break;
                            }
                            observer.OnNext(item);
                        }
                    })
                    .WithRetryPolicy(_parent._retryPolicy);
            }

            private IObservable<ResourceEvent<TResource>> Watch()
            {
                return Observable.Create<ResourceEvent<TResource>>(async (observer, cancellationToken) =>
                    {
                        var result = await _parent._kubernetes.ListWithHttpMessagesAsync<TResource>(
                            _options.Namespace,
                            watch: true,
                            allowWatchBookmarks: true,
                            resourceVersion: _resourceVersion,
                            labelSelector: _options.LabelSelector,
                            cancellationToken: cancellationToken
                        ).ConfigureAwait(false);
                        if (!result.Response.IsSuccessStatusCode)
                        {
                            throw new HttpRequestWithStatusException("Web server replied with error code") { StatusCode = result.Response.StatusCode };
                        }
                        return Task.FromResult(result)
                            .Watch()
                            .SelectMany(x => // this is not a one to one mapping as some events cause side effects but don't propagate, so we need SelectMany
                            {
                                if (x.Object is IMetadata<V1ObjectMeta> status && status.Metadata.ResourceVersion != null)
                                {
                                    _resourceVersion = status.Metadata.ResourceVersion;
                                }
                                switch (x.Type)
                                {
                                    case WatchEventType.Added:
                                        return new[] { x.Object.ToResourceEvent(EventTypeFlags.Add) };
                                    case WatchEventType.Deleted:
                                        return new[] { x.Object.ToResourceEvent(EventTypeFlags.Delete) };
                                    case WatchEventType.Modified:
                                        return new[] { x.Object.ToResourceEvent(EventTypeFlags.Modify) };
                                    case WatchEventType.Bookmark:
                                        // we're just updating resource version
                                        break;
                                    case WatchEventType.Error:
                                    default:
                                        if (x.Object is V1Status error)
                                        {
                                            throw new KubernetesException(error);
                                        }

                                        throw new KubernetesException($"Received unknown error in watch: {x.Object}");
                                }

                                return Enumerable.Empty<ResourceEvent<TResource>>();
                            })
                            .Select(x => x)
                            // watch should never "complete" on it's own unless there's a critical exception, except in testing scenarios
                            .Concat(_parent._restartOnCompletion() ? Observable.Defer(Watch) : Observable.Empty<ResourceEvent<TResource>>())
                            .Subscribe(observer);
                    })
                    .Catch<ResourceEvent<TResource>, Exception>(exception =>
                    {
                        // handle case when we tried rewatching by specifying resource version to resume after disconnect,
                        // but resource is too stale - should try to resubscribe from scratch
                        if (exception is HttpRequestWithStatusException httpException && httpException.StatusCode == HttpStatusCode.Gone && _resourceVersion != null)
                        {
                            // we tried resuming but failed, restart from scratch
                            _resourceVersion = null;
                            return GetObservable();
                        }

                        return Observable.Throw<ResourceEvent<TResource>>(exception);
                    })
                    .WithRetryPolicy(_parent._retryPolicy);
            }
        }
    }
}
