// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

#pragma warning disable SA1606
#pragma warning disable SA1614
namespace k8s.Autorest
{
    /// <summary>
    /// Interface to a source of access tokens.
    /// </summary>
    public interface ITokenProvider
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns>AuthenticationHeaderValue</returns>
        Task<AuthenticationHeaderValue> GetAuthenticationHeaderAsync(CancellationToken cancellationToken);
    }
}
#pragma warning restore SA1614
#pragma warning restore SA1606
