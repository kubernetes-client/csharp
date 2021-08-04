using k8s.Models;

namespace k8s.Util.Informer.Cache
{
    // DeletedFinalStateUnknown is placed into a DeltaFIFO in the case where
    // an object was deleted but the watch deletion event was missed. In this
    // case we don't know the final "resting" state of the object, so there's
    // a chance the included `Obj` is stale.
    public class DeletedFinalStateUnknown<TApi> : IKubernetesObject<V1ObjectMeta>
      where TApi : class, IKubernetesObject<V1ObjectMeta>
    {
        private readonly string _key;
        private readonly TApi _obj;

        public DeletedFinalStateUnknown(string key, TApi obj)
        {
            _key = key;
            _obj = obj;
        }

        public string GetKey() => _key;

        /// <summary>
        /// Gets get obj.
        /// </summary>
        /// <returns>the get obj</returns>
        public TApi GetObj() => _obj;

        public V1ObjectMeta Metadata
        {
            get => _obj.Metadata;
            set => _obj.Metadata = value;
        }

        public string ApiVersion
        {
            get => _obj.ApiVersion;
            set => _obj.ApiVersion = value;
        }

        public string Kind
        {
            get => _obj.Kind;
            set => _obj.Kind = value;
        }
    }
}
