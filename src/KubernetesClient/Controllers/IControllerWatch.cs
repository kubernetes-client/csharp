using k8s.Informer;

namespace k8s.Controllers
{
    public interface IControllerWatch<TApiType>
    {
        IResourceEventHandler<TApiType> ResourceEventHandler { get; }

    }
}