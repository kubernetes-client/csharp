using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Rest;

namespace k8s
{
    public partial interface IKubernetes
    {        
        Task<HttpOperationResponse<L>> List<T, L>(string namespaceParameter, CancellationToken cancellationToken=default(CancellationToken), bool? watch = default(bool?));
    }
}
