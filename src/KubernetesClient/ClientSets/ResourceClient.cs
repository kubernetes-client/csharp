namespace k8s.ClientSets
{
    /// <summary>
    /// Represents a set of Kubernetes clients for interacting with the Kubernetes API.
    /// This class provides access to various client implementations for managing Kubernetes resources.
    /// </summary>
    public abstract class ResourceClient
    {
        protected Kubernetes Client { get; }

        public ResourceClient(Kubernetes kubernetes)
        {
            Client = kubernetes;
        }
    }
}
