using System;
using k8s.Informers.Cache;
using k8s.Informers.Notifications;
using k8s.Models;
using Microsoft.Extensions.Logging;

namespace k8s.Informers
{
    /// <summary>
    ///     Opens a single connection to API server with per unique <see cref="KubernetesInformerOptions" />
    ///     and attaches 1 or more internal subscriber to it. The connection is automatically opened if there is
    ///     at least one subscriber and closes if there are none
    /// </summary>
    /// <typeparam name="TResource">The type of resource to monitor</typeparam>
    public class SharedKubernetesInformer<TResource> :
        SharedOptionsInformer<TResource, KubernetesInformerOptions>,
        IKubernetesInformer<TResource>
        where TResource : IKubernetesObject, IMetadata<V1ObjectMeta>
    {
        public SharedKubernetesInformer(KubernetesInformer<TResource> masterInformer, ILoggerFactory loggerFactory)
            : base(masterInformer, SharedKubernetesInformerFactory(loggerFactory, GetVersionPartitionedCacheFactory()))
        {
        }

        public SharedKubernetesInformer(KubernetesInformer<TResource> masterInformer, Func<ICache<string, TResource>> cacheFactory, ILoggerFactory loggerFactory)
            : base(masterInformer, SharedKubernetesInformerFactory(loggerFactory, cacheFactory))
        {
        }

        /// <inheritdoc cref="IKubernetesInformer{TResource}" />
        public IObservable<ResourceEvent<TResource>> GetResource(ResourceStreamType type) => base.GetResource(type, KubernetesInformerOptions.Default);

        private static Func<ICache<string, TResource>> GetVersionPartitionedCacheFactory()
        {
            var partitionedSharedCache = new VersionPartitionedSharedCache<string, TResource, string>(x => x.Metadata.Name, x => x.Metadata.ResourceVersion);
            return () => partitionedSharedCache.CreatePartition();
        }

        private static Func<IInformer<TResource>, IInformer<TResource>> SharedKubernetesInformerFactory(ILoggerFactory loggerFactory, Func<ICache<string, TResource>> cacheFactory) =>
            masterInformer => new SharedInformer<string, TResource>(
                masterInformer,
                loggerFactory.CreateLogger<ILogger<SharedInformer<string, TResource>>>(),
                x => x.Metadata.Name,
                cacheFactory());
    }
}
