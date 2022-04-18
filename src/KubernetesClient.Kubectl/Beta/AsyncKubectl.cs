namespace k8s.kubectl.beta;

public partial class AsyncKubectl
{
    private readonly IKubernetes client;

    public AsyncKubectl(IKubernetes client)
    {
        this.client = client;
    }
}
