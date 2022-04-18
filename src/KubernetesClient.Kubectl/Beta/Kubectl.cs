namespace k8s.kubectl.beta;

public partial class Kubectl
{
    private readonly AsyncKubectl client;

    public Kubectl(IKubernetes client)
    {
        this.client = new AsyncKubectl(client);
    }
}
