using System;
using System.Threading.Tasks;
using k8s.Models;
using Microsoft.Rest;
using k8s.Util.Utils;

namespace k8s.Util.Cache
{
  public interface IListerWatcher<TApiType, TApiListType>
    where TApiType : IKubernetesObject<V1ObjectMeta>
    where TApiListType : IKubernetesObject<V1ListMeta>
  {
    Task<HttpOperationResponse<TApiListType>> List(CallGeneratorParams param);

    Task<HttpOperationResponse<TApiListType>> Watch(CallGeneratorParams param);
  }
}
