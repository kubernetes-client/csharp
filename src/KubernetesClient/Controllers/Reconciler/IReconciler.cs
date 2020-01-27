using System.Threading.Tasks;

namespace k8s.Controllers.Reconciler
{
    public interface IReconciler
    {
        Task<Result> Reconcile(Request request);

    }
}