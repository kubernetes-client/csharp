// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Net;
using System.Net.Http;

namespace k8s.Autorest
{
    /// <summary>
    /// Wrapper around HttpResponseMessage type that copies properties of HttpResponseMessage so that
    /// they are available after the HttpClient gets disposed.
    /// </summary>
    public class HttpResponseMessageWrapper : HttpMessageWrapper
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HttpResponseMessageWrapper"/> class from HttpResponseMessage.
        /// and content.
        /// </summary>
#pragma warning disable SA1611 // Element parameters should be documented
        public HttpResponseMessageWrapper(HttpResponseMessage httpResponse, string content)
#pragma warning restore SA1611 // Element parameters should be documented
        {
            if (httpResponse == null)
            {
                throw new ArgumentNullException("httpResponse");
            }

            this.CopyHeaders(httpResponse.Headers);
            this.CopyHeaders(httpResponse.GetContentHeaders());

            this.Content = content;
            this.StatusCode = httpResponse.StatusCode;
            this.ReasonPhrase = httpResponse.ReasonPhrase;
        }

        /// <summary>
        /// Gets or sets the status code of the HTTP response.
        /// </summary>
        public HttpStatusCode StatusCode { get; protected set; }

        /// <summary>
        /// Exposes the reason phrase, typically sent along with the status code.
        /// </summary>
        public string ReasonPhrase { get; protected set; }
    }
}
