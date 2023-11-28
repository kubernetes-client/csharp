// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Net.Http;
using System.Net.Http.Headers;

namespace k8s.Autorest
{
    /// <summary>
    /// Extensions for manipulating HTTP request and response objects.
    /// </summary>
    internal static class HttpExtensions
    {
        /// <summary>
        /// Get the content headers of an HttpRequestMessage.
        /// </summary>
        /// <param name="request">The request message.</param>
        /// <returns>The content headers.</returns>
        public static HttpHeaders GetContentHeaders(this HttpRequestMessage request)
        {
            return request?.Content?.Headers;
        }

        /// <summary>
        /// Get the content headers of an HttpResponseMessage.
        /// </summary>
        /// <param name="response">The response message.</param>
        /// <returns>The content headers.</returns>
        public static HttpHeaders GetContentHeaders(this HttpResponseMessage response)
        {
            return response?.Content?.Headers;
        }
    }
}
