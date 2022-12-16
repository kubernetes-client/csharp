// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Net.Http;

namespace k8s.Autorest
{
    /// <summary>
    /// Wrapper around HttpRequestMessage type that copies properties of HttpRequestMessage so that
    /// they are available after the HttpClient gets disposed.
    /// </summary>
    public class HttpRequestMessageWrapper : HttpMessageWrapper
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HttpRequestMessageWrapper"/> class from HttpRequestMessage.
        /// and content.
        /// </summary>
#pragma warning disable SA1611 // Element parameters should be documented
        public HttpRequestMessageWrapper(HttpRequestMessage httpRequest, string content)
#pragma warning restore SA1611 // Element parameters should be documented
        {
            if (httpRequest == null)
            {
                throw new ArgumentNullException("httpRequest");
            }

            CopyHeaders(httpRequest.Headers);
            CopyHeaders(httpRequest.GetContentHeaders());

            Content = content;
            Method = httpRequest.Method;
            RequestUri = httpRequest.RequestUri;
        }

        /// <summary>
        /// Gets or sets the HTTP method used by the HTTP request message.
        /// </summary>
        public HttpMethod Method { get; protected set; }

        /// <summary>
        /// Gets or sets the Uri used for the HTTP request.
        /// </summary>
        public Uri RequestUri { get; protected set; }
    }
}
