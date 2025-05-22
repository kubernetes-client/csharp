namespace k8s.ClientSets
{
    /// <summary>
    /// Represents a base class for clients that interact with Kubernetes resources.
    /// Provides shared functionality for derived resource-specific clients.
    /// </summary>
    public partial class ClientSet
    {
        private readonly Kubernetes _kubernetes;

        public ClientSet(Kubernetes kubernetes)
        {
            _kubernetes = kubernetes;
        }
    }
}
