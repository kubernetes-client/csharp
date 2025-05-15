namespace k8s.ClientSets;

public abstract class ResourceClient
{
    protected Kubernetes Client { get; }
    public ResourceClient(Kubernetes kubernetes)
    {
        Client = kubernetes;
    }
}
