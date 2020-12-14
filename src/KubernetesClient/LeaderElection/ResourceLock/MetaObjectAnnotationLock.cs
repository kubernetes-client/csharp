using System;
using System.Threading;
using System.Threading.Tasks;
using k8s.Models;
using Microsoft.Rest;
using Newtonsoft.Json;

namespace k8s.LeaderElection.ResourceLock
{
    public abstract class MetaObjectAnnotationLock<T> : MetaObjectLock<T>
        where T : class, IMetadata<V1ObjectMeta>, new()
    {
        private readonly JsonSerializerSettings serializationSettings;
        private readonly JsonSerializerSettings derializationSettings;

        protected MetaObjectAnnotationLock(IKubernetes client, string @namespace, string name, string identity)
            : base(client, @namespace, name, identity)
        {
            serializationSettings = client.SerializationSettings;
            derializationSettings = client.DeserializationSettings;
        }

        private const string LeaderElectionRecordAnnotationKey = "control-plane.alpha.kubernetes.io/leader";

        protected override LeaderElectionRecord GetLeaderElectionRecord(T obj)
        {
            var recordRawStringContent = obj.GetAnnotation(LeaderElectionRecordAnnotationKey);

            if (string.IsNullOrEmpty(recordRawStringContent))
            {
                return new LeaderElectionRecord();
            }

            var record =
                JsonConvert.DeserializeObject<LeaderElectionRecord>(
                    recordRawStringContent,
                    derializationSettings);
            return record;
        }


        protected override T SetLeaderElectionRecord(LeaderElectionRecord record, T metaObj)
        {
            metaObj.SetAnnotation(
                LeaderElectionRecordAnnotationKey,
                JsonConvert.SerializeObject(record, serializationSettings));

            return metaObj;
        }
    }
}
