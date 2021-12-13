using k8s.Models;


namespace k8s.LeaderElection.ResourceLock
{
    public abstract class MetaObjectAnnotationLock<T> : MetaObjectLock<T>
        where T : class, IMetadata<V1ObjectMeta>, new()
    {
        protected MetaObjectAnnotationLock(IKubernetes client, string @namespace, string name, string identity)
            : base(client, @namespace, name, identity)
        {
        }

        private const string LeaderElectionRecordAnnotationKey = "control-plane.alpha.kubernetes.io/leader";

        protected override LeaderElectionRecord GetLeaderElectionRecord(T obj)
        {
            var recordRawStringContent = obj.GetAnnotation(LeaderElectionRecordAnnotationKey);

            if (string.IsNullOrEmpty(recordRawStringContent))
            {
                return new LeaderElectionRecord();
            }

            var record = KubernetesJson.Deserialize<LeaderElectionRecord>(recordRawStringContent);
            return record;
        }


        protected override T SetLeaderElectionRecord(LeaderElectionRecord record, T metaObj)
        {
            metaObj.SetAnnotation(LeaderElectionRecordAnnotationKey, KubernetesJson.Serialize(record));
            return metaObj;
        }
    }
}
