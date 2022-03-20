// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Net.Http;

namespace k8s.Autorest
{
    /// <summary>
    /// Represents the base return type of all ServiceClient REST operations without response body.
    /// </summary>
#pragma warning disable SA1649 // File name should match first type name
    public interface IHttpOperationResponse
#pragma warning restore SA1649 // File name should match first type name
    {
        /// <summary>
        /// Gets information about the associated HTTP request.
        /// </summary>
        HttpRequestMessage Request { get; set; }

        /// <summary>
        /// Gets information about the associated HTTP response.
        /// </summary>
        HttpResponseMessage Response { get; set; }
    }

    /// <summary>
    /// Represents the base return type of all ServiceClient REST operations with response body.
    /// </summary>
#pragma warning disable SA1618 // Generic type parameters should be documented
    public interface IHttpOperationResponse<T> : IHttpOperationResponse
#pragma warning restore SA1618 // Generic type parameters should be documented
    {
        /// <summary>
        /// Gets or sets the response object.
        /// </summary>
        T Body { get; set; }
    }

#pragma warning disable SA1622 // Generic type parameter documentation should have text
    /// <summary>
    /// Represents the base return type of all ServiceClient REST operations with a header response.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IHttpOperationHeaderResponse<T> : IHttpOperationResponse
#pragma warning restore SA1622 // Generic type parameter documentation should have text
    {
        /// <summary>
        /// Gets or sets the response header object.
        /// </summary>
        T Headers { get; set; }
    }

    /// <summary>
    /// Represents the base return type of all ServiceClient REST operations with response body and header.
    /// </summary>
#pragma warning disable SA1618 // Generic type parameters should be documented
    public interface IHttpOperationResponse<TBody, THeader> : IHttpOperationResponse<TBody>, IHttpOperationHeaderResponse<THeader>
#pragma warning restore SA1618 // Generic type parameters should be documented
    {
    }

    /// <summary>
    /// Represents the base return type of all ServiceClient REST operations without response body.
    /// </summary>
    public class HttpOperationResponse : IHttpOperationResponse, IDisposable
    {
        /// <summary>
        /// Indicates whether the HttpOperationResponse has been disposed.
        /// </summary>
        private bool _disposed;

        /// <summary>
        /// Gets information about the associated HTTP request.
        /// </summary>
        public HttpRequestMessage Request { get; set; }

        /// <summary>
        /// Gets information about the associated HTTP response.
        /// </summary>
        public HttpResponseMessage Response { get; set; }

        /// <summary>
        /// Dispose the HttpOperationResponse.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Dispose the HttpClient and Handlers.
        /// </summary>
        /// <param name="disposing">True to release both managed and unmanaged resources; false to releases only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                _disposed = true;

                // Dispose the request and response
                if (Request != null)
                {
                    Request.Dispose();
                }

                if (Response != null)
                {
                    Response.Dispose();
                }

                Request = null;
                Response = null;
            }
        }
    }

    /// <summary>
    /// Represents the base return type of all ServiceClient REST operations.
    /// </summary>
#pragma warning disable SA1402 // File may only contain a single type
#pragma warning disable SA1618 // Generic type parameters should be documented
    public class HttpOperationResponse<T> : HttpOperationResponse, IHttpOperationResponse<T>
#pragma warning restore SA1618 // Generic type parameters should be documented
#pragma warning restore SA1402 // File may only contain a single type
    {
        /// <summary>
        /// Gets or sets the response object.
        /// </summary>
        public T Body { get; set; }
    }
}
