namespace k8s.ClientSets;

public abstract class ResourceClient
{
    protected Kubernetes Client { get; }
    public ResourceClient(IKubernetes kubernetes)
    {
        Client = (Kubernetes)kubernetes;
    }
}
