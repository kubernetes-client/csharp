// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

namespace k8s.Autorest
{
    /// <summary>
    /// Exception thrown for an invalid response with custom error information.
    /// </summary>
    public class HttpOperationException : Exception
    {
        /// <summary>
        /// Gets information about the associated HTTP request.
        /// </summary>
        public HttpRequestMessageWrapper Request { get; set; }

        /// <summary>
        /// Gets information about the associated HTTP response.
        /// </summary>
        public HttpResponseMessageWrapper Response { get; set; }

        /// <summary>
        /// Gets or sets the response object.
        /// </summary>
        public object Body { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpOperationException"/> class.
        /// </summary>
        public HttpOperationException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpOperationException"/> class.
        /// </summary>
        /// <param name="message">The exception message.</param>
        public HttpOperationException(string message)
            : this(message, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpOperationException"/> class.
        /// </summary>
        /// <param name="message">The exception message.</param>
        /// <param name="innerException">Inner exception.</param>
        public HttpOperationException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
