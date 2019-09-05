using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.WebUtilities;

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
                var query = QueryHelpers.ParseQuery(request.RequestUri.Query);

                if (query.TryGetValue("watch", out var values) && values.Any(v => v == "true"))
                {
                    originResponse.Content = new LineSeparatedHttpContent(originResponse.Content,
                        cancellationToken);
                }
            }
            return originResponse;
        }

        internal class LineSeparatedHttpContent : HttpContent
        {
            private readonly HttpContent _originContent;
            private Stream _originStream;
            // This is to workaround https://github.com/dotnet/corefx/issues/9071
            private CancellationToken _cancellationToken;

            public LineSeparatedHttpContent(HttpContent originContent,
                CancellationToken cancellationToken)
            {
                _originContent = originContent;
                _cancellationToken = cancellationToken;
            }

            internal PeekableStreamReader StreamReader { get; private set; }

            protected override async Task SerializeToStreamAsync(Stream stream, TransportContext context)
            {
                _originStream = await _originContent.ReadAsStreamAsync();

                StreamReader = new PeekableStreamReader(_originStream);

                var firstLine = await StreamReader.PeekLineAsync(_cancellationToken);
                if (!string.IsNullOrEmpty(firstLine))
                {
                    var lineBytes = Encoding.UTF8.GetBytes(firstLine);
                    await stream.WriteAsync(lineBytes, 0, lineBytes.Length, _cancellationToken);
                    await stream.FlushAsync(_cancellationToken);
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
