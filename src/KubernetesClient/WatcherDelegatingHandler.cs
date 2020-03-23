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
            var originResponse = await base.SendAsync(request, cancellationToken).ConfigureAwait(false);

            if (originResponse.IsSuccessStatusCode)
            {
                var query = QueryHelpers.ParseQuery(request.RequestUri.Query);

                if (query.TryGetValue("watch", out var values) && values.Any(v => v == "true"))
                {
                    originResponse.Content = new LineSeparatedHttpContent(originResponse.Content, cancellationToken);
                }
            }

            return originResponse;
        }

        internal class CancelableStream : Stream
        {
            private readonly Stream _innerStream;
            private readonly CancellationToken _cancellationToken;

            public CancelableStream(Stream innerStream, CancellationToken cancellationToken)
            {
                _innerStream = innerStream;
                _cancellationToken = cancellationToken;
            }

            public override void Flush() => _innerStream.Flush();

            public override int Read(byte[] buffer, int offset, int count) =>
                _innerStream.ReadAsync(buffer, offset, count, _cancellationToken).GetAwaiter().GetResult();

            public override long Seek(long offset, SeekOrigin origin) => _innerStream.Seek(offset, origin);

            public override void SetLength(long value) => _innerStream.SetLength(value);

            public override void Write(byte[] buffer, int offset, int count) =>
                _innerStream.WriteAsync(buffer, offset, count, _cancellationToken).GetAwaiter().GetResult();

            public override bool CanRead => _innerStream.CanRead;

            public override bool CanSeek => _innerStream.CanSeek;

            public override bool CanWrite => _innerStream.CanWrite;

            public override long Length => _innerStream.Length;

            public override long Position
            {
                get => _innerStream.Position;
                set => _innerStream.Position = value;
            }
        }

        internal class LineSeparatedHttpContent : HttpContent
        {
            private readonly HttpContent _originContent;
            private readonly CancellationToken _cancellationToken;
            private Stream _originStream;

            public LineSeparatedHttpContent(HttpContent originContent, CancellationToken cancellationToken)
            {
                _originContent = originContent;
                _cancellationToken = cancellationToken;
            }

            internal PeekableStreamReader StreamReader { get; private set; }

            protected override async Task SerializeToStreamAsync(Stream stream, TransportContext context)
            {
                _originStream = await _originContent.ReadAsStreamAsync().ConfigureAwait(false);

                StreamReader = new PeekableStreamReader(new CancelableStream(_originStream, _cancellationToken));

                var firstLine = await StreamReader.PeekLineAsync().ConfigureAwait(false);

                var writer = new StreamWriter(stream);

                await writer.WriteAsync(firstLine).ConfigureAwait(false);
                await writer.FlushAsync().ConfigureAwait(false);
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
                var line = await ReadLineAsync().ConfigureAwait(false);
                _buffer.Enqueue(line);
                return line;
            }

            public override int Read() => throw new NotImplementedException();

            public override int Read(char[] buffer, int index, int count) => throw new NotImplementedException();

            public override Task<int> ReadAsync(char[] buffer, int index, int count) => throw new NotImplementedException();

            public override int ReadBlock(char[] buffer, int index, int count) => throw new NotImplementedException();

            public override Task<int> ReadBlockAsync(char[] buffer, int index, int count) => throw new NotImplementedException();

            public override string ReadToEnd() => throw new NotImplementedException();

            public override Task<string> ReadToEndAsync() => throw new NotImplementedException();
        }
    }
}
