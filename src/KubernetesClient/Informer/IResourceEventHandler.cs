using System.Threading.Tasks;

namespace k8s.Informer
{
    public interface IResourceEventHandler<TApiType>
    {
        Task OnAdd(TApiType obj);

        Task OnUpdate(TApiType oldObj, TApiType newObj);

        Task OnDelete(TApiType obj, bool deletedFinalStateUnknown);
    }
}