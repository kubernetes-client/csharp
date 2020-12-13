using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using k8s.Models;
using Microsoft.Rest;
using Newtonsoft.Json;

namespace k8s.LeaderElection.ResourceLock
{
    public class EndpointsLock : ILock
    {
        private const string LeaderElectionRecordAnnotationKey = "control-plane.alpha.kubernetes.io/leader";

        private readonly IKubernetes client;
        private readonly string ns;
        private readonly string name;
        private readonly string identity;
        private V1Endpoints endpointsLocal;

        public EndpointsLock(IKubernetes client, string @namespace, string name, string identity)
        {
            this.client = client;
            ns = @namespace;
            this.name = name;
            this.identity = identity;
        }

        public async Task<LeaderElectionRecord> GetAsync(CancellationToken cancellationToken = default)
        {
            var endpoints = await client.ReadNamespacedEndpointsAsync(name, ns, cancellationToken: cancellationToken)
                .ConfigureAwait(false);
            var recordRawStringContent = endpoints.GetAnnotation(LeaderElectionRecordAnnotationKey);

            if (string.IsNullOrEmpty(recordRawStringContent))
            {
                return new LeaderElectionRecord();
            }

            var record =
                JsonConvert.DeserializeObject<LeaderElectionRecord>(
                    recordRawStringContent,
                    client.DeserializationSettings);

            Interlocked.Exchange(ref endpointsLocal, endpoints);
            return record;
        }

        public async Task<bool> CreateAsync(LeaderElectionRecord record, CancellationToken cancellationToken = default)
        {
            var endpoints = new V1Endpoints(metadata: new V1ObjectMeta()
            {
                Name = name,
                NamespaceProperty = ns,
            });

            endpoints.SetAnnotation(
                LeaderElectionRecordAnnotationKey,
                JsonConvert.SerializeObject(record, client.SerializationSettings));

            try
            {
                var createdendpoints = await client
                    .CreateNamespacedEndpointsAsync(endpoints, ns, cancellationToken: cancellationToken)
                    .ConfigureAwait(false);

                Interlocked.Exchange(ref endpointsLocal, createdendpoints);
                return true;
            }
            catch (HttpOperationException)
            {
                // ignore
            }

            return false;
        }

        public async Task<bool> UpdateAsync(LeaderElectionRecord record, CancellationToken cancellationToken = default)
        {
            var endpoints = Interlocked.CompareExchange(ref endpointsLocal, null, null);
            if (endpoints == null)
            {
                throw new InvalidOperationException("endpoint not initialized, call get or create first");
            }

            endpoints.SetAnnotation(
                LeaderElectionRecordAnnotationKey,
                JsonConvert.SerializeObject(record, client.DeserializationSettings));

            try
            {
                await client.ReplaceNamespacedEndpointsAsync(endpoints, name, ns, cancellationToken: cancellationToken)
                    .ConfigureAwait(false);

                Interlocked.Exchange(ref endpointsLocal, endpoints);
                return true;
            }
            catch (HttpOperationException)
            {
                // ignore
            }

            return false;
        }

        public string Identity => identity;

        public string Describe()
        {
            return $"{ns}/{name}";
        }
    }
}
