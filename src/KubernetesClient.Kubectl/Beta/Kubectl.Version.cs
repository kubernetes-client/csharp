using static k8s.kubectl.beta.AsyncKubectl;

namespace k8s.kubectl.beta;

public partial class Kubectl
{
    // TODO should auto generate this
    public KubeVersion Version()
    {
        return client.Version().GetAwaiter().GetResult();
    }
}
