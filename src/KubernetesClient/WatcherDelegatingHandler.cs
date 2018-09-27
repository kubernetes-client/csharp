using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
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

            internal PeekableStreamReader StreamReader { get; private set; }

            protected override async Task SerializeToStreamAsync(Stream stream, TransportContext context)
            {
                _originStream = await _originContent.ReadAsStreamAsync();

                StreamReader = new PeekableStreamReader(_originStream);

                var firstLine = await StreamReader.PeekLineAsync();

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
        internal class PeekableStreamReader : StreamReader
        {
            private Queue<string> _buffer;
            public PeekableStreamReader(Stream stream) : base(stream)
            {
                _buffer = new Queue<string>();
            }

            public override string ReadLine()
            {
                if (_buffer.Count > 0)
                {
                    return _buffer.Dequeue();
                }
                return base.ReadLine();
            }
            public override Task<string> ReadLineAsync()
            {
                if (_buffer.Count > 0)
                {
                    return Task.FromResult(_buffer.Dequeue());
                }
                return base.ReadLineAsync();
            }
            public async Task<string> PeekLineAsync()
            {
                var line = await ReadLineAsync();
                _buffer.Enqueue(line);
                return line;
            }

            public override int Read()
            {
                throw new NotImplementedException();
            }

            public override int Read(char[] buffer, int index, int count)
            {
                throw new NotImplementedException();
            }
            public override Task<int> ReadAsync(char[] buffer, int index, int count)
            {
                throw new NotImplementedException();
            }
            public override int ReadBlock(char[] buffer, int index, int count)
            {
                throw new NotImplementedException();
            }
            public override Task<int> ReadBlockAsync(char[] buffer, int index, int count)
            {
                throw new NotImplementedException();
            }
            public override string ReadToEnd()
            {
                throw new NotImplementedException();
            }
            public override Task<string> ReadToEndAsync()
            {
                throw new NotImplementedException();
            }
        }
    }
}
