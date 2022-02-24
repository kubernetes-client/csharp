// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace k8s.Autorest
{
    /// <summary>
    /// ServiceClientCredentials is the abstraction for credentials used by ServiceClients accessing REST services.
    /// </summary>
    public abstract class ServiceClientCredentials
    {
        /// <summary>
        /// Apply the credentials to the HTTP request.
        /// </summary>
        /// <param name="request">The HTTP request message.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>
        /// Task that will complete when processing has finished.
        /// </returns>
        public virtual Task ProcessHttpRequestAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            // Return an empty task by default
            return Task.FromResult<object>(null);
        }
    }
}
