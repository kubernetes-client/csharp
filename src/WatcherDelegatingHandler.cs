using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace k8s
{
    /// <summary>
    /// This HttpDelegatingHandler is to rewrite the response and return first line to autorest client
    /// then use WatchExt to create a watch object which interact with the replaced http response to get watch works.
    /// </summary>
    internal class WatcherDelegatingHandler : DelegatingHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            var originResponse = await base.SendAsync(request, cancellationToken);

            if (originResponse.IsSuccessStatusCode)
            {
                if ($"{request.RequestUri.Query}".Contains("watch=true"))
                {
                    originResponse.Content = new LineSeparatedHttpContent(originResponse.Content);
                }
            }
            return originResponse;
        }

        internal class LineSeparatedHttpContent : HttpContent
        {
            private readonly HttpContent _originContent;
            private Stream _originStream;

            public LineSeparatedHttpContent(HttpContent originContent)
            {
                _originContent = originContent;
            }

            internal StreamReader StreamReader { get; private set; }

            protected override async Task SerializeToStreamAsync(Stream stream, TransportContext context)
            {
                _originStream = await _originContent.ReadAsStreamAsync();

                StreamReader = new StreamReader(_originStream);

                var firstLine = await StreamReader.ReadLineAsync();
                var writer = new StreamWriter(stream);

//                using (writer) // leave open
                {
                    await writer.WriteAsync(firstLine);
                    await writer.FlushAsync();
                }
            }

            protected override bool TryComputeLength(out long length)
            {
                length = 0;
                return false;
            }
        }
    }
}