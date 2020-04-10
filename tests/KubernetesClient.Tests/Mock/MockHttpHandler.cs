using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace k8s.Tests.Mock
{
    class MockHttpHandler : HttpClientHandler
    {
        public MockHttpHandler(Func<HttpRequestMessage, HttpResponseMessage> respFunc) => this.respFunc = respFunc;

        public HttpRequestMessage Request;

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            Request = request;
            return Task.FromResult(respFunc(request));
        }

        readonly Func<HttpRequestMessage, HttpResponseMessage> respFunc;
    }
}
