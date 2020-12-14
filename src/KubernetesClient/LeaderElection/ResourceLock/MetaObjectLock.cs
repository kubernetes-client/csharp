using System;
using System.Threading;
using System.Threading.Tasks;
using k8s.Models;
using Microsoft.Rest;
using Newtonsoft.Json;

namespace k8s.LeaderElection.ResourceLock
{
    public abstract class MetaObjectLock<T> : ILock
        where T : class, IMetadata<V1ObjectMeta>, new()
    {
        private IKubernetes client;
        private string ns;
        private string name;
        private string identity;
        private T metaObjCache;

        protected MetaObjectLock(IKubernetes client, string @namespace, string name, string identity)
        {
            this.client = client;
            ns = @namespace;
            this.name = name;
            this.identity = identity;
        }

        private const string LeaderElectionRecordAnnotationKey = "control-plane.alpha.kubernetes.io/leader";

        public string Identity => identity;

        public async Task<LeaderElectionRecord> GetAsync(CancellationToken cancellationToken = default)
        {
            var obj = await ReadMetaObjectAsync(client, name, ns, cancellationToken).ConfigureAwait(false);
            var recordRawStringContent = obj.GetAnnotation(LeaderElectionRecordAnnotationKey);

            if (string.IsNullOrEmpty(recordRawStringContent))
            {
                return new LeaderElectionRecord();
            }

            var record =
                JsonConvert.DeserializeObject<LeaderElectionRecord>(
                    recordRawStringContent,
                    client.DeserializationSettings);

            Interlocked.Exchange(ref metaObjCache, obj);
            return record;
        }

        protected abstract Task<T> ReadMetaObjectAsync(IKubernetes client, string name, string namespaceParameter, CancellationToken cancellationToken);

        public async Task<bool> CreateAsync(LeaderElectionRecord record, CancellationToken cancellationToken = default)
        {
            var metaObj = new T
            {
                Metadata = new V1ObjectMeta() { Name = name, NamespaceProperty = ns },
            };

            metaObj.SetAnnotation(
                LeaderElectionRecordAnnotationKey,
                JsonConvert.SerializeObject(record, client.SerializationSettings));

            try
            {
                var createdObj = await CreateMetaObjectAsync(client, metaObj, ns, cancellationToken)
                    .ConfigureAwait(false);

                Interlocked.Exchange(ref metaObjCache, createdObj);
                return true;
            }
            catch (HttpOperationException)
            {
                // ignore
            }

            return false;
        }

        protected abstract Task<T> CreateMetaObjectAsync(IKubernetes client, T obj, string namespaceParameter, CancellationToken cancellationToken);


        public async Task<bool> UpdateAsync(LeaderElectionRecord record, CancellationToken cancellationToken = default)
        {
            var metaObj = Interlocked.CompareExchange(ref metaObjCache, null, null);
            if (metaObj == null)
            {
                throw new InvalidOperationException("endpoint not initialized, call get or create first");
            }

            metaObj.SetAnnotation(
                LeaderElectionRecordAnnotationKey,
                JsonConvert.SerializeObject(record, client.DeserializationSettings));

            try
            {
                var replacedObj = await ReplaceMetaObjectAsync(client, metaObjCache, name, ns, cancellationToken).ConfigureAwait(false);

                Interlocked.Exchange(ref metaObjCache, replacedObj);
                return true;
            }
            catch (HttpOperationException)
            {
                // ignore
            }

            return false;
        }

        protected abstract Task<T> ReplaceMetaObjectAsync(IKubernetes client, T obj, string name, string namespaceParameter, CancellationToken cancellationToken);

        public string Describe()
        {
            return $"{ns}/{name}";
        }
    }
}
