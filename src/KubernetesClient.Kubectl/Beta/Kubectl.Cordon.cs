namespace k8s.kubectl.beta;

public partial class Kubectl
{
    public void Cordon(string nodeName)
    {
        client.Cordon(nodeName).GetAwaiter().GetResult();
    }

    public void Uncordon(string nodeName)
    {
        client.Uncordon(nodeName).GetAwaiter().GetResult();
    }
}
